using lektion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lektion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        private readonly AgroControlDbContext _context;

        public EquipmentController(AgroControlDbContext context)
        {
            _context = context;
        }

        // GET: api/equipment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Equipment>>> GetEquipment()
        {
            return await _context.Equipment.ToListAsync();
        }

        // GET: api/equipment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Equipment>> GetEquipmentItem(int id)
        {
            var item = await _context.Equipment.FindAsync(id);
            if (item == null)
                return NotFound(new { message = $"Оборудование с id {id} не найдено" });
            return item;
        }

        // GET: api/equipment/line/Линия 1
        [HttpGet("line/{line}")]
        public async Task<ActionResult<IEnumerable<Equipment>>> GetByLine(string line)
        {
            var items = await _context.Equipment
                .Where(e => e.line == line)
                .ToListAsync();
            return Ok(items);
        }

        // POST: api/equipment
        [HttpPost]
        public async Task<ActionResult<Equipment>> CreateEquipment(Equipment equipment)
        {
            _context.Equipment.Add(equipment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEquipmentItem), new { id = equipment.equipment_id }, equipment);
        }

        // PUT: api/equipment/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEquipment(int id, Equipment equipment)
        {
            if (id != equipment.equipment_id)
                return BadRequest(new { message = "ID не совпадают" });

            var existing = await _context.Equipment.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Оборудование с id {id} не найдено" });

            existing.equipment_name = equipment.equipment_name;
            existing.line = equipment.line;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/equipment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipment(int id)
        {
            var item = await _context.Equipment.FindAsync(id);
            if (item == null)
                return NotFound(new { message = $"Оборудование с id {id} не найдено" });

            _context.Equipment.Remove(item);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Оборудование удалено" });
        }
    }
}