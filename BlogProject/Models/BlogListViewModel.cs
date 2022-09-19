using Entity;
using System;
using System.Collections.Generic;

namespace BlogProject.Models
{
    public class PageInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public string CurrentCategory { get; set; }

        public int TotalPages()
        {
            return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
        }
    }
    public class BlogListViewModel
    {
        public PageInfo PageInfo { get; set; }
        public List<Blog> Blogs { get; set; }
    }
}
