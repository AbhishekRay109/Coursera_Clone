using System;
using System.Collections.Generic;

namespace Scaf.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public string? NotificationText { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual User User { get; set; } = null!;
}
