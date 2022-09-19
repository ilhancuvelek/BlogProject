using Business.Abstract;
using DataAccess.Abstract;
using Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Concrete
{
    public class BlogManager : IBlogService
    {
        private IBlogRepository _blogRepository;
        public BlogManager(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

      

        public bool Create(Blog blog)
        {
            if (Validation(blog))
            {
                _blogRepository.Create(blog);
                return true;
            }
            return false;
            
        }

        public void Delete(Blog blog)
        {
            _blogRepository.Delete(blog);
        }

        public List<Blog> GetAll()
        {
            return _blogRepository.GetAll();
        }

        public Blog GetBlogDetails(string url)
        {
            return _blogRepository.GetBlogDetails(url);
        }

        public List<Blog> GetBlogsByCategory(string categoryName, int page, int pageSize)
        {
            return _blogRepository.GetBlogsByCategory(categoryName, page,pageSize);
        }

        public Blog GetById(int id)
        {
            return _blogRepository.GetById(id);
        }

        public int GetCountByCategory(string category)
        {
            return _blogRepository.GetCountByCategory(category);
        }

        

        public List<Blog> GetHomePageBlogs()
        {
            return _blogRepository.GetHomePageBlogs();
        }

        public List<Blog> GetSearchResult(string searchString)
        {
            return _blogRepository.GetSearchResult(searchString);
        }

        public void Update(Blog blog)
        {
            _blogRepository.Update(blog);
        }
        public string ErrorMessage { get; set; }
        public bool Validation(Blog entity)
        {
            var isValid = true;
            if (string.IsNullOrEmpty(entity.BlogTitle))
            {
                ErrorMessage += "Blog Başlığı girmelisiniz \n";
                isValid = false;
            }
            return isValid;
        }
    }
}
