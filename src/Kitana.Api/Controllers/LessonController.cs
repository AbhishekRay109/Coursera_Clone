using iText.IO.Image;
using Kitana.Api.Middleware;
using Kitana.Service.Model;
using Kitana.Service.Model.ResponseHandlers;
using Kitana.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Scaf.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Kitana.Api.Controllers
{
    /// <summary>
    /// API controller for managing lessons within courses.
    /// </summary>
    [Route("courses")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly LessonService _lessonService;

        /// <summary>
        /// Constructor for LessonsController.
        /// </summary>
        /// <param name="lessonService">The injected LessonService instance.</param>
        public LessonsController(LessonService lessonService)
        {
            _lessonService = lessonService;
        }

        /// <summary>
        /// Retrieves lessons for a specified course.
        /// </summary>
        /// <param name="courseid">The ID of the course.</param>
        /// <returns>An action result containing a collection of LessonRS objects.</returns>
        [Permissions("EndUser, Instructor, Admin")]
        [HttpGet("{courseid}/lessons")]
        public async Task<ActionResult<IEnumerable<LessonRS>>> GetLessons(int courseid)
        {
            var lessons = await _lessonService.GetLessonsByCourseIdAsync(courseid);
            return Ok(ResponseHandler.HandleSuccess("Successfully fetched lessons", lessons));
        }

        /// <summary>
        /// Retrieves a specific lesson by ID.
        /// </summary>
        /// <param name="lessonid">The ID of the lesson.</param>
        /// <returns>An action result containing a LessonRS object.</returns>
        [Permissions("EndUser, Instructor, Admin")]
        [HttpGet("lesson/{lessonid}")]
        public async Task<ActionResult<LessonRS>> GetLesson(int lessonid)
        {
            var lesson = await _lessonService.GetLessonByIdAsync(lessonid);
            if (lesson == null)
            {
                return NotFound(ResponseHandler.HandleError<string>("NotFound", "Couldn't fetch"));
            }
            return Ok(ResponseHandler.HandleSuccess("Successfully fetched lesson by lessonId", lesson));
        }

        /// <summary>
        /// Creates a new lesson within a course.
        /// </summary>
        /// <param name="courseid">The ID of the course.</param>
        /// <param name="lessonRQ">The LessonRQ object containing lesson details.</param>
        /// <returns>An action result containing the created LessonRS object.</returns>
        [Permissions("Instructor")]
        [HttpPost("lesson/{courseid}")]
        public async Task<ActionResult<LessonRS>> CreateLesson(int courseid, [FromForm] LessonRQ lessonRQ)
        {
            var createdLesson = await _lessonService.CreateLessonAsync(courseid, lessonRQ);
            return Ok(ResponseHandler.HandleSuccess("Successfully inserted the lesson", createdLesson));
        }

        /// <summary>
        /// Updates an existing lesson.
        /// </summary>
        /// <param name="lessonid">The ID of the lesson to update.</param>
        /// <param name="lessonRQ">The updated LessonRQ object.</param>
        /// <returns>An action result representing the updated lesson.</returns>
        [Permissions("Instructor")]
        [HttpPut("lesson/{lessonid}")]
        public async Task<IActionResult> UpdateLesson(int lessonid, [FromForm] LessonRQ lessonRQ)
        {
            var updatedLesson = await _lessonService.UpdateLessonAsync(lessonid, lessonRQ);
            if (updatedLesson == null)
            {
                return NotFound(ResponseHandler.HandleError<string>("NotFound", "Couldn't update"));
            }

            return Ok(ResponseHandler.HandleSuccess("Successfully updated the lesson", updatedLesson));
        }

        /// <summary>
        /// Deletes a lesson by ID.
        /// </summary>
        /// <param name="lessonid">The ID of the lesson to delete.</param>
        /// <returns>An action result indicating the outcome of the delete operation.</returns>
        [Permissions("Instructor")]
        [HttpDelete("lesson/{lessonid}")]
        public async Task<IActionResult> DeleteLesson(int lessonid)
        {
            var result = await _lessonService.DeleteLessonAsync(lessonid);
            if (!result)
            {
                return NotFound(ResponseHandler.HandleError<string>("NotFound", "Couldn't delete"));
            }
            return Ok(ResponseHandler.HandleSuccess<Lesson>("Successfully deleted the lesson"));
        }

        /// <summary>
        /// Marks lessons as completed.
        /// </summary>
        /// <param name="lessonid">The IDs of the lessons to mark as completed.</param>
        /// <returns>An action result indicating the completion status.</returns>
        [Permissions("EndUser, Admin")]
        [HttpPut("lesson/completed")]
        public async Task<IActionResult> CompletedLesson([Required][FromForm] List<int> lessonid)
        {
            try
            {
                var lessonDone = await _lessonService.CompletedLessonService(lessonid);
                return Ok(ResponseHandler.HandleSuccess("Successfully marked the lesson as completed", lessonDone));

            }
            catch (ArgumentException ex)
            {
                return NotFound(ResponseHandler.HandleError<string>("NotFound", $"{ex.Message}"));

            }
        }

        /// <summary>
        /// Adds a thumbnail for a lesson.
        /// </summary>
        /// <param name="thumbnail">The ThumbnailRQ object containing thumbnail details.</param>
        /// <returns>An action result representing the outcome of the operation.</returns>
        [Permissions("Instructor")]
        [HttpPost("lesson/thumbnail")]
        public async Task<IActionResult> AddThumbnailForLesson([Required][FromForm]ThumbnailRQ thumbnail)
        {
            var output = await _lessonService.PostThumbnailService(thumbnail);
            return File(output.ThumbnailImage, "image/jpeg");
        }

        /// <summary>
        /// Retrieves the thumbnail for a lesson by ID.
        /// </summary>
        /// <param name="lessonid">The ID of the lesson.</param>
        /// <returns>An action result containing the retrieved thumbnail.</returns>
        [Permissions("EndUser,Instructor")]
        [HttpGet("thumbnail/{lessonid}")]
        public async Task<IActionResult> GetThumbnailForLesson(int lessonid)
        {
            var output = await _lessonService.GetThumbnailService(lessonid);
            return Ok(ResponseHandler.HandleSuccess("Successfully fetched thumbnail", output));
        }

        /// <summary>
        /// Activates a thumbnail for a lesson.
        /// </summary>
        /// <param name="thumbnailid">The ID of the thumbnail to activate.</param>
        /// <returns>An action result representing the outcome of the activation.</returns>
        [Permissions("Instructor")]
        [HttpPut("thumbnail/{thumbnailid}")]
        public async Task<IActionResult> ActivateThumbnailForLesson(int thumbnailid)
        {
            var output = await _lessonService.ActivateThumbnailService(thumbnailid);
            return File(output.ThumbnailImage, "image/jpeg");
        }
    }
}
