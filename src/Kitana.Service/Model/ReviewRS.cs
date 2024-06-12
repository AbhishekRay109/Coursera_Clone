using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitana.Service.Model
{
    public class ReviewRS
    {
        public string UserName { get; set; }
        public string CourseTitle { get; set; }
        public int? Rating { get; set; }
        public string Comment { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int ReviewId { get; internal set; }
    }
}
