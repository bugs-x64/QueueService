using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QueueServiceAPI;

namespace QueueServiceAPI.Controllers
{
    //TODO: написать методы API для очередей
    [Route("api/[controller]")]
    [ApiController]
    public class QueuesController : ControllerBase
    {
        private readonly qsdbContext _context;

        public QueuesController(qsdbContext context)
        {
            _context = context;
        }

        // GET: api/Queues
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Queues>>> GetQueues()
        {
            return await _context.Queues.ToListAsync();
        }

        // GET: api/Queues/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Queues>> GetQueues(int id)
        {
            var queues = await _context.Queues.FindAsync(id);

            if (queues == null)
            {
                return NotFound();
            }

            return queues;
        }

        // PUT: api/Queues/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQueues(int id, Queues queues)
        {
            if (id != queues.Id)
            {
                return BadRequest();
            }

            _context.Entry(queues).State = EntityState.Modified;

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

        // POST: api/Queues
        [HttpPost]
        public async Task<ActionResult<Queues>> PostQueues(Queues queues)
        {
            _context.Queues.Add(queues);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQueues", new { id = queues.Id }, queues);
        }

        // DELETE: api/Queues/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Queues>> DeleteQueues(int id)
        {
            var queues = await _context.Queues.FindAsync(id);
            if (queues == null)
            {
                return NotFound();
            }

            _context.Queues.Remove(queues);
            await _context.SaveChangesAsync();

            return queues;
        }

        private bool QueuesExists(int id)
        {
            return _context.Queues.Any(e => e.Id == id);
        }
    }
}
