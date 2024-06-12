using Kitana.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Scaf.Models;

namespace Kitana.Infrastructure.Repository
{
    public class CourseRepository : IDBCourseRepository
    {
        private readonly SkillForgeDBContext _context;

        public CourseRepository(SkillForgeDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetCoursesAsync()
        {
            var courseDetail = await _context.Courses.Include(us => us.Instructor).ToListAsync();
            return courseDetail;
        }

        public async Task<Course> GetCourseByIdAsync(int courseId)
        {
            var courseDetail = await _context.Courses.Include(us => us.Instructor).FirstOrDefaultAsync(x => x.CourseId == courseId);
            return courseDetail;
        }

        public async Task<Course> CreateCourseAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            var courseDetail = await _context.Courses.Include(us => us.Instructor).FirstOrDefaultAsync(x => x.CourseId == course.CourseId);
            return courseDetail;
        }

        public async Task<Course> UpdateCourseAsync(Course course)
        {
            _context.Entry(course).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            var courseDetail = await _context.Courses.Include(us => us.Instructor).FirstOrDefaultAsync(x => x.CourseId == course.CourseId);
            return courseDetail;
        }

        public async Task<bool> DeleteCourseAsync(int courseId)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(x => x.CourseId == courseId);
            if (course == null)
            {
                return false;
            }
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<UserCourse> GetUserCourseRepoAsync(int courseId, int userId)
        {
            return await _context.UserCourses
                .Include(uc => uc.Course) // Include the Course entity
                    .ThenInclude(c => c.Instructor) // Then include the Instructor navigation property of Course
                .Include(uc => uc.User) // Include the User entity
                .FirstOrDefaultAsync(x => x.CourseId == courseId && x.UserId == userId);
        }
        public async Task<Certificate> GetCertificateAsync(int userId, int courseId)
        {
            var certicate = await _context.Certificates.FirstOrDefaultAsync(x => x.UserId == userId && x.CourseId == courseId);
            return certicate;
        }
        public async Task<Certificate> PostCertificateAsync(Certificate certi)
        {
            _context.Add(certi);
            await _context.SaveChangesAsync();
            return certi;
        }
    }
}
