using Jose;
using Kitana.Core.Interfaces;
using Kitana.Service.Model;
using Scaf.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using Amazon.SecurityToken.Model;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using System.Security;
using Microsoft.AspNetCore.Http;
using iText.StyledXmlParser.Css.Validate.Impl.Datatype;

namespace Kitana.Service.Services
{
  public class UserService
    {
      private readonly IDBUserRepository _userRepository;
        private const int SaltSize = 32;
        private readonly ISession _session;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserService(IDBUserRepository userRepository, IHttpContextAccessor accessor)
        {
            httpContextAccessor = accessor;
            _session = httpContextAccessor.HttpContext.Session;
            _userRepository = userRepository;
        }

        public async Task<UserRS> GetUserByIdAsync()
        {
            var userId = Convert.ToInt32(_session.GetString("UserId"));
            var user = await _userRepository.GetUserByIdAsync(userId);
            return MapToUserRS(user);
        }
        public async Task<LoginRQ> UpdatePasswordAsync(string email, string password)
        {
            var user = await _userRepository.UserBasedOnEmail(email);
            var Newpassword = HashPassword(password, user.Salt);
            var updated = await _userRepository.UpdatePasswordAsync(user.UserId ,Newpassword);
            var loginInfo = new LoginRQ
            {
                Email = user.Email,
                Password = password,
            };
            return loginInfo;
        }

        public async Task<string> LoginService(LoginRQ loginReq)
        {
            var user = await _userRepository.LoginRepositoryAsync(loginReq.Email);
            if(user == null)
            {
                throw new ArgumentException("no such user found");
            }
            if (VerifyPassword(loginReq.Password, user.PasswordHash, user.Salt))
            {
                var token = GenerateToken(user);
                return token;
            }
            else
            {
                throw new ArgumentException("Details not correct");
            }
        }

        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("Secret")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>()
            {
                new Claim("userId", user.UserId.ToString()),
                new Claim("email", user.Email),
                new Claim("role", user.Role),
                           };
            string issuer = Environment.GetEnvironmentVariable("ValidIssuer");
            var token = new JwtSecurityToken(
                issuer,
                issuer,
                claims,
                expires: DateTime.Now.AddMinutes(1100),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<UserRS> CreateUserAsync(UserRQ userRQ)
        {
            var user = MapToUser(userRQ);
            var createdUser = await _userRepository.CreateUserAsync(user);
            return MapToUserRS(createdUser);
        }
        public async Task<UserRS> CreateUserInstuctorAsync(UserRQ userRQ)
        {
            var user = MapToUser(userRQ);
            user.Role = RoleTypes.Instructor.ToString();
            var createdUser = await _userRepository.CreateUserAsync(user);
            return MapToUserRS(createdUser);
        }
        public async Task<UserRS> CreateUserAdminAsync(UserRQ userRQ)
        {
            var user = MapToUser(userRQ);
            user.Role = RoleTypes.Admin.ToString();
            var createdUser = await _userRepository.CreateUserAsync(user);
            return MapToUserRS(createdUser);
        }
        public async Task<UserRS> UpdateUserAsync(UserRQ userRQ)
        {
            var userId = Convert.ToInt32(_session.GetString("UserId"));
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return null;
            }
            MapUserRQToUser(userRQ, user);

            var updatedUser = await _userRepository.UpdateUserAsync(userId, user);
            return MapToUserRS(updatedUser);
        }
        public async Task<List<UserCourseRS>> CourseForUserIdServiceAsync()
        {
            var userId = Convert.ToInt32(_session.GetString("UserId"));
            var userCourses = await _userRepository.GetCoursesForUser(userId);
            List<UserCourseRS> courses = new();
            foreach (var userCourse in userCourses)
            {
                var percentage = userCourse.CompletedPercent;
                if(userCourse.CompletedPercent > 100)
                {
                    percentage = 100;
                }
                UserCourseRS rs = new()
                {
                    CourseId = userCourse.CourseId,
                    UserId = userCourse.UserId,
                    CompletedPercent = percentage,
                    AvailableTill = userCourse.AvailableTill,
                };
                courses.Add(rs);
            }
            return courses;
        } 
        // Helper methods for mapping between models
        private UserRS MapToUserRS(User user)
        {
            if (user == null)
            {
                return null;
            }

            return new UserRS
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                City = user.City,
                State = user.State,
                Country = user.Country,
                PostalCode = user.PostalCode,
                UpdatedAt = user.UpdatedAt
            };
        }
        public User MapToUser(UserRQ userRQ)
        {
            byte[] salt = GenerateSalt();
            string hashedPassword = HashPassword(userRQ.PasswordHash, salt);

            return new User
            {
                Username = userRQ.Username,
                Email = userRQ.Email,
                PasswordHash = hashedPassword,
                FullName = userRQ.FullName,
                Salt = salt,
                PhoneNumber = userRQ.PhoneNumber,
                Address = userRQ.Address,
                City = userRQ.City,
                State = userRQ.State,
                Country = userRQ.Country,
                PostalCode = userRQ.PostalCode,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Role = RoleTypes.EndUser.ToString()
            };
        }

        public bool VerifyPassword(string providedPassword, string storedHash, byte[] salt)
        {
            string hashedPassword = HashPassword(providedPassword, salt);
            return storedHash.Equals(hashedPassword);
        }

        private byte[] GenerateSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[SaltSize];
                rng.GetBytes(salt);
                return salt;
            }
        }

        private string HashPassword(string password, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] combinedBytes = new byte[passwordBytes.Length + salt.Length];
                Buffer.BlockCopy(passwordBytes, 0, combinedBytes, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, combinedBytes, passwordBytes.Length, salt.Length);

                byte[] hashedBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private void MapUserRQToUser(UserRQ userRQ, User user)
        {
            user.Username = userRQ.Username;
            user.Email = userRQ.Email;
            user.FullName = userRQ.FullName;
            user.PhoneNumber = userRQ.PhoneNumber;
            user.Address = userRQ.Address;
            user.City = userRQ.City;
            user.State = userRQ.State;
            user.Country = userRQ.Country;
            user.PostalCode = userRQ.PostalCode;
            user.UpdatedAt = DateTime.Now;
        }
    }
}
