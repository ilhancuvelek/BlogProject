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
using BlogProject.Extensions;
using Microsoft.AspNetCore.Identity;
using ShopApp.Identity;

namespace BlogProject.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private IBlogService _blogService;
        private ICategoryService _categoryService;
        CategoryManager categoryManager = new CategoryManager(new EfCoreCategoryRepository());

        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager;
        public AdminController(IBlogService blogService, ICategoryService categoryService, RoleManager<IdentityRole> roleManager,UserManager<User> userManager)
        {
            _blogService = blogService;
            _categoryService = categoryService;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        //roles
        public IActionResult RoleList()
        {
            return View(_roleManager.Roles);
        }
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleModel roleModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleModel.Name));
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList", "Admin");
                }
            }
            return View(roleModel);
        }
        public async Task<IActionResult> RoleEdit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            var members = new List<User>();
            var nonmembers = new List<User>();
            foreach (var user in _userManager.Users)
            {
                var list = await _userManager.IsInRoleAsync(user, role.Name) ? members : nonmembers;
                list.Add(user);
            }
            var model = new RoleDetails()
            {
                Role = role,
                Members = members,
                NonMembers = nonmembers
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> RoleEdit(RoleEditModel roleEditModel)
        {
            if (ModelState.IsValid)
            {
                foreach (var userId in roleEditModel.IdsToAdd ?? new string[] { })
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user!=null)
                    {
                        var result = await _userManager.AddToRoleAsync(user, roleEditModel.RoleName);
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                }
                foreach (var userId in roleEditModel.IdsToDelete ?? new string[] { })
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        var result = await _userManager.RemoveFromRoleAsync(user, roleEditModel.RoleName);
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                }
            }
            return Redirect("/admin/role/" + roleEditModel.RoleId);
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
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "blog eklendi",
                        Message = "blog eklendi.",
                        AlertType = "success"
                    });
                    return RedirectToAction("BlogList");
                    
                }
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Hata",
                    Message = _blogService.ErrorMessage,
                    AlertType = "danger"
                });
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
                TempData.Put("message", new AlertMessage()
                {
                    Title = "blog güncellendi",
                    Message = "blog güncellendi",
                    AlertType = "success"
                });
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
            TempData.Put("message", new AlertMessage()
            {
                Title = "blog silindi",
                Message = "blog silindi",
                AlertType = "danger"
            });

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
                TempData.Put("message", new AlertMessage()
                {
                    Title = "kategori eklendi",
                    Message = "kategori eklendi",
                    AlertType = "success"
                });
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
                TempData.Put("message", new AlertMessage()
                {
                    Title = "kategori güncellendi",
                    Message = "kategori güncellendi",
                    AlertType = "success"
                });

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
            TempData.Put("message", new AlertMessage()
            {
                Title = "kategori silindi",
                Message = "kategori silindi",
                AlertType = "danger"
            });
            return RedirectToAction("CategoryList");
        }
        
    }
}
