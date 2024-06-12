using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitana.Service.Model
{
    using System.ComponentModel.DataAnnotations;

    public class CartItemRQ
    {
        [Required(ErrorMessage = "CourseId is required")]
        public int CourseId { get; set; }

        //[Required(ErrorMessage = "Quantity is required")]
        //[Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        //public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Discount must be a non-negative value")]
        public decimal Discount { get; set; }
    }

    public class CartItemRS
    {
        public int CartItemId { get; set; }
        public string CourseName { get; set; }
        public int Quantity { get; set; }
    }
}


