using lektion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lektion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductionOrdersController : ControllerBase
    {
        private readonly AgroControlDbContext _context;

        public ProductionOrdersController(AgroControlDbContext context)
        {
            _context = context;
        }

        // GET: api/productionorders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductionOrder>>> GetOrders()
        {
            return await _context.ProductionOrders.ToListAsync();
        }

        // GET: api/productionorders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductionOrder>> GetOrder(int id)
        {
            var order = await _context.ProductionOrders.FindAsync(id);
            if (order == null)
                return NotFound(new { message = $"Заказ с id {id} не найден" });
            return order;
        }

        // GET: api/productionorders/status/planned
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<ProductionOrder>>> GetByStatus(string status)
        {
            var orders = await _context.ProductionOrders
                .Where(o => o.status == status)
                .ToListAsync();
            return Ok(orders);
        }

        // POST: api/productionorders
        [HttpPost]
        public async Task<ActionResult<ProductionOrder>> CreateOrder(ProductionOrder order)
        {
            _context.ProductionOrders.Add(order);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrder), new { id = order.order_id }, order);
        }

        // PUT: api/productionorders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, ProductionOrder order)
        {
            if (id != order.order_id)
                return BadRequest(new { message = "ID не совпадают" });

            var existing = await _context.ProductionOrders.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Заказ с id {id} не найден" });

            existing.order_number = order.order_number;
            existing.product_id = order.product_id;
            existing.planned_quantity = order.planned_quantity;
            existing.status = order.status;
            existing.recipe_id = order.recipe_id;
            existing.techcard_id = order.techcard_id;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/productionorders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.ProductionOrders.FindAsync(id);
            if (order == null)
                return NotFound(new { message = $"Заказ с id {id} не найден" });

            _context.ProductionOrders.Remove(order);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Заказ удалён" });
        }
    }
}