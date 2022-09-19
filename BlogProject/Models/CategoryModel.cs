using Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogProject.Models
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Kategori için 5-100 arasında değer giriniz.")]
        public string CategoryName { get; set; }
        [Required(ErrorMessage = "Url zorunludur.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Url için 5-100 arasında değer giriniz.")]
        public string Url { get; set; }
        public List<Blog> Blogs { get; set; }
    }
}
