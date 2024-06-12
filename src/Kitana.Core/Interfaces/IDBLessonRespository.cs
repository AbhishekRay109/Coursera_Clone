using Scaf.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kitana.Core.Interfaces
{
    public interface IDBLessonRepository
    {
        Task<List<Lesson>> GetLessonsByCourseIdAsync(int courseId);
        Task<Lesson> GetLessonByIdAsync(int lessonId);
        Task<Lesson> CreateLessonAsync(Lesson lesson);
        Task<Lesson> UpdateLessonAsync(Lesson lesson);
        Task<bool> DeleteLessonAsync(int lessonId);
        public Task<List<int>> ListOfLessonsInCourse(int courseId);
        public Task<UserCourse> UpdateUserCourse(int courseId, int userId, int percentage);
        public Task<Thumbnail> PostThumbnailAsync(Thumbnail thumbnail);
        public Task<List<Thumbnail>> GetThumbnailAsync(int lessonId);
        public Task<Thumbnail> ActivateThumbnailAsync(int thumbnailId);
        public Task<UserCourse> GetUserCourseWithUserIdAndCourseId(int userId, int courseId);
        public Task<byte[]> GetThumbnailBasedOnLessonId(int lessonId);

    }
}
