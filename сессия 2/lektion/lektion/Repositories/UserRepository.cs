using System;
using System.Collections.Generic;
using System.Linq;
using lektion.Models;

namespace lektion.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AgroControlDbContext _context;

        // Конструктор получает DbContext через dependency injection
        public UserRepository(AgroControlDbContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetAll()
        {
            // Возвращаем всех активных пользователей
            return _context.Users.Where(u => u.is_active).ToList();
        }

        public User GetById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.user_id == id);
        }

        public User GetByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.username == username);
        }

        public void Create(User user)
        {
            // Устанавливаем дату создания, если она не задана
            if (user.created_at == default)
                user.created_at = DateTime.Now;

            _context.Users.Add(user);
            SaveChanges();
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
            SaveChanges();
        }

        public User Delete(int id)
        {
            var user = GetById(id);
            if (user != null)
            {
                // "Мягкое" удаление - меняем статус, а не удаляем запись.
                user.is_active = false;
                _context.Users.Update(user);
                SaveChanges();
            }
            return user;
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}