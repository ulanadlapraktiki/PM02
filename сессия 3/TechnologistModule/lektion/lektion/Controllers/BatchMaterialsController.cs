using lektion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lektion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatchMaterialsController : ControllerBase
    {
        private readonly AgroControlDbContext _context;

        public BatchMaterialsController(AgroControlDbContext context)
        {
            _context = context;
        }

        // GET: api/batchmaterials - получить все записи использования сырья
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BatchMaterial>>> GetBatchMaterials()
        {
            return await _context.BatchMaterials.ToListAsync();
        }

        // GET: api/batchmaterials/{id} - получить запись по id
        [HttpGet("{id}")]
        public async Task<ActionResult<BatchMaterial>> GetBatchMaterial(int id)
        {
            var batchMaterial = await _context.BatchMaterials.FindAsync(id);

            if (batchMaterial == null)
            {
                return NotFound(new { message = $"Запись с id {id} не найдена" });
            }

            return batchMaterial;
        }

        // GET: api/batchmaterials/batch/{batchId} - получить все записи по партии
        [HttpGet("batch/{batchId}")]
        public async Task<ActionResult<IEnumerable<BatchMaterial>>> GetByBatchId(int batchId)
        {
            var materials = await _context.BatchMaterials
                .Where(bm => bm.batch_id == batchId)
                .ToListAsync();

            if (materials == null || materials.Count == 0)
            {
                return NotFound(new { message = $"Записи для партии {batchId} не найдены" });
            }

            return Ok(materials);
        }

        // GET: api/batchmaterials/rawbatch/{rawBatchId} - получить все записи по партии сырья
        [HttpGet("rawbatch/{rawBatchId}")]
        public async Task<ActionResult<IEnumerable<BatchMaterial>>> GetByRawBatchId(int rawBatchId)
        {
            var materials = await _context.BatchMaterials
                .Where(bm => bm.raw_batch_id == rawBatchId)
                .ToListAsync();

            if (materials == null || materials.Count == 0)
            {
                return NotFound(new { message = $"Записи для партии сырья {rawBatchId} не найдены" });
            }

            return Ok(materials);
        }

        // GET: api/batchmaterials/batch/{batchId}/total - получить общее количество сырья по партии
        [HttpGet("batch/{batchId}/total")]
        public async Task<ActionResult<object>> GetTotalQuantityByBatch(int batchId)
        {
            var total = await _context.BatchMaterials
                .Where(bm => bm.batch_id == batchId)
                .SumAsync(bm => bm.quantity_used);

            return Ok(new { batch_id = batchId, total_quantity_used = total });
        }

        // POST: api/batchmaterials - создать запись
        [HttpPost]
        public async Task<ActionResult<BatchMaterial>> CreateBatchMaterial(BatchMaterial batchMaterial)
        {
            // Проверяем, существует ли партия
            var batchExists = await _context.ProductionBatches.AnyAsync(pb => pb.batch_id == batchMaterial.batch_id);
            if (!batchExists)
            {
                return BadRequest(new { message = $"Партия с id {batchMaterial.batch_id} не существует" });
            }

            // Проверяем, существует ли партия сырья
            var rawBatchExists = await _context.RawBatches.AnyAsync(rb => rb.raw_batch_id == batchMaterial.raw_batch_id);
            if (!rawBatchExists)
            {
                return BadRequest(new { message = $"Партия сырья с id {batchMaterial.raw_batch_id} не существует" });
            }

            _context.BatchMaterials.Add(batchMaterial);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBatchMaterial), new { id = batchMaterial.usage_id }, batchMaterial);
        }

        // PUT: api/batchmaterials/{id} - обновить запись
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBatchMaterial(int id, BatchMaterial batchMaterial)
        {
            if (id != batchMaterial.usage_id)
            {
                return BadRequest(new { message = "ID в пути и в теле запроса не совпадают" });
            }

            var existing = await _context.BatchMaterials.FindAsync(id);
            if (existing == null)
            {
                return NotFound(new { message = $"Запись с id {id} не найдена" });
            }

            // Проверяем существование партии
            var batchExists = await _context.ProductionBatches.AnyAsync(pb => pb.batch_id == batchMaterial.batch_id);
            if (!batchExists)
            {
                return BadRequest(new { message = $"Партия с id {batchMaterial.batch_id} не существует" });
            }

            // Проверяем существование партии сырья
            var rawBatchExists = await _context.RawBatches.AnyAsync(rb => rb.raw_batch_id == batchMaterial.raw_batch_id);
            if (!rawBatchExists)
            {
                return BadRequest(new { message = $"Партия сырья с id {batchMaterial.raw_batch_id} не существует" });
            }

            existing.batch_id = batchMaterial.batch_id;
            existing.raw_batch_id = batchMaterial.raw_batch_id;
            existing.quantity_used = batchMaterial.quantity_used;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/batchmaterials/{id} - удалить запись
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBatchMaterial(int id)
        {
            var batchMaterial = await _context.BatchMaterials.FindAsync(id);
            if (batchMaterial == null)
            {
                return NotFound(new { message = $"Запись с id {id} не найдена" });
            }

            _context.BatchMaterials.Remove(batchMaterial);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Запись удалена", deleted = batchMaterial });
        }

        // DELETE: api/batchmaterials/batch/{batchId} - удалить все записи по партии
        [HttpDelete("batch/{batchId}")]
        public async Task<IActionResult> DeleteByBatchId(int batchId)
        {
            var materials = await _context.BatchMaterials
                .Where(bm => bm.batch_id == batchId)
                .ToListAsync();

            if (materials.Count == 0)
            {
                return NotFound(new { message = $"Записи для партии {batchId} не найдены" });
            }

            _context.BatchMaterials.RemoveRange(materials);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Удалено {materials.Count} записей для партии {batchId}" });
        }
    }
}