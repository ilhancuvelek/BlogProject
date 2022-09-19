using Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Abstract
{
    public interface IBlogRepository:IRepository<Blog>
    {
        Blog GetBlogDetails(string url);
        List<Blog> GetBlogsByCategory(string categoryName, int page, int pageSize);
        int GetCountByCategory(string category);
        List<Blog> GetHomePageBlogs();
        List<Blog> GetSearchResult(string searchString);
        
    }
}
