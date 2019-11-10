using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    public class EmployeesController : ControllerBase
    {
        private readonly qsdbContext _context;

        public EmployeesController(qsdbContext context)
        {
            _context = context;
        }

        private async Task<ActionResult<string>> GetAllEmployees()
        {
            Thread.Sleep(10 * 1000);
            var response = await (from empl in _context.Employees
                                  orderby empl.Fio ascending
                                  select new
                                  {
                                     id =  empl.Id,
                                     fio =  empl.Fio
                                  }
                         ).ToListAsync();
            return JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.None });
        }

        private async Task<ActionResult<string>> GetEmployee(string fio)
        {
            Thread.Sleep(10 * 1000);
            var employees = await _context.Employees.FirstOrDefaultAsync(x => x.Fio == fio);

            if (employees == null)
            {
                return NotFound();
            }

            return JsonConvert.SerializeObject(new
            {
               id = employees.Id,
               fio = employees.Fio
            },
            new JsonSerializerSettings { Formatting = Formatting.None });
        }

        // GET: api/Employees
        //получить список сотрудников
        [HttpGet]
        public async Task<ActionResult<string>> GetEmployees(string fio)
        {
            Thread.Sleep(10 * 1000);
            if (fio is null || fio == "") return await GetAllEmployees();
            else return await GetEmployee(fio);
        }

        // GET: api/Employees/5
        //получить информацию о сотруднике по id
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetEmployees(int id)
        {
            Thread.Sleep(10 * 1000);
            var employees = await _context.Employees.FindAsync(id);

            if (employees == null)
            {
                return NotFound();
            }

            return JsonConvert.SerializeObject(new
            {
                id = employees.Id,
                fio = employees.Fio
            },
            new JsonSerializerSettings { Formatting = Formatting.None });
        }


        // POST: api/Employees
        // добавить нового сотрудника
        [HttpPost]
        public async Task<ActionResult<string>> PostEmployees([FromBody]Employees employees)
        {
            Thread.Sleep(10 * 1000);
            _context.Employees.Add(employees);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmployees", new { id = employees.Id }, employees);
        }



        // POST: api/Employees
        // авторизация сотрудника
        [HttpPost("auth")]
        public async Task<ActionResult<string>> Auth([FromBody]Employees employees)
        {
            Thread.Sleep(10 * 1000);
            if (await _context.Employees.AnyAsync(x => x.Fio == employees.Fio))
                return await GetEmployee(fio: employees.Fio);
            else
                return await PostEmployees(employees); ;
            //return CreatedAtAction("GetEmployees", new { id = employees.Id }, employees);
        }
    }
}
