using Kitana.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Scaf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.IO;
using Kitana.Service.Model;
using Amazon.Runtime;
using Amazon;
using Amazon.S3.Model;
using Kitana.Infrastructure.Repository;
using System.Transactions;

namespace Kitana.Service.Services
{
    public class LessonService
    {
        private readonly IDBLessonRepository _lessonRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISession _session;
        private readonly AmazonS3Client _s3Client;
        private readonly IDBCourseRepository _courseRepository;

        public LessonService(IDBLessonRepository lessonRepository, IHttpContextAccessor accessor, IDBCourseRepository courseRepository)
        {
            _lessonRepository = lessonRepository;
            httpContextAccessor = accessor;
            _session = httpContextAccessor.HttpContext.Session;
            var accessKey = Environment.GetEnvironmentVariable("AWS_ACCESSKEY");
            var secretKey = Environment.GetEnvironmentVariable("AWS_SECRETACESSKEY");
            var region = Environment.GetEnvironmentVariable("AWS_REGION");

            var awsCredentials = new BasicAWSCredentials(accessKey, secretKey);
            var awsRegion = RegionEndpoint.GetBySystemName(region);

            _s3Client = new AmazonS3Client(awsCredentials, awsRegion);
            _courseRepository = courseRepository;
        }

        public async Task<IEnumerable<LessonRS>> GetLessonsByCourseIdAsync(int courseId)
        {
            var lessons = await _lessonRepository.GetLessonsByCourseIdAsync(courseId);
            await CheckExpiration(courseId);
            //return lessons.Select(var thumbnail = (await _lessonRepository.GetThumbnailBasedOnLessonId(lesson.LessonId));
            //return MapToLessonRS(updatedLesson, thumbnail););
            var lessonRSList = new List<LessonRS>();

            foreach (var lesson in lessons)
            {
                var thumbnail = await _lessonRepository.GetThumbnailBasedOnLessonId(lesson.LessonId);
                var lessonRS = MapToLessonRS(lesson, thumbnail);
                lessonRSList.Add(lessonRS);
            }
            return lessonRSList;
        }

        public async Task<LessonRS> GetLessonByIdAsync(int lessonId)
        {
            var lesson = await _lessonRepository.GetLessonByIdAsync(lessonId);
            if(lesson == null)
            {
                throw new ArgumentException("LessonId not found");
            }
            await CheckExpiration(lesson.CourseId);
            var thumbnail = (await _lessonRepository.GetThumbnailBasedOnLessonId(lesson.LessonId));
            return MapToLessonRS(lesson, thumbnail);
        }

        public async Task<LessonRS> CreateLessonAsync(int courseId, LessonRQ lessonRQ)
        {
            IFormFile attachments = lessonRQ.VideoFile;
            var userId = GetUserId();
            var lesson = MapToLesson(lessonRQ);
            lesson.CourseId = courseId;
            var courseDeatils = await _courseRepository.GetCourseByIdAsync(courseId) ?? throw new ArgumentException("course doesn't exist");
            if (courseDeatils.InstructorId != userId)
            {
                throw new ArgumentException("Don't have permission to access this resource");
            }
            var lessonList = await _lessonRepository.GetLessonsByCourseIdAsync(courseId);
            if (lessonList != null)
            {
                foreach(var item in lessonList)
                {
                    if(item.Title == lessonRQ.Title)
                    {
                        throw new ArgumentException("Title already exists");
                    }
                }
            }
            var createdLesson = await _lessonRepository.CreateLessonAsync(lesson);

            if (attachments != null)
            {
                var videoUrl = await UploadAttachmentsToS3Async(attachments, courseId, createdLesson.LessonId);
                createdLesson.VideoUrl = videoUrl;

                await _lessonRepository.UpdateLessonAsync(createdLesson);
            }

            var thumbnail = (await _lessonRepository.GetThumbnailBasedOnLessonId(lesson.LessonId));
            return MapToLessonRS(createdLesson, thumbnail);
        }

        public async Task<LessonRS> UpdateLessonAsync(int lessonId, LessonRQ lessonRQ)
        {
            var userId = GetUserId();
            var lesson = await _lessonRepository.GetLessonByIdAsync(lessonId) ?? throw new ArgumentException("lesson doesn't exist");
            ValidateLessonAccess(lesson, userId);
            var courseDeatils = await _courseRepository.GetCourseByIdAsync(lesson.CourseId);
            if (courseDeatils.InstructorId != userId)
            {
                throw new ArgumentException("Don't have permission to access this resource");
            }
            // Retrieve the old video URL
            var oldVideoUrl = lesson.VideoUrl;

            // Update lesson metadata
            MapLessonRQToLesson(lessonRQ, lesson);

            // Handle video updates
            if (lessonRQ.VideoFile != null)
            {
                // Upload new videos to S3
                var newVideoUrl = await UploadAttachmentsToS3Async(lessonRQ.VideoFile, lesson.CourseId, lesson.LessonId);

                // Update the lesson's video URL with the new video URL
                lesson.VideoUrl = newVideoUrl;
            }

            // Save the updated lesson to the database
            var updatedLesson = await _lessonRepository.UpdateLessonAsync(lesson);
            var thumbnail = (await _lessonRepository.GetThumbnailBasedOnLessonId(lesson.LessonId));
            return MapToLessonRS(updatedLesson, thumbnail);
        }
        private async Task CheckExpiration(int courseId)
        {
            var userId = GetUserId();
            var userCourse = await _lessonRepository.GetUserCourseWithUserIdAndCourseId(userId, courseId);
            if(userCourse == null)
            {
                throw new ArgumentException("not subscribed");
            }
            if (userCourse.AvailableTill < DateTime.Now)
            {
                throw new ArgumentException("subcription expired");
            }
        }
        public async Task<bool> DeleteLessonAsync(int lessonId)
        {
            var userId = GetUserId();
            var lesson = await _lessonRepository.GetLessonByIdAsync(lessonId) ?? throw new ArgumentException("lesson doesn't exist");
            ValidateLessonAccess(lesson, userId);
            //await DeleteVideoFromS3Async(lesson.VideoUrl);
            return await _lessonRepository.DeleteLessonAsync(lessonId);
        }
        public async Task<bool> CompletedLessonService(List<int> lessonIds)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (var lessonId in lessonIds)
                    {
                        var userId = Convert.ToInt32(_session.GetString("UserId"));
                        var lessonDetails = await _lessonRepository.GetLessonByIdAsync(lessonId);
                        if (lessonDetails == null)
                        {
                            throw new ArgumentException($"Lesson not found for ID: {lessonId}");
                        }
                        await CheckExpiration(lessonDetails.CourseId);
                        var noOfLessons = await _lessonRepository.GetLessonsByCourseIdAsync(lessonDetails.CourseId);
                        int totalLessons = noOfLessons.Count();
                        int completedLessons = 1;
                        int percentage = totalLessons > 0 ? (int)Math.Ceiling((decimal)completedLessons / totalLessons * 100) : 0;
                        await _lessonRepository.UpdateUserCourse(lessonDetails.CourseId, userId, percentage);
                    }
                    // Complete the transaction if all operations succeed
                    scope.Complete();
                    return true;
                }
                catch (Exception ex)
                {
                    scope.Dispose(); // Rollback will automatically occur upon disposal
                    throw;
                }
            }
        }

        private int GetUserId()
        {
            var userId = _session.GetString("UserId");
            return Convert.ToInt32(userId);
        }

        private void ValidateLessonAccess(Lesson lesson, int userId)
        {
            if (lesson == null)
            {
                throw new ArgumentException("Lesson not found");
            }

            if (lesson.Course.InstructorId != userId)
            {
                throw new SecurityException("You do not have access for these operations");
            }
        }

        private LessonRS MapToLessonRS(Lesson lesson, byte[] thumbnail)
        {
            if (lesson == null)
            {
                return null;
            }

            return new LessonRS
            {
                LessonId = lesson.LessonId,
                CourseId = lesson.CourseId,
                Title = lesson.Title,
                Description = lesson.Description,
                Video = lesson.VideoUrl,
                CreatedAt = lesson.CreatedAt,
                ThumbNail = thumbnail,
            };
        }

        private Lesson MapToLesson(LessonRQ lessonRQ)
        {
            return new Lesson
            {
                Title = lessonRQ.Title,
                Description = lessonRQ.Description,
                CreatedAt = DateTime.Now
            };
        }

        private void MapLessonRQToLesson(LessonRQ lessonRQ, Lesson lesson)
        {
            lesson.Title = lessonRQ.Title;
            lesson.Description = lessonRQ.Description;
        }

        private async Task<string> UploadAttachmentsToS3Async(IFormFile attachment, int courseId, int lessonId)
        {
            var uploadedUrls = new List<string>();
            var keyPrefix = $"{courseId}/{lessonId}";

            using (var stream = attachment.OpenReadStream())
            {
                var key = $"{keyPrefix}/{Guid.NewGuid()}_{Path.GetFileName(attachment.FileName)}";

                var fileTransferUtility = new TransferUtility(_s3Client);
                await fileTransferUtility.UploadAsync(stream, "sttskillforge", key);

                uploadedUrls.Add($"https://sttskillforge.s3.amazonaws.com/{key}");
            }

            return string.Join(",", uploadedUrls);
        }
        public async Task<Thumbnail> PostThumbnailService(ThumbnailRQ thumbnailImages)
        {
            var userId = GetUserId();
            var lessonDetails = await _lessonRepository.GetLessonByIdAsync(thumbnailImages.LessonId);
            var courseDeatils = await _courseRepository.GetCourseByIdAsync(lessonDetails.CourseId);
            if (courseDeatils.InstructorId != userId)
            {
                throw new ArgumentException("Don't have permission to access this resource");
            }
            byte[] imageData;
            using (var stream = thumbnailImages.Image.OpenReadStream())
            {
                using (var memoryStream = new MemoryStream())
                {
                    thumbnailImages.Image.CopyTo(memoryStream);
                    imageData = memoryStream.ToArray();
                }
            }
            var thumbnail = new Thumbnail()
            {
                Active = false,
                ThumbnailImage = imageData,
                LessonId = thumbnailImages.LessonId,
                CreatedAt = DateTime.Now,
            };
            try
            {
                var output = await _lessonRepository.PostThumbnailAsync(thumbnail);
                return output;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Image not supported, can be a size issue {ex.Message}");
            }
        }
        public async Task<ThumbnailRS> GetThumbnailService(int lessonId)
        {
            var userId = GetUserId();
            var lessons = await _lessonRepository.GetThumbnailAsync(lessonId);
            var output = new ThumbnailRS()
            {
                LessonId = lessonId,
                Thumbnail = new List<ThumbnailValue>()
            };
            foreach (var item in lessons)
            {
                var eachThumbnail = new ThumbnailValue()
                {
                    ThumbnailId = item.ThumbnailId,
                    ThumbnailImage = item.ThumbnailImage,
                };
                output.Thumbnail.Add(eachThumbnail);
            }
            return output;
        }
        public async Task<ThumbnailValue> ActivateThumbnailService(int thumbnailId)
        {
            var userId = GetUserId();
            var thumbnail = await _lessonRepository.ActivateThumbnailAsync(thumbnailId);
            var eachThumbnail = new ThumbnailValue()
            {
                ThumbnailId = thumbnail.ThumbnailId,
                ThumbnailImage = thumbnail.ThumbnailImage,
            };

            return eachThumbnail;
        }
    }
}
