using System.Collections.Generic;
using lektion.Models;

namespace lektion.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAll();
        User GetById(int id);
        User GetByUsername(string username);
        void Create(User user);
        void Update(User user);
        User Delete(int id);
        bool SaveChanges();
    }
}
