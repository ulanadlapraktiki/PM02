using lektion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lektion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RawMaterialsController : ControllerBase
    {
        private readonly AgroControlDbContext _context;

        public RawMaterialsController(AgroControlDbContext context)
        {
            _context = context;
        }

        // GET: api/rawmaterials
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RawMaterials>>> GetRawMaterials()
        {
            return await _context.RawMaterials.ToListAsync();
        }

        // GET: api/rawmaterials/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RawMaterials>> GetRawMaterial(int id)
        {
            var material = await _context.RawMaterials.FindAsync(id);
            if (material == null)
                return NotFound(new { message = $"Сырьё с id {id} не найдено" });
            return material;
        }

        // GET: api/rawmaterials/code/RM-001
        [HttpGet("code/{code}")]
        public async Task<ActionResult<RawMaterials>> GetByCode(string code)
        {
            var material = await _context.RawMaterials
                .FirstOrDefaultAsync(m => m.material_code == code);
            if (material == null)
                return NotFound(new { message = $"Сырьё с кодом {code} не найдено" });
            return material;
        }

        // POST: api/rawmaterials
        [HttpPost]
        public async Task<ActionResult<RawMaterials>> CreateRawMaterial(RawMaterials material)
        {
            var existing = await _context.RawMaterials
                .FirstOrDefaultAsync(m => m.material_code == material.material_code);
            if (existing != null)
                return BadRequest(new { message = $"Сырьё с кодом {material.material_code} уже существует" });

            _context.RawMaterials.Add(material);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRawMaterial), new { id = material.material_id }, material);
        }

        // PUT: api/rawmaterials/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRawMaterial(int id, RawMaterials material)
        {
            if (id != material.material_id)
                return BadRequest(new { message = "ID не совпадают" });

            var existing = await _context.RawMaterials.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Сырьё с id {id} не найдено" });

            existing.material_code = material.material_code;
            existing.material_name = material.material_name;
            existing.unit = material.unit;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/rawmaterials/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRawMaterial(int id)
        {
            var material = await _context.RawMaterials.FindAsync(id);
            if (material == null)
                return NotFound(new { message = $"Сырьё с id {id} не найдено" });

            _context.RawMaterials.Remove(material);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Сырьё удалено" });
        }
    }
}