using System;
using System.Collections.Generic;

namespace Scaf.Models;

public partial class Assessment
{
    public int AssessmentId { get; set; }

    public int LessonId { get; set; }

    public string Type { get; set; } = null!;

    public string Question { get; set; } = null!;

    public string CorrectAnswer { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Lesson Lesson { get; set; } = null!;
}
