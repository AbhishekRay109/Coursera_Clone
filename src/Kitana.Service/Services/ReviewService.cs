using Kitana.Core.Interfaces;
using Kitana.Service.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Scaf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kitana.Service.Services
{
    public class ReviewService
    {
        private readonly IDBReviewRepository _reviewRepository;
        private readonly ISession _session;
        private readonly IHttpContextAccessor httpContextAccessor;
        public ReviewService(IDBReviewRepository reviewRepository, IHttpContextAccessor accessor)
        {
            httpContextAccessor = accessor;
            _session = httpContextAccessor.HttpContext.Session;
            _reviewRepository = reviewRepository;
        }

        public async Task<IEnumerable<ReviewRS>> GetCourseReviewsAsync(int courseId)
        {
            var reviews = await _reviewRepository.GetCourseReviewsAsync(courseId);
            return reviews.Select(MapToReviewRS);
        }

        public async Task<ReviewRS> AddReviewAsync(int courseId, ReviewRQ reviewRQ)
        {
            var userId = Convert.ToInt32(_session.GetString("UserId"));
            var review = MapToReview(reviewRQ);
            review.UserId = userId;
            review.CourseId = courseId;
            var addedReview = await _reviewRepository.AddReviewAsync(review);
            return MapToReviewRS(addedReview);
        }

        public async Task<ReviewRS> UpdateReviewAsync(int reviewId, ReviewRQ reviewRQ)
        {
            var userId = Convert.ToInt32(_session.GetString("UserId"));
            var role = _session.GetString("Role");

            var review = await _reviewRepository.GetReviewByIdAsync(reviewId);
            if (review == null)
            {
                return null;
            }
            if (review.UserId == userId || role == "Admin")
            {
                MapReviewRQToReview(reviewRQ, review);
                return MapToReviewRS(await _reviewRepository.UpdateReviewAsync(review));
            }
            return null;
        }

        public async Task<bool> DeleteReviewAsync(int reviewId)
        {
            return await _reviewRepository.DeleteReviewAsync(reviewId);
        }

        // Helper methods for mapping between models
        private ReviewRS MapToReviewRS(Review review)
        {
            if (review == null)
            {
                return null;
            }

            return new ReviewRS
            {
                ReviewId = review.ReviewId,
                UserName = review.User.FullName,
                CourseTitle = review.Course.Title,
                Rating = review.RatingValue,
                Comment = review.ReviewText,
                UpdatedAt = review.UpdatedAt
            };
        }

        private Review MapToReview(ReviewRQ reviewRQ)
        {
            return new Review
            {
                RatingValue = reviewRQ.Rating,
                ReviewText = reviewRQ.Comment,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }

        private void MapReviewRQToReview(ReviewRQ reviewRQ, Review review)
        {
            review.RatingValue = reviewRQ.Rating;
            review.ReviewText = reviewRQ.Comment;
            review.UpdatedAt = DateTime.Now;
        }
    }
}
