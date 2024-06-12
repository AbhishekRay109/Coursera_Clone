using Scaf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitana.Core.Interfaces
{
    public interface IDBCourseRepository
    {
        public Task<IEnumerable<Course>> GetCoursesAsync();
        public Task<Course> GetCourseByIdAsync(int courseId);
        public Task<Course> CreateCourseAsync(Course course);
        public Task<Course> UpdateCourseAsync(Course course);
        public Task<bool> DeleteCourseAsync(int courseId);
        public Task<UserCourse> GetUserCourseRepoAsync(int courseId, int userId);
        public Task<Certificate> PostCertificateAsync(Certificate certi);
        public Task<Certificate> GetCertificateAsync(int userId, int courseId);
    }
}
