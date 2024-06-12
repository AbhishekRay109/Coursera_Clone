using System;
using System.Collections.Generic;

namespace Scaf.Models;

public partial class SFTransaction
{
    public int TransactionId { get; set; }

    public string CourseIds { get; set; }

    public string TransactionStatus { get; set; } = null!;

    public decimal? TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ShoppingCart Cart { get; set; } = null!;
}
