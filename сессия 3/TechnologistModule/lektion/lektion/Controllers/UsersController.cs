using lektion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lektion.Models;

namespace lektion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AgroControlDbContext _context;

        public UsersController(AgroControlDbContext context)
        {
            _context = context;
        }

        // GET: api/users - получить всех пользователей
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/users/5 - получить пользователя по id
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = $"Пользователь с id {id} не найден" });
            }

            return user;
        }

        // GET: api/users/username/tech.ivanov - получить пользователя по логину
        [HttpGet("username/{username}")]
        public async Task<ActionResult<User>> GetUserByUsername(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.username == username);

            if (user == null)
            {
                return NotFound(new { message = $"Пользователь с логином {username} не найден" });
            }

            return user;
        }

        // POST: api/users - создать нового пользователя
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            // Проверяем, не существует ли пользователь с таким логином
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.username == user.username);
            if (existingUser != null)
            {
                return BadRequest(new { message = $"Пользователь с логином {user.username} уже существует" });
            }

            // Устанавливаем дату создания
            user.created_at = DateTime.Now;
            user.is_active = true;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.user_id }, user);
        }

        // PUT: api/users/5 - обновить пользователя
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.user_id)
            {
                return BadRequest(new { message = "ID в пути и в теле запроса не совпадают" });
            }

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound(new { message = $"Пользователь с id {id} не найден" });
            }

            // Обновляем поля
            existingUser.username = user.username;
            existingUser.full_name = user.full_name;
            existingUser.role = user.role;
            existingUser.is_active = user.is_active;
            existingUser.department = user.department;

            _context.Entry(existingUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/users/5 - удалить пользователя (мягкое удаление)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = $"Пользователь с id {id} не найден" });
            }

            // Мягкое удаление - просто меняем статус
            user.is_active = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Пользователь {user.username} деактивирован", user });
        }
    }
}