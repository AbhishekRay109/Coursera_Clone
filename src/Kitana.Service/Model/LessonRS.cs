using Microsoft.AspNetCore.Http;

namespace Kitana.Service.Model
{
    public class LessonRS
    {
        public int LessonId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Video { get; set; }
        public int CourseId { get; set; }
        public byte[] ThumbNail { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
