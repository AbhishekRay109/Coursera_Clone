using System;
using System.Collections.Generic;

namespace Scaf.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int UserId { get; set; }

    public int CourseId { get; set; }

    public string? ReviewText { get; set; }

    public int? RatingValue { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
