using System;
using System.Collections.Generic;

namespace Scaf.Models;

public partial class Note
{
    public int NotesId { get; set; }

    public int LessonId { get; set; }

    public string NoteLink { get; set; } = null!;
}
