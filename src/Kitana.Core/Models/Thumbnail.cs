using System;
using System.Collections.Generic;

namespace Scaf.Models;

public partial class Thumbnail
{
    public int ThumbnailId { get; set; }

    public int LessonId { get; set; }

    public byte[] ThumbnailImage { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public bool Active {  get; set; }

    public virtual Lesson Video { get; set; } = null!;
}
