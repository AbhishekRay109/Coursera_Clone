using Kitana.Api.Middleware;
using Kitana.Service.Model;
using Kitana.Service.Model.ResponseHandlers;
using Kitana.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Scaf.Models;

namespace Kitana.Api.Controllers
{
    /// <summary>
    /// Controller for managing course reviews.
    /// </summary>
    [Route("review")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewService _reviewService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReviewsController"/> class.
        /// </summary>
        /// <param name="reviewService">The service responsible for review operations.</param>
        public ReviewsController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// Retrieves reviews for a specific course.
        /// </summary>
        /// <param name="courseid">The ID of the course to retrieve reviews for.</param>
        /// <returns>An action result containing the list of course reviews.</returns>
        [Permissions("Instructor, Admin, EndUser")]
        [HttpGet("{courseid}")]
        public async Task<ActionResult<IEnumerable<ReviewRS>>> GetCourseReviews(int courseid)
        {
            var reviews = await _reviewService.GetCourseReviewsAsync(courseid);
            return Ok(ResponseHandler.HandleSuccess("Successfully fetched review", reviews));
        }

        /// <summary>
        /// Adds a new review for a specific course.
        /// </summary>
        /// <param name="courseid">The ID of the course to add the review to.</param>
        /// <param name="review">The review details to be added.</param>
        /// <returns>An action result indicating the result of the operation.</returns>
        [Permissions("EndUser")]
        [HttpPost("{courseid}")]
        public async Task<IActionResult> AddReview(int courseid, ReviewRQ review)
        {
            var result = await _reviewService.AddReviewAsync(courseid, review);
            if (result!=null)
            {
                return Ok(ResponseHandler.HandleSuccess<Review>("Successfully added the review"));
            }
            return NotFound(ResponseHandler.HandleError<string>("NotFound", "Course not found"));

        }

        /// <summary>
        /// Updates an existing review.
        /// </summary>
        /// <param name="reviewid">The ID of the review to update.</param>
        /// <param name="review">The updated review details.</param>
        /// <returns>An action result indicating the result of the operation.</returns>
        [Permissions("EndUser")]
        [HttpPut("{reviewid}")]
        public async Task<IActionResult> UpdateReview(int reviewid, ReviewRQ review)
        {
            var result = await _reviewService.UpdateReviewAsync(reviewid, review);
            if (result != null)
            {
                return Ok(ResponseHandler.HandleSuccess<Review>("Successfully updated the review"));
            }
            return NotFound(ResponseHandler.HandleError<string>("NotFound", "Review not found"));

        }

        /// <summary>
        /// Deletes an existing review.
        /// </summary>
        /// <param name="reviewid">The ID of the review to delete.</param>
        /// <returns>An action result indicating the result of the operation.</returns>
        [Permissions("Admin, EndUser")]
        [HttpDelete("{reviewid}")]
        public async Task<IActionResult> DeleteReview(int reviewid)
        {
            var result = await _reviewService.DeleteReviewAsync(reviewid);
            if (result)
            {
                return Ok(ResponseHandler.HandleSuccess<Review>("Successfully deleted a review"));
            }
            return NotFound(ResponseHandler.HandleError<string>("NotFound", "Review not found"));

        }
    }
}