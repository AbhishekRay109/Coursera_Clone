using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitana.Service.Model
{
    public class UserCourseRS
    {
        public int CourseId { get; set; }
        public int UserId { get; set; }
        public int CompletedPercent { get; set; }
        public DateTime AvailableTill { get; set; }
    }
}
