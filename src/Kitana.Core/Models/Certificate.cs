using System;
using System.Collections.Generic;

namespace Scaf.Models;

public partial class Certificate
{
    public int CertificateId { get; set; }

    public int UserId { get; set; }

    public int CourseId { get; set; }

    public byte[] CertificateFile { get; set; }
    public DateTime? IssueDate { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
