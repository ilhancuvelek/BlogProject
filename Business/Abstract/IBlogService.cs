using Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IBlogService: IValidator<Blog>
    {
        bool Create(Blog blog);
        void Update(Blog blog);
        void Delete(Blog blog);
        Blog GetById(int id);
        List<Blog> GetAll();

        Blog GetBlogDetails(string url);

        List<Blog> GetBlogsByCategory(string categoryName, int page, int pageSize);
        int GetCountByCategory(string category);

        List<Blog> GetHomePageBlogs();
        List<Blog> GetSearchResult(string searchString);
        
    }
}
