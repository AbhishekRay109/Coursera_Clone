using System;
using System.Collections.Generic;

namespace Scaf.Models;

public partial class UserCourse
{
    public int UserCourseId { get; set; }

    public int CourseId { get; set; }

    public int UserId { get; set; }

    public int CompletedPercent { get; set; }
    public DateTime AvailableTill {  get; set; }
    public virtual Course Course { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
