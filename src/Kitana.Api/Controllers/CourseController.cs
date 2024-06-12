using Kitana.Api.Middleware;
using Kitana.Service.Model;
using Kitana.Service.Model.ResponseHandlers;
using Kitana.Service.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scaf.Models;

namespace Kitana.Api.Controllers
{
    /// <summary>
    /// Controller for managing courses.
    /// </summary>
    [Route("course")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        /// <summary>
        /// Service for handling course-related operations.
        /// </summary>
        public CourseService _courseService;
        private readonly ISession _session;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoursesController"/> class.
        /// </summary>
        /// <param name="courseService">The course service.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public CoursesController(CourseService courseService, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _session = _httpContextAccessor.HttpContext.Session;
            _courseService = courseService;
        }

        /// <summary>
        /// Retrieves all courses.
        /// </summary>
        /// 200 OK - Success.
        /// <response code="400"> Bad Request </response> 
        /// <response code="500"> Internal Server Error </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="403"> Forbidden </response>
        /// <response code="404"> Not Found </response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseRS>>> GetCourses()
        {
            var courses = await _courseService.GetCoursesAsync();
            return Ok(ResponseHandler.HandleSuccess("Successfully fetched courses", courses));
        }

        /// <summary>
        /// Retrieves a specific course by ID.
        /// </summary>
        /// <param name="courseid">The ID of the course to retrieve.</param>
        /// 200 OK - Success.
        /// <response code="400"> Bad Request </response> 
        /// <response code="500"> Internal Server Error </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="403"> Forbidden </response>
        /// <response code="404"> Not Found </response>
        [HttpGet("{courseid}")]
        public async Task<ActionResult<CourseRS>> GetCourse(int courseid)
        {
            var course = await _courseService.GetCourseByIdAsync(courseid);
            if (course == null)
            {
                return NotFound(ResponseHandler.HandleError<string>("NotFound", "Couldn't get courses"));
            }
            return Ok(ResponseHandler.HandleSuccess("Successfully fetched course by courseId", course));
        }

        /// <summary>
        /// Creates a new course.
        /// </summary>
        /// <param name="course">
        /// - title : Set the title of the course.
        /// - description : Set the description of the course. 
        /// - city : Set the city where the course is offered.
        /// - state : Set the state where the course is offered.
        /// - country : Set the country where the course is offered.
        /// - enrollmentStatus : A boolean indicating whether enrollment is open (true) or closed (false).
        /// - activeDays : An integer representing the number of days the course is active.
        /// - price : Set the price of the course.
        /// </param>
        /// 200 OK - Success. 
        /// <response code="400"> Bad Request </response> 
        /// <response code="500"> Internal Server Error </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="403"> Forbidden </response>
        /// <response code="404"> Not Found </response>
        [Permissions("Instructor")]
        [HttpPost]
        public async Task<ActionResult<CourseRS>> CreateCourse(CourseRQ course)
        {
            var createdCourse = await _courseService.CreateCourseAsync(course);
            return Ok(ResponseHandler.HandleSuccess("Successfully created the course", createdCourse));
        }

        /// <summary>
        /// Updates an existing course.
        /// </summary>
        /// <param name="courseid">The ID of the course to update.</param>
        /// <param name="course">The updated course request model.</param>
        /// 200 OK - Success.
        /// <response code="400"> Bad Request </response> 
        /// <response code="500"> Internal Server Error </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="403"> Forbidden </response>
        /// <response code="404"> Not Found </response>
        [Permissions("Instructor")]
        [HttpPut("{courseid}")]
        public async Task<IActionResult> UpdateCourse(int courseid, CourseRQ course)
        {
            var updatedCourse = await _courseService.UpdateCourseAsync(courseid, course);
            if (updatedCourse == null)
            {
                return NotFound(ResponseHandler.HandleError<string>("NotFound", "courseid not present"));
            }
            return Ok(ResponseHandler.HandleSuccess("Successfully updated the course", updatedCourse));
        }

        /// <summary>
        /// Deletes a course.
        /// </summary>
        /// <param name="courseid">The ID of the course to delete.</param>
        /// 200 OK - Success.
        /// <response code="400"> Bad Request </response> 
        /// <response code="500"> Internal Server Error </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="403"> Forbidden </response>
        /// <response code="404"> Not Found </response>
        [Permissions("Admin, Instructor")]
        [HttpDelete("{courseid}")]
        public async Task<IActionResult> DeleteCourse(int courseid)
        {
            var result = await _courseService.DeleteCourseAsync(courseid);
            if (!result)
            {
                return NotFound(ResponseHandler.HandleError<string>("NotFound", "courseid not present"));
            }
            return Ok(ResponseHandler.HandleSuccess<Course>("Successfully deleted"));
        }

        /// <summary>
        /// Retrieves a certificate for a specific course.
        /// </summary>
        /// <param name="courseid">The ID of the course.</param>
        /// 200 OK - Success.
        /// <response code="400"> Bad Request </response> 
        /// <response code="500"> Internal Server Error </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="403"> Forbidden </response>
        /// <response code="404"> Not Found </response>
        [Permissions("EndUser")]
        [HttpGet("/certificate/{courseid}")]
        public async Task<IActionResult> FetchCertificate(int courseid)
        {
            var userId = Convert.ToInt32(_session.GetString("UserId"));
            var certificate = await _courseService.FetchCertificateAsync(courseid);
            return File(certificate, "application/pdf", $"Certificate_{userId}_{courseid}.pdf");
        }
    }
}