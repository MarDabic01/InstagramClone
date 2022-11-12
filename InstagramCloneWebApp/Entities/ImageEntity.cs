using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace InstagramCloneWebApp.Entities
{
    public class ImageEntity
    {
        [Key]
        public int id { get; set; }
        public string ImageDecsription { get; set; }
        public string ImageAuthor { get; set; }
        public string ImageData { get; set; }
        public string authorname { get; set; }
        public string authorpic { get; set; }
        public int likes { get; set; }
        public DateTime ImageDate { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
