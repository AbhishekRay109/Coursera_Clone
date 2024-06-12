using System;
using System.Collections.Generic;

namespace Scaf.Models;

public partial class Lesson
{
    public int LessonId { get; set; }

    public int CourseId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }
    public string? VideoUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();
    public virtual ICollection<Thumbnail> Thumbnails { get; set; } = new List<Thumbnail>();

    public virtual Course Course { get; set; } = null!;

}
