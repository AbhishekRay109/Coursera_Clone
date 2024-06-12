using System;
using System.Collections.Generic;

namespace Scaf.Models;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int CartId { get; set; }

    public int CourseId { get; set; }

    public int Quantity { get; set; }

    public decimal Discount { get; set; }

    public virtual ShoppingCart Cart { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;
}
