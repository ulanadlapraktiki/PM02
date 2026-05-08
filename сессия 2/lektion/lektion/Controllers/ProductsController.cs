using lektion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lektion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AgroControlDbContext _context;

        public ProductsController(AgroControlDbContext context)
        {
            _context = context;
        }

        // GET: api/products - получить все продукты
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // GET: api/products/5 - получить продукт по id
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(new { message = $"Продукт с id {id} не найден" });
            }

            return product;
        }

        // GET: api/products/code/P-001 - получить продукт по коду
        [HttpGet("code/{code}")]
        public async Task<ActionResult<Product>> GetProductByCode(string code)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.product_code == code);

            if (product == null)
            {
                return NotFound(new { message = $"Продукт с кодом {code} не найден" });
            }

            return product;
        }

        // GET: api/products/active - получить только активные
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Product>>> GetActiveProducts()
        {
            var products = await _context.Products
                .Where(p => p.status == "active")
                .ToListAsync();

            return Ok(products);
        }

        // GET: api/products/type/гербицид - получить по типу
        [HttpGet("type/{type}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByType(string type)
        {
            var products = await _context.Products
                .Where(p => p.product_type == type)
                .ToListAsync();

            if (products.Count == 0)
            {
                return NotFound(new { message = $"Продукты типа '{type}' не найдены" });
            }

            return Ok(products);
        }

        // POST: api/products - создать продукт
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            // Проверяем уникальность кода
            var existing = await _context.Products
                .FirstOrDefaultAsync(p => p.product_code == product.product_code);

            if (existing != null)
            {
                return BadRequest(new { message = $"Продукт с кодом {product.product_code} уже существует" });
            }

            // Статус по умолчанию
            if (string.IsNullOrEmpty(product.status))
            {
                product.status = "active";
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.product_id }, product);
        }

        // PUT: api/products/5 - обновить продукт
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.product_id)
            {
                return BadRequest(new { message = "ID в пути и в теле запроса не совпадают" });
            }

            var existing = await _context.Products.FindAsync(id);
            if (existing == null)
            {
                return NotFound(new { message = $"Продукт с id {id} не найден" });
            }

            existing.product_code = product.product_code;
            existing.product_name = product.product_name;
            existing.product_type = product.product_type;
            existing.status = product.status;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/products/5/archive - архивировать
        [HttpPatch("{id}/archive")]
        public async Task<IActionResult> ArchiveProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = $"Продукт с id {id} не найден" });
            }

            product.status = "archived";
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Продукт {product.product_name} архивирован" });
        }

        // DELETE: api/products/5 - удалить продукт
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = $"Продукт с id {id} не найден" });
            }

            // Проверяем, есть ли связанные рецептуры
            var hasRecipes = await _context.Recipes.AnyAsync(r => r.product_id == id);
            if (hasRecipes)
            {
                return BadRequest(new { message = "Нельзя удалить продукт: есть связанные рецептуры" });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Продукт {product.product_name} удалён" });
        }
    }
}