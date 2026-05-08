using lektion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lektion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestParametersController : ControllerBase
    {
        private readonly AgroControlDbContext _context;

        public TestParametersController(AgroControlDbContext context)
        {
            _context = context;
        }

        // GET: api/testparameters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TestParameters>>> GetTestParameters()
        {
            return await _context.TestParameters.ToListAsync();
        }

        // GET: api/testparameters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TestParameters>> GetTestParameter(int id)
        {
            var param = await _context.TestParameters.FindAsync(id);
            if (param == null)
                return NotFound(new { message = $"Параметр с id {id} не найден" });
            return param;
        }

        // GET: api/testparameters/test/1 - параметры по испытанию
        [HttpGet("test/{testId}")]
        public async Task<ActionResult<IEnumerable<TestParameters>>> GetByTest(int testId)
        {
            var param = await _context.TestParameters
                .Where(p => p.test_id == testId)
                .ToListAsync();
            return Ok(param);
        }

        // POST: api/testparameters
        [HttpPost]
        public async Task<ActionResult<TestParameters>> CreateTestParameter(TestParameters param)
        {
            _context.TestParameters.Add(param);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTestParameter), new { id = param.param_id }, param);
        }

        // PUT: api/testparameters/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTestParameter(int id, TestParameters param)
        {
            if (id != param.param_id)
                return BadRequest(new { message = "ID не совпадают" });

            var existing = await _context.TestParameters.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Параметр с id {id} не найден" });

            existing.param_name = param.param_name;
            existing.standard_min = param.standard_min;
            existing.standard_max = param.standard_max;
            existing.actual_value = param.actual_value;
            existing.is_ok = param.is_ok;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/testparameters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestParameter(int id)
        {
            var param = await _context.TestParameters.FindAsync(id);
            if (param == null)
                return NotFound(new { message = $"Параметр с id {id} не найден" });

            _context.TestParameters.Remove(param);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Параметр удалён" });
        }
    }
}