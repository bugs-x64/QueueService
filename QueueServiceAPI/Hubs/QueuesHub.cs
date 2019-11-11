using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;
using QueueServiceAPI.Models;

namespace QueueServiceAPI.Hubs
{
    public class QueuesHub:Hub
    {
        private readonly qsdbContext _context;
        public QueuesHub()
        {
            _context = new qsdbContext();
        }
   
        /// <summary>
        /// Получить список клиентов в очереди
        /// </summary>
        /// <returns></returns>
        public async Task GetQueues()
        {
            Thread.Sleep(10 * 1000);
            var response = await (from record in _context.Queues
                                  join client in _context.Clients on record.Clientid equals client.Id
                                  where record.Employeeid == 0
                                  select new QueueRecord()
                                  {
                                      RecId = record.Id,
                                      Fio = client.Fio,
                                      Competing = record.Competing
                                  }
                                 ).ToListAsync();
            await Clients.Caller.SendAsync("SetQueues", response);
        }

        /// <summary>
        /// следующего клиента из неконкурирующей очереди
        /// </summary>
        /// <param name="employeeid">id сотрудника</param>
        /// <returns></returns>
        public async Task<QueueRecord> GetNext(int employeeid)
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

            if (datalist.Count == 0) return null;

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

            return new QueueRecord()
            {
                RecId = result.record.Id,
                Fio = result.client.Fio,
                Competing = result.record.Competing
            };                       
        }


        /// <summary>
        /// метод возвращает true, если сотрудник успешно забрал клиента
        /// </summary>
        /// <param name="clientid">id клиента</param>
        /// <param name="employeeid">id сотрудника</param>
        /// <returns></returns>
        public async Task<bool> PickClient(int clientid, int employeeid)
        {
            Thread.Sleep(10 * 1000);
            if (!QueuesExists(clientid))
            {
                return false;
            }

            var record = await _context.Queues.FindAsync(clientid);
            record.Employeeid = employeeid;
            _context.Entry(record).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QueuesExists(clientid))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

            return true;
        }
        
        /// <summary>
        /// Метод возвращает true, если клиент успешно записан в очередь
        /// </summary>
        /// <param name="fio"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public async Task<bool> PostQueues(string fio, bool c)
        {
            Thread.Sleep(10 * 1000);
            if (fio == "" || fio is null) return false;

            var client = await _context.Clients.FirstOrDefaultAsync(x => x.Fio == fio);

            if (client is null)
            {
                client = new Clients() { Fio = fio };
                _context.Clients.Add(client);
            }

            var queues = new Queues() { Client = client, Competing = c };

            _context.Queues.Add(queues);
            await _context.SaveChangesAsync();

            return true;
        }


        private bool QueuesExists(int id)
        {
            return _context.Queues.Any(e => e.Id == id && e.Employeeid == 0);
        }
    }
}
