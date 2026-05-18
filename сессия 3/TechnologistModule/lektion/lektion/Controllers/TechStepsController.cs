using lektion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lektion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TechStepsController : ControllerBase
    {
        private readonly AgroControlDbContext _context;

        public TechStepsController(AgroControlDbContext context)
        {
            _context = context;
        }

        // GET: api/techsteps
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TechSteps>>> GetTechSteps()
        {
            return await _context.TechSteps.ToListAsync();
        }

        // GET: api/techsteps/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TechSteps>> GetTechStep(int id)
        {
            var step = await _context.TechSteps.FindAsync(id);
            if (step == null)
                return NotFound(new { message = $"Шаг с id {id} не найден" });
            return step;
        }

        // GET: api/techsteps/techcard/1 - шаги по техкарте
        [HttpGet("techcard/{techcardId}")]
        public async Task<ActionResult<IEnumerable<TechSteps>>> GetByTechCard(int techcardId)
        {
            var steps = await _context.TechSteps
                .Where(s => s.techcard_id == techcardId)
                .OrderBy(s => s.sort_order)
                .ToListAsync();
            return Ok(steps);
        }

        // POST: api/techsteps
        [HttpPost]
        public async Task<ActionResult<TechSteps>> CreateTechStep(TechSteps step)
        {
            _context.TechSteps.Add(step);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTechStep), new { id = step.step_id }, step);
        }

        // PUT: api/techsteps/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTechStep(int id, TechSteps step)
        {
            if (id != step.step_id)
                return BadRequest(new { message = "ID не совпадают" });

            var existing = await _context.TechSteps.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Шаг с id {id} не найден" });

            existing.step_number = step.step_number;
            existing.step_name = step.step_name;
            existing.equipment_id = step.equipment_id;
            existing.planned_value = step.planned_value;
            existing.tolerance_min = step.tolerance_min;
            existing.tolerance_max = step.tolerance_max;
            existing.instruction = step.instruction;
            existing.sort_order = step.sort_order;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/techsteps/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTechStep(int id)
        {
            var step = await _context.TechSteps.FindAsync(id);
            if (step == null)
                return NotFound(new { message = $"Шаг с id {id} не найден" });

            _context.TechSteps.Remove(step);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Шаг удалён" });
        }
    }
}