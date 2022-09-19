using BlogProject.Models;
using Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace BlogProject.Controllers
{
    public class HomeController : Controller
    {
        private IBlogService _blogService;
        public HomeController(IBlogService blogService)
        {
            _blogService = blogService;
        }
        
        public IActionResult Index()
        {
            var blogs = new BlogListViewModel() {Blogs= _blogService.GetHomePageBlogs()};
            return View(blogs);
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
    }
}
