using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitana.Service.Model
{
    public class ThumbnailRQ
    {
        [Required]
        public IFormFile Image {  get; set; }
        [Required]
        public int LessonId {  get; set; }
    }
    public class ThumbnailRS
    {
        public List<ThumbnailValue> Thumbnail { get; set; }
        public int LessonId { get; set; }
    }
    public class ThumbnailValue
    {
        public int ThumbnailId {  get; set; }
        public byte[] ThumbnailImage { get; set; }
    }
}
