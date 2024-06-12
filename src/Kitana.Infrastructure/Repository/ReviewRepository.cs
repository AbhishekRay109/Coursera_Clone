using Kitana.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Scaf.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kitana.Infrastructure.Repository
{
    public class ReviewRepository : IDBReviewRepository
    {
        private readonly SkillForgeDBContext _context;

        public ReviewRepository(SkillForgeDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetCourseReviewsAsync(int courseId)
        {
            return await _context.Reviews
                .Where(r => r.CourseId == courseId)
                .Include(r => r.User).Include(r => r.Course)
                .ToListAsync();
        }

        public async Task<Review> AddReviewAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            var output = await _context.Reviews.Include(r => r.User).Include(r => r.Course).FirstOrDefaultAsync(x => x.ReviewId == review.ReviewId);
            return output;
        }

        public async Task<Review> UpdateReviewAsync(Review review)
        {
            _context.Entry(review).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            var output = await _context.Reviews.Include(r => r.User).Include(r => r.Course).FirstOrDefaultAsync(x => x.ReviewId == review.ReviewId);
            return output;
        }

        public async Task<bool> DeleteReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
            {
                return false;
            }
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Review> GetReviewByIdAsync(int reviewId)
        {
            return await _context.Reviews.FindAsync(reviewId);
        }
    }
}
