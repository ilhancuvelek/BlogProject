using BlogProject.Models;
using Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BlogProject.Controllers
{
    public class BlogController : Controller
    {
        private IBlogService _blogService;
        public BlogController(IBlogService blogService)
        {
            _blogService=blogService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult list(string category,int page=1) {

            const int pageSize= 2;

            var blogs = _blogService.GetBlogsByCategory(category,page,pageSize);

            var blogViewModel = new BlogListViewModel()
            {
                PageInfo = new PageInfo()
                {
                    TotalItems = _blogService.GetCountByCategory(category),
                    CurrentPage=page,
                    CurrentCategory=category,
                    ItemsPerPage=pageSize
                },
                Blogs = blogs
            };

            return View(blogViewModel); 
        
        }
        [Authorize]
        public IActionResult Details(string url)
        {
            if (url == null)
            {
                return NotFound();
            }
            var blog = _blogService.GetBlogDetails(url);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }
        public IActionResult Search(string q)
        {
          
            var blogViewModel = new BlogListViewModel()
            {
                
                Blogs = _blogService.GetSearchResult(q)
            };
            return View(blogViewModel);
        }
    }
}
