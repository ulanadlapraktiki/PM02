using lektion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lektion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeComponentsController : ControllerBase
    {
        private readonly AgroControlDbContext _context;

        public RecipeComponentsController(AgroControlDbContext context)
        {
            _context = context;
        }

        // GET: api/recipecomponents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeComponent>>> GetRecipeComponents()
        {
            return await _context.RecipeComponents.ToListAsync();
        }

        // GET: api/recipecomponents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeComponent>> GetRecipeComponent(int id)
        {
            var component = await _context.RecipeComponents.FindAsync(id);
            if (component == null)
                return NotFound(new { message = $"Компонент с id {id} не найден" });
            return component;
        }

        // GET: api/recipecomponents/recipe/1
        [HttpGet("recipe/{recipeId}")]
        public async Task<ActionResult<IEnumerable<RecipeComponent>>> GetByRecipe(int recipeId)
        {
            var components = await _context.RecipeComponents
                .Where(c => c.recipe_id == recipeId)
                .OrderBy(c => c.load_order)
                .ToListAsync();
            return Ok(components);
        }

        // POST: api/recipecomponents
        [HttpPost]
        public async Task<ActionResult<RecipeComponent>> CreateRecipeComponent(RecipeComponent component)
        {
            _context.RecipeComponents.Add(component);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRecipeComponent), new { id = component.component_id }, component);
        }

        // PUT: api/recipecomponents/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecipeComponent(int id, RecipeComponent component)
        {
            if (id != component.component_id)
                return BadRequest(new { message = "ID не совпадают" });

            var existing = await _context.RecipeComponents.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Компонент с id {id} не найден" });

            existing.percentage = component.percentage;
            existing.load_order = component.load_order;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/recipecomponents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipeComponent(int id)
        {
            var component = await _context.RecipeComponents.FindAsync(id);
            if (component == null)
                return NotFound(new { message = $"Компонент с id {id} не найден" });

            _context.RecipeComponents.Remove(component);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Компонент удалён" });
        }
    }
}