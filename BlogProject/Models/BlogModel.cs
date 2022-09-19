using System;
using System.ComponentModel.DataAnnotations;

namespace BlogProject.Models
{
    public class BlogModel
    {
        public int BlogId { get; set; }
        [Required(ErrorMessage = "Url zorunlu bir alan.")]
        public string Url { get; set; }
        public string BlogTitle { get; set; }
        [Required(ErrorMessage = "BlogContent zorunlu bir alan.")]
        public string BlogContent { get; set; }
        [Required(ErrorMessage = "BlogThumbnailImage zorunlu bir alan.")]
        public string BlogThumbnailImage { get; set; }
        [Required(ErrorMessage = "BlogImage zorunlu bir alan.")]
        public string BlogImage { get; set; }
        public DateTime BlogCreateDate { get; set; }
        public int CategoryId { get; set; }

        public bool BlogStatus { get; set; }
        public bool IsHomePageBlog { get; set; }
    }
}
