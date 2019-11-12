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
        private readonly qsdbContext _context;
        public QueuesHub()
        {
            _context = new qsdbContext();
        }

        /// <summary>
        /// Получить список клиентов в очереди из БД
        /// </summary>
        /// <returns></returns>
        private async Task<List<QueueRecord>> GetQueuesData()
        {
            return await (from record in _context.Queues
                                                join client in _context.Clients on record.Clientid equals client.Id
                                                where record.Employeeid == 0
                                                select new QueueRecord()
                                                {
                                                    RecId = record.Id,
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
                await Clients.All.SendAsync("QueuesUpdate", GetQueuesData());
            else
            {
                /* ============================================================
                 * В данном методе задержка стоит только здесь
                 * т.к. all == true используется только при вызове других методов. 
                 * Если оставить в начале выполнения метода, 
                 * то другие методы будут выполняться 20 секунд
                 * ============================================================
                 */
                Thread.Sleep(10 * 1000);
                await Clients.Caller.SendAsync("QueuesUpdate", GetQueuesData());
            }
        }

        /// <summary>
        /// следующего клиента из неконкурирующей очереди
        /// </summary>
        /// <param name="employeeid">id сотрудника</param>
        /// <returns></returns>
        public async Task GetNext(int employeeid)
        {
            Thread.Sleep(10 * 1000);
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

            await Clients.Caller.SendAsync("RequestResult", true, new QueueRecord()
            {
                RecId = result.record.Id,
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
            Thread.Sleep(10 * 1000);
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

            await Clients.Caller.SendAsync("RequestResult", true, new QueueRecord()
            {
                RecId = result.record.Id,
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
            Thread.Sleep(10 * 1000);
            if (fio == "" || fio is null)
            {
                await Clients.Caller.SendAsync("SendingStatus", false);
                return;
            }

            Clients client = await _context.Clients.FirstOrDefaultAsync(x => x.Fio == fio);

            if (client is null)
            {
                client = new Clients() { Fio = fio };
                _context.Clients.Add(client);
            }

            Queues queues = new Queues() { Client = client, Competing = c };

            _context.Queues.Add(queues);
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
