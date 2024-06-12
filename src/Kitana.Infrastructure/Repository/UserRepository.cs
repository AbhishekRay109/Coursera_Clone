using Kitana.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Scaf.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kitana.Infrastructure.Repository
{
    public class UserRepository : IDBUserRepository
    {
        private readonly SkillForgeDBContext _context;

        public UserRepository(SkillForgeDBContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user;
        }


        public async Task<User> CreateUserAsync(User user)
        {
            // Check if the email is already taken
            var existingEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingEmail != null)
            {
                throw new ArgumentException("Email address is already in use.");
            }
            // Check if the username is already taken
            var existingUsername = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
            if (existingUsername != null)
            {
                throw new ArgumentException("Username is already in use.");
            }
            // If email and username are unique, proceed to add the user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }


        public async Task<User> UpdateUserAsync(int userId, User user)
        {
            var existingEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.UserId != userId);
            if (existingEmail != null)
            {
                throw new ArgumentException("Email address is already in use.");
            }
            // Check if the username is already taken
            var existingUsername = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username && u.UserId != userId);
            if (existingUsername != null)
            {
                throw new ArgumentException("Username is already in use.");
            }
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (existingUser == null)
            {
                return null;
            }
            existingUser.Email = user.Email;
            existingUser.PasswordHash = user.PasswordHash;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.City = user.City;
            existingUser.Country = user.Country;
            existingUser.PostalCode = user.PostalCode;
            existingUser.Address = user.Address;
            existingUser.State = user.State;
            existingUser.UpdatedAt = DateTime.Now;
            _context.Update(existingUser);
            await _context.SaveChangesAsync();
            return existingUser;
        }
        public async Task<User> UpdatePasswordAsync(int userId, string password)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (existingUser == null)
            {
                return null;
            }
            existingUser.PasswordHash = password;
            _context.Update(existingUser);
            await _context.SaveChangesAsync();
            return existingUser;
        }
        public async Task<User> LoginRepositoryAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                throw new ArgumentException("Details not correct");
            }
            return user;
        }
        public async Task<User> UserBasedOnEmail(string email)
        {
            var users = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            return users;
        }
        public async Task<List<UserCourse>> GetCoursesForUser(int userId)
        {
            var userCourse = await _context.UserCourses.Where(x => x.UserId == userId).ToListAsync();
            return userCourse;
        } 
    }
}
