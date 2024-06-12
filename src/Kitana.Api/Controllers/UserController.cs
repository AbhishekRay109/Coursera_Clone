using Kitana.Api.Middleware;
using Kitana.Service.Model;
using Kitana.Service.Model.ResponseHandlers;
using Kitana.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scaf.Models;
using System.Threading.Tasks;

namespace Kitana.Api.Controllers
{
    /// <summary>
    /// Controller for managing users.
    /// </summary>
    [Route("user")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="userService">The service responsible for user operations.</param>
        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Retrieves a user by user ID.
        /// </summary>
        /// <param>The ID of the user to retrieve.</param>
        /// <returns>An action result containing the user details.</returns>
        [Permissions("Instructor, Admin, EndUser")]
        [HttpGet()]
        public async Task<ActionResult<UserRS>> GetUser()
        {
            var user = await _userService.GetUserByIdAsync();
            if (user == null)
            {
                return NotFound(ResponseHandler.HandleError<string>("NotFound", "Couldn't fetch"));
            }
            return Ok(ResponseHandler.HandleSuccess("Successfully fetched the user", user));
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user">The details of the user to create.</param>
        /// <returns>An action result containing the created user.</returns>
        [HttpPost]
        public async Task<ActionResult<UserRS>> CreateUser(UserRQ user)
        {
            var createdUser = await _userService.CreateUserAsync(user);
            return Ok(ResponseHandler.HandleSuccess("Successfully added a user", createdUser));
        }

        /// <summary>
        /// Creates a new instructor user.
        /// </summary>
        /// <param name="user">The details of the instructor to create.</param>
        /// <returns>An action result containing the created instructor.</returns>
        [HttpPost("instructor")]
        public async Task<ActionResult<UserRS>> CreateInstructor(UserRQ user)
        {
            var createdUser = await _userService.CreateUserInstuctorAsync(user);
            return Ok(ResponseHandler.HandleSuccess("Successfully added an instructor", createdUser));
        }

        /// <summary>
        /// Creates a new admin user.
        /// </summary>
        /// <param name="user">The details of the admin to create.</param>
        /// <returns>An action result containing the created admin.</returns>
        [HttpPost("admin")]
        public async Task<ActionResult<UserRS>> CreateAdmin(UserRQ user)
        {
            var createdUser = await _userService.CreateUserAdminAsync(user);
            return Ok(ResponseHandler.HandleSuccess("Successfully added an admin", createdUser));
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="user">The updated user details.</param>
        /// <returns>An action result indicating the result of the operation.</returns>
        [Permissions("Admin, Instructor, EndUser")]
        [HttpPut("{userid}")]
        public async Task<IActionResult> UpdateUser(UserRQ user)
        {
            var updatedUser = await _userService.UpdateUserAsync(user);
            if (updatedUser == null)
            {
                return NotFound(ResponseHandler.HandleError<string>("NotFound", "Couldn't update"));
            }
            return Ok(ResponseHandler.HandleSuccess("Successfully updated an user", updatedUser));
        }

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="login">The login credentials.</param>
        /// <returns>An action result containing a generated token if successful.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<string>> LoginUser(LoginRQ login)
        {
            var generatedToken = await _userService.LoginService(login);
            if (generatedToken == null)
            {
                return NotFound(ResponseHandler.HandleError<string>("NotFound", "Couldn't login"));
            }
            return Ok(generatedToken);
        }

        /// <summary>
        /// Resets a user's password.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="password">The new password to set.</param>
        /// <returns>An action result indicating the result of the operation.</returns>
        //[Permissions("Admin")]
        [HttpPut("setPass")]
        public async Task<IActionResult> ResetPassword(string email, string password)
        {
            var updatePassword = await _userService.UpdatePasswordAsync(email, password);
            return Ok(ResponseHandler.HandleSuccess("Successfully updated the password", updatePassword));
        }
        /// <summary>
        /// Retrieves the list of courses for the current user.
        /// </summary>
        /// <returns>
        /// Containing the list of courses associated with the current user.
        /// Returns a 200 status code with a success message and the list of courses.
        /// </returns>
        /// <response code="200">Successfully fetched the courses.</response>
        [Permissions("EndUser")]
        [HttpGet("course")]
        public async Task<IActionResult> GetCoursesForUser()
        {
            var userCourse = await _userService.CourseForUserIdServiceAsync();
            return Ok(ResponseHandler.HandleSuccess("Successfully fecthed the courses", userCourse));
        }
    }
}
