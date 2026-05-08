using lektion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lektion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviationsController : ControllerBase
    {
        private readonly AgroControlDbContext _context;

        public DeviationsController(AgroControlDbContext context)
        {
            _context = context;
        }

        // GET: api/deviations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Deviation>>> GetDeviations()
        {
            return await _context.Deviations.ToListAsync();
        }

        // GET: api/deviations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Deviation>> GetDeviation(int id)
        {
            var deviation = await _context.Deviations.FindAsync(id);
            if (deviation == null)
                return NotFound(new { message = $"Отклонение с id {id} не найдено" });
            return deviation;
        }

        // GET: api/deviations/batch/1
        [HttpGet("batch/{batchId}")]
        public async Task<ActionResult<IEnumerable<Deviation>>> GetByBatch(int batchId)
        {
            var deviations = await _context.Deviations
                .Where(d => d.batch_id == batchId)
                .ToListAsync();
            return Ok(deviations);
        }

        // GET: api/deviations/severity/critical
        [HttpGet("severity/{severity}")]
        public async Task<ActionResult<IEnumerable<Deviation>>> GetBySeverity(string severity)
        {
            var deviations = await _context.Deviations
                .Where(d => d.severity == severity)
                .ToListAsync();
            return Ok(deviations);
        }

        // POST: api/deviations
        [HttpPost]
        public async Task<ActionResult<Deviation>> CreateDeviation(Deviation deviation)
        {
            deviation.created_at = DateTime.Now;
            _context.Deviations.Add(deviation);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDeviation), new { id = deviation.deviation_id }, deviation);
        }

        // PUT: api/deviations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDeviation(int id, Deviation deviation)
        {   
            if (id != deviation.deviation_id)
                return BadRequest(new { message = "ID не совпадают" });

            var existing = await _context.Deviations.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Отклонение с id {id} не найдено" });

            existing.description = deviation.description;
            existing.severity = deviation.severity;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/deviations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeviation(int id)
        {
            var deviation = await _context.Deviations.FindAsync(id);
            if (deviation == null)
                return NotFound(new { message = $"Отклонение с id {id} не найдено" });

            _context.Deviations.Remove(deviation);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Отклонение удалено" });
        }
    }
}