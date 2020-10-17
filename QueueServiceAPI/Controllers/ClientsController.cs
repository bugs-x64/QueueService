using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QueueServiceAPI.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QueueServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly QueueServiceDbContext _context;

        public ClientsController(QueueServiceDbContext context)
        {
            _context = context;
        }

        private async Task<ActionResult<string>> GetAllClients()
        {
            Thread.Sleep(Config.SyntheticDelay);
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
            Thread.Sleep(Config.SyntheticDelay);
            Client client = await _context.Clients.FirstOrDefaultAsync(x => x.Fio == fio);

            if (client == null)
            {
                return NotFound();
            }

            return JsonConvert.SerializeObject(new
            {
                id = client.Id,
                fio = client.Fio
            },
            new JsonSerializerSettings { Formatting = Formatting.None });
        }

        // GET: api/Client
        //получить список сотрудников
        [HttpGet]
        public async Task<ActionResult<string>> GetClients(string fio)
        {
            Thread.Sleep(Config.SyntheticDelay);
            if (fio is null || fio == "")
            {
                return await GetAllClients();
            }
            else
            {
                return await GetClient(fio);
            }
        }

        // GET: api/Client/5
        //получить информацию о сотруднике по id
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetClients(int id)
        {
            Thread.Sleep(Config.SyntheticDelay);
            Client client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return JsonConvert.SerializeObject(new
            {
                id = client.Id,
                fio = client.Fio
            },
            new JsonSerializerSettings { Formatting = Formatting.None });
        }


        // POST: api/Client
        // добавить нового сотрудника
        [HttpPost]
        public async Task<ActionResult<string>> PostClients([FromBody]Client client)
        {
            Thread.Sleep(Config.SyntheticDelay);
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClients", new { id = client.Id }, client);
        }



        // POST: api/Client
        // авторизация
        [HttpPost("auth")]
        public async Task<ActionResult<string>> Auth([FromBody]Client client)
        {
            Thread.Sleep(Config.SyntheticDelay);
            if (await _context.Clients.AnyAsync(x => x.Fio == client.Fio))
            {
                return await GetClient(fio: client.Fio);
            }
            else
            {
                return await PostClients(client);
            };
        }
    }
}
