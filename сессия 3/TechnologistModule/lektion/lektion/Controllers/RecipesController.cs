using lektion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lektion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly AgroControlDbContext _context;

        public RecipesController(AgroControlDbContext context)
        {
            _context = context;
        }

        // GET: api/recipes - все рецептуры
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Receipe>>> GetRecipes()
        {
            return await _context.Recipes.ToListAsync();
        }

        // GET: api/recipes/5 - по id
        [HttpGet("{id}")]
        public async Task<ActionResult<Receipe>> GetRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
                return NotFound(new { message = $"Рецептура с id {id} не найдена" });
            return recipe;
        }

        // GET: api/recipes/product/1 - по продукту
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<Receipe>>> GetByProduct(int productId)
        {
            var recipes = await _context.Recipes
                .Where(r => r.product_id == productId)
                .ToListAsync();
            return Ok(recipes);
        }

        // GET: api/recipes/active - активные рецептуры
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Receipe>>> GetActiveRecipes()
        {
            var recipes = await _context.Recipes
                .Where(r => r.is_active == true)
                .ToListAsync();
            return Ok(recipes);
        }

        // POST: api/recipes - создать
        [HttpPost]
        public async Task<ActionResult<Receipe>> CreateRecipe(Receipe recipe)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.recipe_id }, recipe);
        }

        // PUT: api/recipes/5 - обновить
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecipe(int id, Receipe recipe)
        {
            if (id != recipe.recipe_id)
                return BadRequest(new { message = "ID не совпадают" });

            var existing = await _context.Recipes.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Рецептура с id {id} не найдена" });

            existing.product_id = recipe.product_id;
            existing.version = recipe.version;
            existing.status = recipe.status;
            existing.is_active = recipe.is_active;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/recipes/5 - удалить
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
                return NotFound(new { message = $"Рецептура с id {id} не найдена" });

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Рецептура удалена" });
        }
    }
}