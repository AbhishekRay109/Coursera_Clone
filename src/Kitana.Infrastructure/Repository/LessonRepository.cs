using Kitana.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Scaf.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kitana.Infrastructure.Repository
{
    public class LessonRepository : IDBLessonRepository
    {
        private readonly SkillForgeDBContext _context;

        public LessonRepository(SkillForgeDBContext context)
        {
            _context = context;
        }

        public async Task<List<Lesson>> GetLessonsByCourseIdAsync(int courseId)
        {
            var lessons = await _context.Lessons
                .Where(l => l.CourseId == courseId)
                .ToListAsync();
            return lessons;
        }
        public async Task<UserCourse> GetUserCourseWithUserIdAndCourseId(int userId, int courseId)
        {
            return await _context.UserCourses.FirstOrDefaultAsync(x=>x.UserId == userId && x.CourseId == courseId);
        }
        public async Task<Lesson> GetLessonByIdAsync(int lessonId)
        {
            var lesson = await _context.Lessons.Include(x => x.Course)
                .FirstOrDefaultAsync(l => l.LessonId == lessonId);
            return lesson;
        }

        public async Task<Lesson> CreateLessonAsync(Lesson lesson)
        {
            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();
            return lesson;
        }

        public async Task<Lesson> UpdateLessonAsync(Lesson lesson)
        {
            _context.Entry(lesson).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return lesson;
        }

        public async Task<bool> DeleteLessonAsync(int lessonId)
        {
            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(l => l.LessonId == lessonId);

            if (lesson == null)
            {
                return false;
            }

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<int>> ListOfLessonsInCourse(int courseId)
        {
            var output = await _context.Lessons.Where(x => x.CourseId == courseId).ToListAsync();
            return output.Select(x => x.LessonId).ToList();
        }
        public async Task<UserCourse> UpdateUserCourse(int courseId, int userId, int percentage)
        {
            var userCourse = await _context.UserCourses.FirstOrDefaultAsync(x => x.CourseId == courseId && x.UserId == userId);
            userCourse.CompletedPercent += percentage;
            _context.Entry(userCourse).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return userCourse;
        }
        public async Task<Thumbnail> PostThumbnailAsync(Thumbnail thumbnail)
        {
            _context.Add(thumbnail);
            await _context.SaveChangesAsync();
            return thumbnail;
        }
        public async Task<List<Thumbnail>> GetThumbnailAsync(int lessonId)
        {
            var thumbnail = await _context.Thumbnails.Where(x => x.LessonId == lessonId).ToListAsync();
            return thumbnail;
        }
        public async Task<Thumbnail> ActivateThumbnailAsync(int thumbnailId)
        {
            var thumbnail = await _context.Thumbnails.Where(x => x.ThumbnailId == thumbnailId).FirstOrDefaultAsync();
            thumbnail.Active = true;
            var OldThumbnail = await _context.Thumbnails.Where(x => x.LessonId == thumbnail.LessonId && x.Active).FirstOrDefaultAsync();
            _context.Entry(thumbnail).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            if (OldThumbnail != null)
            {
                OldThumbnail.Active = false;
                _context.Entry(OldThumbnail).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            return thumbnail;
        }
        public async Task<byte[]> GetThumbnailBasedOnLessonId(int lessonId)
        {
            var thumbnail = await _context.Thumbnails.FirstOrDefaultAsync(x => x.LessonId == lessonId && x.Active);
            if (thumbnail == null)
                return null;
            return thumbnail.ThumbnailImage;
        }

    }
}
