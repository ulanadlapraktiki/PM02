using lektion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lektion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RawBatchesController : ControllerBase
    {
        private readonly AgroControlDbContext _context;

        public RawBatchesController(AgroControlDbContext context)
        {
            _context = context;
        }

        // GET: api/rawbatches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RawBatch>>> GetRawBatches()
        {
            return await _context.RawBatches.ToListAsync();
        }

        // GET: api/rawbatches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RawBatch>> GetRawBatch(int id)
        {
            var batch = await _context.RawBatches.FindAsync(id);
            if (batch == null)
                return NotFound(new { message = $"Партия сырья с id {id} не найдена" });
            return batch;
        }

        // GET: api/rawbatches/status/pending
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<RawBatch>>> GetByStatus(string status)
        {
            var batches = await _context.RawBatches
                .Where(b => b.lab_status == status)
                .ToListAsync();
            return Ok(batches);
        }

        // GET: api/rawbatches/material/1
        [HttpGet("material/{materialId}")]
        public async Task<ActionResult<IEnumerable<RawBatch>>> GetByMaterial(int materialId)
        {
            var batches = await _context.RawBatches
                .Where(b => b.material_id == materialId)
                .ToListAsync();
            return Ok(batches);
        }

        // POST: api/rawbatches
        [HttpPost]
        public async Task<ActionResult<RawBatch>> CreateRawBatch(RawBatch batch)
        {
            _context.RawBatches.Add(batch);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRawBatch), new { id = batch.raw_batch_id }, batch);
        }

        // PUT: api/rawbatches/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRawBatch(int id, RawBatch batch)
        {
            if (id != batch.raw_batch_id)
                return BadRequest(new { message = "ID не совпадают" });

            var existing = await _context.RawBatches.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Партия сырья с id {id} не найдена" });

            existing.batch_number = batch.batch_number;
            existing.material_id = batch.material_id;
            existing.supplier = batch.supplier;
            existing.quantity = batch.quantity;
            existing.receipt_date = batch.receipt_date;
            existing.lab_status = batch.lab_status;
            existing.lab_comment = batch.lab_comment;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/rawbatches/5/approve - одобрить партию сырья
        [HttpPatch("{id}/approve")]
        public async Task<IActionResult> ApproveBatch(int id, [FromBody] string? comment)
        {
            var batch = await _context.RawBatches.FindAsync(id);
            if (batch == null)
                return NotFound(new { message = $"Партия сырья с id {id} не найдена" });

            batch.lab_status = "approved";
            batch.lab_comment = comment;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Партия сырья одобрена", batch });
        }

        // PATCH: api/rawbatches/5/block - заблокировать партию сырья
        [HttpPatch("{id}/block")]
        public async Task<IActionResult> BlockBatch(int id, [FromBody] string comment)
        {
            var batch = await _context.RawBatches.FindAsync(id);
            if (batch == null)
                return NotFound(new { message = $"Партия сырья с id {id} не найдена" });

            if (string.IsNullOrEmpty(comment))
                return BadRequest(new { message = "Для блокировки необходимо указать комментарий" });

            batch.lab_status = "blocked";
            batch.lab_comment = comment;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Партия сырья заблокирована", batch });
        }

        // DELETE: api/rawbatches/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRawBatch(int id)
        {
            var batch = await _context.RawBatches.FindAsync(id);
            if (batch == null)
                return NotFound(new { message = $"Партия сырья с id {id} не найдена" });

            _context.RawBatches.Remove(batch);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Партия сырья удалена" });
        }
    }
}