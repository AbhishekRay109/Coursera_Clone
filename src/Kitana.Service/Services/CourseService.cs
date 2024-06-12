using iText.Kernel.Pdf;
using Kitana.Core.Interfaces;
using Kitana.Service.Model;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Scaf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Events;

namespace Kitana.Service.Services
{
    public class CourseService
    {
        private readonly IDBCourseRepository _courseRepository;
        private readonly ISession _session;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CourseService(IDBCourseRepository courseRepository, IHttpContextAccessor accessor)
        {
            httpContextAccessor = accessor;
            _session = httpContextAccessor.HttpContext.Session;
            _courseRepository = courseRepository;
        }

        public async Task<IEnumerable<CourseRS>> GetCoursesAsync()
        {
            var courses = await _courseRepository.GetCoursesAsync();
            return courses.Select(MapToCourseRS);
        }

        public async Task<CourseRS> GetCourseByIdAsync(int courseId)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            return MapToCourseRS(course);
        }

        public async Task<CourseRS> CreateCourseAsync(CourseRQ courseRQ)
        {
            var userId = Convert.ToInt32(_session.GetString("UserId"));
            var course = MapToCourse(courseRQ);
            course.InstructorId = userId;
            await CheckCourseDetailsAsync(course);
            var createdCourse = await _courseRepository.CreateCourseAsync(course);
            return MapToCourseRS(createdCourse);
        }

        public async Task<CourseRS> UpdateCourseAsync(int courseId, CourseRQ courseRQ)
        {
            var userId = Convert.ToInt32(_session.GetString("UserId"));
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                return null;
            }
            if (course.InstructorId == userId)
            {
                MapCourseRQToCourse(courseRQ, course);
                await CheckCourseDetailsAsync(course);
                var updatedCourse = await _courseRepository.UpdateCourseAsync(course);
                return MapToCourseRS(updatedCourse);
            }
            else
            {
                throw new SecurityException("You do not have access for these operations");
            }
        }
        private async Task CheckCourseDetailsAsync(Course demoCourse)
        {
            var allCourses = await _courseRepository.GetCoursesAsync();
            foreach (var item in allCourses)
            {
                if (item.InstructorId == demoCourse.InstructorId && item.Title == demoCourse.Title)
                {
                    throw new ArgumentException("Title already exists");
                }
            }
        }
        public async Task<bool> DeleteCourseAsync(int courseId)
        {
            var userId = Convert.ToInt32(_session.GetString("UserId"));
            var role = _session.GetString("Admin");

            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                throw new ArgumentException("No such caourse found");
            }
            if (course.InstructorId == userId || role == "Admin")
            {
                return await _courseRepository.DeleteCourseAsync(courseId);
            }
            else
            {
                throw new SecurityException("You do not have access for these operations");
            }
        }
        public async Task<byte[]> FetchCertificateAsync(int courseId)
        {
            var userId = Convert.ToInt32(_session.GetString("UserId"));
            var userCourse = await _courseRepository.GetUserCourseRepoAsync(courseId, userId);

            if (userCourse == null)
            {
                throw new ArgumentException($"Course with ID {courseId} is not enrolled by you.");
            }

            if (userCourse.CompletedPercent >= 100)
            {
                var alreadyCertified = await _courseRepository.GetCertificateAsync(userId, courseId);
                if (alreadyCertified != null)
                {
                    return alreadyCertified.CertificateFile;
                }
                var certificate = await GenerateCertificateAsync(courseId, userId);
                var newCerti = new Certificate()
                {
                    CourseId = courseId,
                    UserId = userId,
                    IssueDate = DateTime.Now,
                    CertificateFile = certificate,
                };
                await _courseRepository.PostCertificateAsync(newCerti);
                return certificate;
            }
            else
            {
                throw new ArgumentException($"Dear {userCourse.User.FullName}, please complete the course first to receive the certificate.");
            }
        }
        public async Task<byte[]> GenerateCertificateAsync(int courseId, int userId)
        {
            var userCourse = await _courseRepository.GetUserCourseRepoAsync(courseId, userId);

            if (userCourse == null)
            {
                throw new ArgumentException($"Course with ID {courseId} is not enrolled by the user.");
            }

            if (userCourse.CompletedPercent < 100)
            {
                throw new ArgumentException($"Dear {userCourse.User.FullName}, please complete the course first to receive the certificate.");
            }

            // Generate a PDF certificate
            using (MemoryStream stream = new MemoryStream())
            {
                using (var writer = new PdfWriter(stream))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        // Set page properties for landscape orientation
                        PageSize pageSize = PageSize.A4.Rotate(); // Rotate the page to landscape
                        pdf.SetDefaultPageSize(pageSize);

                        var document = new Document(pdf);

                        // Set up fonts
                        PdfFont titleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                        PdfFont textFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                        // Add border to the entire page
                        PdfCanvas canvas = new PdfCanvas(pdf.AddNewPage());
                        Rectangle pageSizeRect = new Rectangle(36, 36, pageSize.GetWidth() - 72, pageSize.GetHeight() - 72); // Add margin of 36 points
                        canvas.SaveState();
                        canvas.SetLineWidth(36f); // Set border line width to 36 points (1/2 inch)
                        canvas.SetStrokeColor(DeviceGray.BLACK); // Set border color to black
                        canvas.Rectangle(pageSizeRect);
                        canvas.Stroke();
                        canvas.RestoreState();

                        // Certificate title
                        Paragraph title = new Paragraph("Certificate of Completion")
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetFont(titleFont)
                            .SetFontSize(24);

                        // Student name (highlighted)
                        Paragraph studentName = new Paragraph($"This certifies that {userCourse.User.FullName}")
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetFont(textFont)
                            .SetFontSize(18)
                            .SetItalic()
                            .SetBold();

                        // Course title (highlighted)
                        Paragraph courseTitle = new Paragraph($"{userCourse.Course.Title}")
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetFont(titleFont)
                            .SetFontSize(28)
                            .SetItalic()
                            .SetBold();

                        // Completion date
                        Paragraph completionDate = new Paragraph($"Completion Date: {DateTime.Now.ToString("MMMM dd, yyyy")}")
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetFont(textFont)
                            .SetFontSize(16);

                        // Additional details (customizable)
                        Paragraph instructorName = new Paragraph($"Instructor: {userCourse.Course.Instructor.FullName}")
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetFont(textFont)
                            .SetFontSize(16);

                        // Add elements to the document with margins
                        document.SetMargins(100f, 100f, 100f, 100f); // Set margins (adjust as needed)
                        document.Add(title);
                        document.Add(studentName);
                        document.Add(courseTitle);
                        document.Add(completionDate);
                        document.Add(instructorName);

                        document.Close();
                    }
                }
                return stream.ToArray();
            }
        }
        // Helper methods for mapping between models
        private CourseRS MapToCourseRS(Course course)
        {
            if (course == null)
            {
                return null;
            }

            return new CourseRS
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description,
                InstructorName = course.Instructor.FullName,
                City = course.City,
                State = course.State,
                Country = course.Country,
                EnrollmentStatus = course.EnrollmentStatus,
                Price = course.Price,
                CreatedAt = course.CreatedAt,
                UpdatedAt = course.UpdatedAt
            };
        }

        private Course MapToCourse(CourseRQ courseRQ)
        {
            return new Course
            {
                ActiveTime = courseRQ.ActiveDays,
                Title = courseRQ.Title,
                Description = courseRQ.Description,
                City = courseRQ.City,
                State = courseRQ.State,
                Country = courseRQ.Country,
                EnrollmentStatus = courseRQ.EnrollmentStatus,
                Price = courseRQ.Price,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }

        private void MapCourseRQToCourse(CourseRQ courseRQ, Course course)
        {
            course.ActiveTime = courseRQ.ActiveDays;
            course.Title = courseRQ.Title;
            course.Description = courseRQ.Description;
            course.City = courseRQ.City;
            course.State = courseRQ.State;
            course.Country = courseRQ.Country;
            course.EnrollmentStatus = courseRQ.EnrollmentStatus;
            course.Price = courseRQ.Price;
            course.UpdatedAt = DateTime.Now;
        }
    }
}
