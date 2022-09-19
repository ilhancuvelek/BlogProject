using Business.Abstract;
using DataAccess.Concrete.EfCore;
using Entity;
using Microsoft.AspNetCore.Mvc;

namespace BlogProject.ViewComponents
{
    public class Categories:ViewComponent
    {
        private ICategoryService _categoryService;
        public Categories(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        
        
        public IViewComponentResult Invoke()
        {
            if (RouteData.Values["category"] != null)
            {
                ViewBag.SelectedCategory = RouteData?.Values["category"];
            }

            return View(_categoryService.GetAll());
        }
    }
}
