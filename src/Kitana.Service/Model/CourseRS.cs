using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitana.Service.Model
{
    public class CourseRS
    {
        public int CourseId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string InstructorName { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Country { get; set; }

        public string? EnrollmentStatus { get; set; }
        public int? Price { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

    }
}
