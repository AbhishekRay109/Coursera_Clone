using Scaf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitana.Core.Interfaces
{
    public interface IDBReviewRepository
    {
        public Task<IEnumerable<Review>> GetCourseReviewsAsync(int courseId);
        public Task<Review> AddReviewAsync(Review review);
        public Task<Review> UpdateReviewAsync(Review review);
        public Task<bool> DeleteReviewAsync(int reviewId);
        public Task<Review> GetReviewByIdAsync(int reviewId);
    }
}
