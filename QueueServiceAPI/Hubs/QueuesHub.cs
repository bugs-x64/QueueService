using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using QueueServiceAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QueueServiceAPI.Hubs
{
    public class QueuesHub : Hub
    {
        private readonly QueueServiceDbContext _context;
        public QueuesHub(QueueServiceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить список клиентов в очереди из БД
        /// </summary>
        /// <returns></returns>
        private async Task<List<QueueRecordDto>> GetQueuesData()
        {
            return await (from record in _context.Queues
                          join client in _context.Clients on record.Clientid equals client.Id
                          where record.Employeeid == 0
                          select new QueueRecordDto()
                          {
                              Id = record.Id,
                              Fio = client.Fio,
                              Competing = record.Competing
                          }
                          ).ToListAsync();
        }

        /// <summary>
        /// Отправить список клиентов в очереди. Если (all == true) список отправится всем подключенным клиентам
        /// </summary>
        /// <param name="all"></param>
        /// <returns></returns>
        public async Task GetQueues(bool all)
        {
            if (all)
                await Clients.All.SendAsync("QueuesUpdate", await GetQueuesData());
            else
            {
                /*
                 * ============================================================
                 * В данном методе задержка стоит только здесь
                 * т.к. all == true используется только при вызове других методов. 
                 * Если оставить в начале выполнения метода, 
                 * то другие методы будут выполняться 20 секунд
                 * ============================================================
                 */

                Thread.Sleep(Config.SyntheticDelay);
                await Clients.Caller.SendAsync("QueuesUpdate", await GetQueuesData());
            }
        }

        /// <summary>
        /// следующего клиента из неконкурирующей очереди
        /// </summary>
        /// <param name="employeeid">id сотрудника</param>
        /// <returns></returns>
        public async Task GetNext(int employeeid)
        {
            Thread.Sleep(Config.SyntheticDelay);
            var datalist = await (from record in _context.Queues
                                  join client in _context.Clients on record.Clientid equals client.Id
                                  orderby record.Id ascending
                                  where record.Employeeid == 0 && record.Competing == false
                                  select new
                                  {
                                      client,
                                      record
                                  }).ToListAsync();

            if (datalist.Count == 0)
            {
                await Clients.Caller.SendAsync("RequestResult", false, null);
                return;
            }

            var result = datalist[0];
            result.record.Employeeid = employeeid;
            _context.Entry(result.record).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            await Clients.Caller.SendAsync("RequestResult", true, new QueueRecordDto()
            {
                Id = result.record.Id,
                Fio = result.client.Fio,
                Competing = result.record.Competing
            });
            await GetQueues(true);
        }


        /// <summary>
        /// метод возвращает true, если сотрудник успешно забрал клиента
        /// </summary>
        /// <param name="clientid">id клиента</param>
        /// <param name="employeeid">id сотрудника</param>
        /// <returns></returns>
        public async Task GetSelected(int recordid, int employeeid)
        {
            Thread.Sleep(Config.SyntheticDelay);
            if (!QueuesExists(recordid))
            {
                await Clients.Caller.SendAsync("RequestResult", false, null);
                return;
            }

            var result = await (from record in _context.Queues
                                join client in _context.Clients on record.Clientid equals client.Id
                                where record.Id == recordid
                                select new
                                {
                                    record,
                                    client
                                }
                                 ).FirstAsync();

            result.record.Employeeid = employeeid;
            _context.Entry(result.record).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QueuesExists(recordid))
                {
                    return;
                }
                else
                {
                    throw;
                }
            }

            await Clients.Caller.SendAsync("RequestResult", true, new QueueRecordDto()
            {
                Id = result.record.Id,
                Fio = result.client.Fio,
                Competing = result.record.Competing
            });
            await GetQueues(true);
        }

        /// <summary>
        /// Метод возвращает true, если клиент успешно записан в очередь
        /// </summary>
        /// <param name="fio"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public async Task CreateRecord(string fio, bool c)
        {
            Thread.Sleep(Config.SyntheticDelay);
            if (fio == "" || fio is null)
            {
                await Clients.Caller.SendAsync("SendingStatus", false);
                return;
            }

            Client client = await _context.Clients.FirstOrDefaultAsync(x => x.Fio == fio);

            if (client is null)
            {
                client = new Client() { Fio = fio };
                _context.Clients.Add(client);
            }

            Queue queue = new Queue() { Client = client, Competing = c };

            _context.Queues.Add(queue);
            await _context.SaveChangesAsync();

            await Clients.Caller.SendAsync("SendingStatus", true);
            await GetQueues(true);
        }


        private bool QueuesExists(int id)
        {
            return _context.Queues.Any(e => e.Id == id && e.Employeeid == 0);
        }
    }
}
