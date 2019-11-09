using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QueueServiceAPI;

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

        private async Task<ActionResult<string>> GetEmployee(string fio)
        {
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
            if (fio is null || fio == "") return await GetAllClients();
            else return await GetEmployee(fio);
        }

        // GET: api/Clients/5
        //получить информацию о сотруднике по id
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetClients(int id)
        {
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
        public async Task<ActionResult<string>> PostClients(Clients clients)
        {
            _context.Clients.Add(clients);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClients", new { id = clients.Id }, clients);
        }



        // POST: api/Clients
        // добавить нового сотрудника
        [HttpPost("auth")]
        public async Task<ActionResult<string>> Auth(Clients clients)
        {
            if (await _context.Clients.AnyAsync(x => x.Fio == clients.Fio))
                return await GetEmployee(fio: clients.Fio);
            else
                return await PostClients(clients); ;
            //return CreatedAtAction("GetClients", new { id = clients.Id }, clients);
        }
    }
}
