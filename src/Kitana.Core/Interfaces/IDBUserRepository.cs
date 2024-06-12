using Scaf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitana.Core.Interfaces
{
    public interface IDBUserRepository
    {
        public Task<User> GetUserByIdAsync(int userId);
        public Task<User> CreateUserAsync(User user);
        public Task<User> UpdateUserAsync(int userId, User user);
        public Task<User> LoginRepositoryAsync(string email);
        public Task<User> UserBasedOnEmail(string email);
        public Task<User> UpdatePasswordAsync(int userId, string password);
        public Task<List<UserCourse>> GetCoursesForUser(int userId);

    }
}
