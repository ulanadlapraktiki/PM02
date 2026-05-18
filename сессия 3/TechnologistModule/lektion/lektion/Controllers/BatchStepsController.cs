using lektion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lektion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatchStepsController : ControllerBase
    {
        private readonly AgroControlDbContext _context;

        public BatchStepsController(AgroControlDbContext context)
        {
            _context = context;
        }

        // GET: api/batchsteps
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BatchStep>>> GetBatchSteps()
        {
            return await _context.BatchSteps.ToListAsync();
        }

        // GET: api/batchsteps/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BatchStep>> GetBatchStep(int id)
        {
            var step = await _context.BatchSteps.FindAsync(id);
            if (step == null)
                return NotFound(new { message = $"Выполнение шага с id {id} не найдено" });
            return step;
        }

        // GET: api/batchsteps/batch/1 - шаги по партии
        [HttpGet("batch/{batchId}")]
        public async Task<ActionResult<IEnumerable<BatchStep>>> GetByBatch(int batchId)
        {
            var steps = await _context.BatchSteps
                .Where(s => s.batch_id == batchId)
                .ToListAsync();
            return Ok(steps);
        }

        // GET: api/batchsteps/batch/1/uncompleted - невыполненные шаги
        [HttpGet("batch/{batchId}/uncompleted")]
        public async Task<ActionResult<IEnumerable<BatchStep>>> GetUncompletedSteps(int batchId)
        {
            var steps = await _context.BatchSteps
                .Where(s => s.batch_id == batchId && s.is_completed == false)
                .ToListAsync();
            return Ok(steps);
        }

        // POST: api/batchsteps
        [HttpPost]
        public async Task<ActionResult<BatchStep>> CreateBatchStep(BatchStep step)
        {
            _context.BatchSteps.Add(step);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBatchStep), new { id = step.execution_id }, step);
        }

        // PUT: api/batchsteps/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBatchStep(int id, BatchStep step)
        {
            if (id != step.execution_id)
                return BadRequest(new { message = "ID не совпадают" });

            var existing = await _context.BatchSteps.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Выполнение шага с id {id} не найдено" });

            existing.actual_value = step.actual_value;
            existing.started_by = step.started_by;
            existing.started_at = step.started_at;
            existing.completed_by = step.completed_by;
            existing.completed_at = step.completed_at;
            existing.is_completed = step.is_completed;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/batchsteps/5/start - начать шаг
        [HttpPatch("{id}/start")]
        public async Task<IActionResult> StartStep(int id, [FromBody] int userId)
        {
            var step = await _context.BatchSteps.FindAsync(id);
            if (step == null)
                return NotFound(new { message = $"Шаг с id {id} не найден" });

            step.started_by = userId;
            step.started_at = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Шаг начат", step });
        }

        // PATCH: api/batchsteps/5/complete - завершить шаг
        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> CompleteStep(int id, [FromBody] int userId)
        {
            var step = await _context.BatchSteps.FindAsync(id);
            if (step == null)
                return NotFound(new { message = $"Шаг с id {id} не найден" });

            step.completed_by = userId;
            step.completed_at = DateTime.Now;
            step.is_completed = true;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Шаг завершён", step });
        }

        // DELETE: api/batchsteps/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBatchStep(int id)
        {
            var step = await _context.BatchSteps.FindAsync(id);
            if (step == null)
                return NotFound(new { message = $"Выполнение шага с id {id} не найдено" });

            _context.BatchSteps.Remove(step);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Запись удалена" });
        }
    }
}