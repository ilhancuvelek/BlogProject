using Entity;
using System.Collections.Generic;

namespace BlogProject.Models
{
    public class BlogDetailModel
    {
        public Blog Blog { get; set; }
        public List<Category> Categories { get; set; }
    }
}
