using lektion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lektion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductionBatchesController : ControllerBase
    {
        private readonly AgroControlDbContext _context;

        public ProductionBatchesController(AgroControlDbContext context)
        {
            _context = context;
        }

        // GET: api/productionbatches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductionBatch>>> GetBatches()
        {
            return await _context.ProductionBatches.ToListAsync();
        }

        // GET: api/productionbatches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductionBatch>> GetBatch(int id)
        {
            var batch = await _context.ProductionBatches.FindAsync(id);
            if (batch == null)
                return NotFound(new { message = $"Партия с id {id} не найдена" });
            return batch;
        }

        // GET: api/productionbatches/status/in_progress
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<ProductionBatch>>> GetByStatus(string status)
        {
            var batches = await _context.ProductionBatches
                .Where(b => b.status == status)
                .ToListAsync();
            return Ok(batches);
        }

        // GET: api/productionbatches/product/1
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<ProductionBatch>>> GetByProduct(int productId)
        {
            var batches = await _context.ProductionBatches
                .Where(b => b.product_id == productId)
                .ToListAsync();
            return Ok(batches);
        }

        // POST: api/productionbatches
        [HttpPost]
        public async Task<ActionResult<ProductionBatch>> CreateBatch(ProductionBatch batch)
        {
            _context.ProductionBatches.Add(batch);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBatch), new { id = batch.batch_id }, batch);
        }

        // PUT: api/productionbatches/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBatch(int id, ProductionBatch batch)
        {
            if (id != batch.batch_id)
                return BadRequest(new { message = "ID не совпадают" });

            var existing = await _context.ProductionBatches.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Партия с id {id} не найдена" });

            existing.batch_number = batch.batch_number;
            existing.order_id = batch.order_id;
            existing.product_id = batch.product_id;
            existing.status = batch.status;
            existing.start_time = batch.start_time;
            existing.end_time = batch.end_time;
            existing.lab_decision = batch.lab_decision;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/productionbatches/5/start
        [HttpPatch("{id}/start")]
        public async Task<IActionResult> StartBatch(int id)
        {
            var batch = await _context.ProductionBatches.FindAsync(id);
            if (batch == null)
                return NotFound(new { message = $"Партия с id {id} не найдена" });

            batch.status = "in_progress";
            batch.start_time = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Партия запущена", batch });
        }

        // PATCH: api/productionbatches/5/complete
        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> CompleteBatch(int id)
        {
            var batch = await _context.ProductionBatches.FindAsync(id);
            if (batch == null)
                return NotFound(new { message = $"Партия с id {id} не найдена" });

            batch.status = "completed";
            batch.end_time = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Партия завершена", batch });
        }

        // DELETE: api/productionbatches/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBatch(int id)
        {
            var batch = await _context.ProductionBatches.FindAsync(id);
            if (batch == null)
                return NotFound(new { message = $"Партия с id {id} не найдена" });

            _context.ProductionBatches.Remove(batch);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Партия удалена" });
        }
    }
}