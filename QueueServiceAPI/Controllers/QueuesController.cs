using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QueueServiceAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QueueServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueuesController : ControllerBase
    {
        private readonly QueueServiceDbContext _context;

        public QueuesController(QueueServiceDbContext context)
        {
            _context = context;
        }

        // GET: api/Queue
        // получение списка клиентов в очереди
        [HttpGet]
        public async Task<ActionResult<string>> GetQueues()
        {
            Thread.Sleep(Config.SyntheticDelay);
            var response = await (from record in _context.Queues
                                  join client in _context.Clients on record.Clientid equals client.Id
                                  where record.Employeeid == 0
                                  select new
                                  {
                                      recid = record.Id,
                                      fio = client.Fio,
                                      competing = record.Competing
                                  }
                         ).ToListAsync();
            return JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.None });
        }

        // GET: api/Queue/next
        // получение списка клиентов в очереди
        [HttpPost("next")]
        public async Task<ActionResult<string>> GetNext([FromBody] int employeeid)
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
                return NotFound();
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

            var response = new
            {
                recid = result.record.Id,
                fio = result.client.Fio,
                competing = result.record.Competing
            };

            return JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.None });
        }


        // PUT: api/Queue/5
        // сотрудник забирает клиента из очереди
        [HttpPut("{id}")]
        public async Task<IActionResult> PickClient(int id, [FromBody] int employeeid)
        {
            Thread.Sleep(Config.SyntheticDelay);
            if (!QueuesExists(id))
            {
                return NotFound();
            }
            Queue record = await _context.Queues.FindAsync(id);
            record.Employeeid = employeeid;
            _context.Entry(record).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QueuesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Queue
        // встать в очередь
        [HttpPost]
        public async Task<IActionResult> PostQueues([FromBody] string fio, bool c)
        {
            Thread.Sleep(Config.SyntheticDelay);
            if (fio == "" || fio is null)
            {
                return BadRequest();
            }

            Client client = await _context.Clients.FirstOrDefaultAsync(x => x.Fio == fio);

            if (client is null)
            {
                client = new Client() { Fio = fio };
                _context.Clients.Add(client);
            }

            Queue queue = new Queue() { Client = client, Competing = c, Id = 0 };

            _context.Queues.Add(queue);
            await _context.SaveChangesAsync();

            return Ok();
        }


        private bool QueuesExists(int id)
        {
            return _context.Queues.Any(e => e.Id == id && e.Employeeid == 0);
        }
    }
}
