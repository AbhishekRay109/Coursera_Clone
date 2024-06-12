using System.ComponentModel.DataAnnotations;

namespace Kitana.Service.Model
{
    public class CourseRQ
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Country { get; set; }

        [RegularExpression(@"^(Open|Closed)$", ErrorMessage = "EnrollmentStatus must be 'Open' or 'Closed'")]
        public string EnrollmentStatus { get; set; } = "Open";
        public int ActiveDays { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Price must be a non-negative value")]
        public int? Price { get; set; }
    }
}
