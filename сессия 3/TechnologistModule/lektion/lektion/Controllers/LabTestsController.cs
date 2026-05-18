using lektion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lektion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabTestsController : ControllerBase
    {
        private readonly AgroControlDbContext _context;

        public LabTestsController(AgroControlDbContext context)
        {
            _context = context;
        }

        // GET: api/labtests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LabTest>>> GetLabTests()
        {
            return await _context.LabTests.ToListAsync();
        }

        // GET: api/labtests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LabTest>> GetLabTest(int id)
        {
            var test = await _context.LabTests.FindAsync(id);
            if (test == null)
                return NotFound(new { message = $"Испытание с id {id} не найдено" });
            return test;
        }

        // GET: api/labtests/target/raw/1
        [HttpGet("target/{targetType}/{targetId}")]
        public async Task<ActionResult<IEnumerable<LabTest>>> GetByTarget(string targetType, int targetId)
        {
            var tests = await _context.LabTests
                .Where(t => t.target_type == targetType && t.target_id == targetId)
                .ToListAsync();
            return Ok(tests);
        }

        // GET: api/labtests/status/in_progress
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<LabTest>>> GetByStatus(string status)
        {
            var tests = await _context.LabTests
                .Where(t => t.status == status)
                .ToListAsync();
            return Ok(tests);
        }

        // POST: api/labtests
        [HttpPost]
        public async Task<ActionResult<LabTest>> CreateLabTest(LabTest test)
        {
            _context.LabTests.Add(test);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLabTest), new { id = test.test_id }, test);
        }

        // PUT: api/labtests/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLabTest(int id, LabTest test)
        {
            if (id != test.test_id)
                return BadRequest(new { message = "ID не совпадают" });

            var existing = await _context.LabTests.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Испытание с id {id} не найдено" });

            existing.test_number = test.test_number;
            existing.target_type = test.target_type;
            existing.target_id = test.target_id;
            existing.status = test.status;
            existing.decision = test.decision;
            existing.decision_comment = test.decision_comment;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/labtests/5/complete - завершить испытание
        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> DeleteLabTest(int id)
        {
            var test = await _context.LabTests.FindAsync(id);
            if (test == null)
                return NotFound(new { message = $"Испытание с id {id} не найдено" });

            _context.LabTests.Remove(test);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Испытание удалено" });
        }
    }

    public class CompleteTestRequest
    {
        public string Decision { get; set; } = string.Empty;
        public string? Comment { get; set; }
    }
}