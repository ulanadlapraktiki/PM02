using lektion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lektion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TechCardsController : ControllerBase
    {
        private readonly AgroControlDbContext _context;

        public TechCardsController(AgroControlDbContext context)
        {
            _context = context;
        }

        // GET: api/techcards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TechCards>>> GetTechCards()
        {
            return await _context.TechCards.ToListAsync();
        }

        // GET: api/techcards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TechCards>> GetTechCard(int id)
        {
            var card = await _context.TechCards.FindAsync(id);
            if (card == null)
                return NotFound(new { message = $"Техкарта с id {id} не найдена" });
            return card;
        }

        // GET: api/techcards/product/1
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<TechCards>>> GetByProduct(int productId)
        {
            var cards = await _context.TechCards
                .Where(t => t.product_id == productId)
                .ToListAsync();
            return Ok(cards);
        }

        // GET: api/techcards/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<TechCards>>> GetActiveTechCards()
        {
            var cards = await _context.TechCards
                .Where(t => t.is_active == true)
                .ToListAsync();
            return Ok(cards);
        }

        // POST: api/techcards
        [HttpPost]
        public async Task<ActionResult<TechCards>> CreateTechCard(TechCards card)
        {
            _context.TechCards.Add(card);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTechCard), new { id = card.techcard_id }, card);
        }

        // PUT: api/techcards/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTechCard(int id, TechCards card)
        {
            if (id != card.techcard_id)
                return BadRequest(new { message = "ID не совпадают" });

            var existing = await _context.TechCards.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Техкарта с id {id} не найдена" });

            existing.product_id = card.product_id;
            existing.version = card.version;
            existing.status = card.status;
            existing.is_active = card.is_active;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/techcards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTechCard(int id)
        {
            var card = await _context.TechCards.FindAsync(id);
            if (card == null)
                return NotFound(new { message = $"Техкарта с id {id} не найдена" });

            _context.TechCards.Remove(card);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Техкарта удалена" });
        }
    }
}