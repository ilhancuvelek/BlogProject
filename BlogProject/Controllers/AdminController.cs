using BlogProject.Models;
using DataAccess.Abstract;
using Microsoft.AspNetCore.Mvc;
using Entity;
using System;
using Business.Concrete;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using DataAccess.Concrete.EfCore;
using System.Linq;
using Business.Abstract;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace BlogProject.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private IBlogService _blogService;
        private ICategoryService _categoryService;
        CategoryManager categoryManager = new CategoryManager(new EfCoreCategoryRepository());
        public AdminController(IBlogService blogService, ICategoryService categoryService)
        {
            _blogService = blogService;
            _categoryService = categoryService;
        }

        //blog
        public IActionResult BlogList()
        {
            var blogListViewModel = new BlogListViewModel()
            {
                Blogs= _blogService.GetAll()
            };
            return View(blogListViewModel);
        }
        [HttpGet]
        public IActionResult BlogCreate()
        {
            List<SelectListItem> categoryValues = (from x in categoryManager.GetAll() select new SelectListItem { Text = x.CategoryName, Value = x.CategoryId.ToString() }).ToList();
            ViewBag.cv = categoryValues;
            return View();
        }
        [HttpPost]
        public IActionResult BlogCreate(BlogModel blogModel)
        {
            if (ModelState.IsValid)
            {
                var entity = new Blog()
                {
                    BlogTitle = blogModel.BlogTitle,
                    BlogContent = blogModel.BlogContent,
                    BlogImage = blogModel.BlogImage,
                    BlogThumbnailImage = blogModel.BlogThumbnailImage,
                    BlogCreateDate = DateTime.Now,
                    CategoryId = blogModel.CategoryId
                };
                if (_blogService.Create(entity))
                {
                    CreateMessage("blog eklendi", "success");
                    return RedirectToAction("BlogList");
                    
                }
                CreateMessage(_blogService.ErrorMessage, "danger");
                return View(blogModel);
            }
            
            return View(blogModel);


        }
        [HttpGet]
        public IActionResult BlogEdit(int? id)
        {
            List<SelectListItem> categoryValues = (from x in categoryManager.GetAll() select new SelectListItem { Text = x.CategoryName, Value = x.CategoryId.ToString() }).ToList();
            ViewBag.cv = categoryValues;
            if (id == null)
            {
                return NotFound();
            }

            var entity = _blogService.GetById((int)id);

            if (entity == null)
            {
                return NotFound();
            }

            var model = new BlogModel()
            {
                BlogId = entity.BlogId,
                BlogTitle = entity.BlogTitle,
                Url = entity.Url,
                BlogThumbnailImage = entity.BlogThumbnailImage,
                BlogImage = entity.BlogImage,
                BlogContent = entity.BlogContent,
                CategoryId=entity.CategoryId,
                IsHomePageBlog=entity.IsHomePageBlog,
                BlogStatus=entity.BlogStatus
            };
            return View(model);

        }
        [HttpPost]
        public async Task<IActionResult> BlogEdit(BlogModel blogModel, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var entity = _blogService.GetById(blogModel.BlogId);
                if (entity == null)
                {
                    return NotFound();
                }
                entity.BlogTitle = blogModel.BlogTitle;
                entity.BlogThumbnailImage = blogModel.BlogThumbnailImage;
                entity.Url = blogModel.Url;
                entity.BlogContent = blogModel.BlogContent;
                entity.CategoryId = blogModel.CategoryId;
                entity.BlogStatus = blogModel.BlogStatus;
                entity.IsHomePageBlog = blogModel.IsHomePageBlog;

                if (file != null)
                {
                    var extention = Path.GetExtension(file.FileName);
                    var randomName = string.Format($"{Guid.NewGuid()}{extention}");
                    entity.BlogImage = randomName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", randomName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                _blogService.Update(entity);

                CreateMessage("blog güncellendi", "success");
                return RedirectToAction("BlogList");
            }
            List<SelectListItem> categoryValues = (from x in categoryManager.GetAll() select new SelectListItem { Text = x.CategoryName, Value = x.CategoryId.ToString() }).ToList();
            ViewBag.cv = categoryValues;
            return View(blogModel);

        }
        public IActionResult DeleteBlog(int blogId)
        {
            var blog= _blogService.GetById(blogId);
            if (blog!=null)
            {
                _blogService.Delete(blog);
            }

            CreateMessage("blog silindi", "success");

            return RedirectToAction("BlogList");
        }

        //category

        public IActionResult CategoryList()
        {
            var categoryListViewModel = new CategoryListViewModel()
            {
                Categories = _categoryService.GetAll()
            };
            return View(categoryListViewModel);
        }
        [HttpGet]
        public IActionResult CategoryCreate()
        {
            List<SelectListItem> categoryValues = (from x in categoryManager.GetAll() select new SelectListItem { Text = x.CategoryName, Value = x.CategoryId.ToString() }).ToList();
            ViewBag.cv = categoryValues;
            return View();
        }
        [HttpPost]
        public IActionResult CategoryCreate(CategoryModel categoryModel)
        {
            if (ModelState.IsValid)
            {
                var entity = new Category()
                {
                    CategoryName = categoryModel.CategoryName,
                    Url = categoryModel.Url,

                };
                _categoryService.Create(entity);
                CreateMessage("kategori eklendi", "success");
                return RedirectToAction("CategoryList");
            }
            return View(categoryModel);
            
        }
        [HttpGet]
        public IActionResult CategoryEdit(int? id)
        {
            List<SelectListItem> categoryValues = (from x in categoryManager.GetAll() select new SelectListItem { Text = x.CategoryName, Value = x.CategoryId.ToString() }).ToList();
            ViewBag.cv = categoryValues;
            if (id == null)
            {
                return NotFound();
            }

            var entity = _categoryService.GetByIdWithBlogs((int)id);

            if (entity == null)
            {
                return NotFound();
            }

            var model = new CategoryModel()
            {
                CategoryId = entity.CategoryId,
                CategoryName = entity.CategoryName,
                Url = entity.Url,
                Blogs=entity.Blogs
            };
            return View(model);

        }
        [HttpPost]
        public IActionResult CategoryEdit(CategoryModel categoryModel)
        {
            if (ModelState.IsValid)
            {
                var entity = _categoryService.GetById(categoryModel.CategoryId);
                if (entity == null)
                {
                    return NotFound();
                }
                entity.CategoryName = categoryModel.CategoryName;
                entity.Url = categoryModel.Url;

                _categoryService.Update(entity);

                CreateMessage("kategori güncellendi", "success");

                return RedirectToAction("CategoryList");
            }
            return View(categoryModel);

        }
        public IActionResult DeleteCategory(int categoryId)
        {
            var category = _categoryService.GetById(categoryId);
            if (category != null)
            {
                _categoryService.Delete(category);
            }

            CreateMessage("kategori silindi", "success");

            return RedirectToAction("CategoryList");
        }


        private void CreateMessage(string message, string alerttype)
        {
            var msg = new AlertMessage
            {
                Message = message,
                AlertType = alerttype
            };
            TempData["message"] = JsonConvert.SerializeObject(msg);
        }
    }
}
