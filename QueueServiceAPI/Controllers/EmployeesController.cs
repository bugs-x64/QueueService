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
    public class EmployeesController : ControllerBase
    {
        private readonly QueueServiceDbContext _context;

        public EmployeesController(QueueServiceDbContext context)
        {
            _context = context;
        }

        private async Task<ActionResult<string>> GetAllEmployees()
        {
            Thread.Sleep(Config.SyntheticDelay);
            var response = await (from empl in _context.Employees
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
            Thread.Sleep(Config.SyntheticDelay);
            Employee employee = await _context.Employees.FirstOrDefaultAsync(x => x.Fio == fio);

            if (employee == null)
            {
                return NotFound();
            }

            return JsonConvert.SerializeObject(new
            {
                id = employee.Id,
                fio = employee.Fio
            },
            new JsonSerializerSettings { Formatting = Formatting.None });
        }

        // GET: api/Employee
        //получить список сотрудников
        [HttpGet]
        public async Task<ActionResult<string>> GetEmployees(string fio)
        {
            Thread.Sleep(Config.SyntheticDelay);
            if (fio is null || fio == "")
            {
                return await GetAllEmployees();
            }
            else
            {
                return await GetEmployee(fio);
            }
        }

        // GET: api/Employee/5
        //получить информацию о сотруднике по id
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetEmployees(int id)
        {
            Thread.Sleep(Config.SyntheticDelay);
            Employee employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return JsonConvert.SerializeObject(new
            {
                id = employee.Id,
                fio = employee.Fio
            },
            new JsonSerializerSettings { Formatting = Formatting.None });
        }


        // POST: api/Employee
        // добавить нового сотрудника
        [HttpPost]
        public async Task<ActionResult<string>> PostEmployees([FromBody]Employee employee)
        {
            Thread.Sleep(Config.SyntheticDelay);
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmployees", new { id = employee.Id }, employee);
        }



        // POST: api/Employee
        // авторизация сотрудника
        [HttpPost("auth")]
        public async Task<ActionResult<string>> Auth([FromBody]Employee employee)
        {
            Thread.Sleep(Config.SyntheticDelay);
            if (await _context.Employees.AnyAsync(x => x.Fio == employee.Fio))
            {
                return await GetEmployee(fio: employee.Fio);
            }
            else
            {
                return await PostEmployees(employee);
            };
        }
    }
}
