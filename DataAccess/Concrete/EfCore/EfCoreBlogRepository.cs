using DataAccess.Abstract;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Concrete.EfCore
{
    public class EfCoreBlogRepository : EfCoreGenericRepository<Blog, BlogContext>, IBlogRepository
    {
        public Blog GetBlogDetails(string url)
        {
            using (var context=new BlogContext())
            {
                return context.Blogs.Where(b => b.Url == url).Include(c => c.Category).FirstOrDefault();
            }
        }

        public List<Blog> GetBlogsByCategory(string categoryName,int page,int pageSize)
        {
            using (var context = new BlogContext())
            {
                var blogs = context.Blogs.Where(b=>b.BlogStatus).AsQueryable();
                if (!string.IsNullOrEmpty(categoryName))
                {
                    blogs = blogs
                    .Include(c => c.Category)
                    .Where(s => s.Category.Url.Contains(categoryName));
                }
                return blogs.Skip((page-1)*pageSize).Take(pageSize).ToList();
                  
            }
        }

        public int GetCountByCategory(string category)
        {
            using (var context = new BlogContext())
            {
                var blogs = context.Blogs.Where(b=>b.BlogStatus).AsQueryable();
                if (!string.IsNullOrEmpty(category))
                {
                    blogs=blogs.Include(c => c.Category).Where(i => i.Category.CategoryName == category);
                }
                return blogs.Count();
            }
        }

        public List<Blog> GetSearchResult(string searchString)
        {
            using (var context = new BlogContext())
            {
                var blogs = context.Blogs.Where(b => b.BlogStatus & (b.BlogTitle.ToLower().Contains(searchString.ToLower()) || b.BlogContent.ToLower().Contains(searchString.ToLower()))).AsQueryable();
                return blogs.ToList();
            }
        }

        List<Blog> IBlogRepository.GetHomePageBlogs()
        {
            using (var context = new BlogContext())
            {
                var blogs = context.Blogs.Where(b=>b.IsHomePageBlog&&b.BlogStatus).ToList();
                return blogs;
            }
        }
    }
}
