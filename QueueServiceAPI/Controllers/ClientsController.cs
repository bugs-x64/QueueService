using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QueueServiceAPI.Models;

namespace QueueServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly qsdbContext _context;

        public ClientsController(qsdbContext context)
        {
            _context = context;
        }

        private async Task<ActionResult<string>> GetAllClients()
        {
            Thread.Sleep(10 * 1000);
            var response = await (from empl in _context.Clients
                                  orderby empl.Fio ascending
                                  select new
                                  {
                                      id = empl.Id,
                                      fio = empl.Fio
                                  }
                         ).ToListAsync();
            return JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.None });
        }

        private async Task<ActionResult<string>> GetClient(string fio)
        {
            Thread.Sleep(10 * 1000);
            var clients = await _context.Clients.FirstOrDefaultAsync(x => x.Fio == fio);

            if (clients == null)
            {
                return NotFound();
            }

            return JsonConvert.SerializeObject(new
            {
                id = clients.Id,
                fio = clients.Fio
            },
            new JsonSerializerSettings { Formatting = Formatting.None });
        }

        // GET: api/Clients
        //получить список сотрудников
        [HttpGet]
        public async Task<ActionResult<string>> GetClients(string fio)
        {
            Thread.Sleep(10 * 1000);
            if (fio is null || fio == "") return await GetAllClients();
            else return await GetClient(fio);
        }

        // GET: api/Clients/5
        //получить информацию о сотруднике по id
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetClients(int id)
        {
            Thread.Sleep(10 * 1000);
            var clients = await _context.Clients.FindAsync(id);

            if (clients == null)
            {
                return NotFound();
            }

            return JsonConvert.SerializeObject(new
            {
                id = clients.Id,
                fio = clients.Fio
            },
            new JsonSerializerSettings { Formatting = Formatting.None });
        }


        // POST: api/Clients
        // добавить нового сотрудника
        [HttpPost]
        public async Task<ActionResult<string>> PostClients([FromBody]Clients clients)
        {
            Thread.Sleep(10 * 1000);
            _context.Clients.Add(clients);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClients", new { id = clients.Id }, clients);
        }



        // POST: api/Clients
        // авторизация
        [HttpPost("auth")]
        public async Task<ActionResult<string>> Auth([FromBody]Clients clients)
        {
            Thread.Sleep(10 * 1000);
            if (await _context.Clients.AnyAsync(x => x.Fio == clients.Fio))
                return await GetClient(fio: clients.Fio);
            else
                return await PostClients(clients); ;
            //return CreatedAtAction("GetClients", new { id = clients.Id }, clients);
        }
    }
}
