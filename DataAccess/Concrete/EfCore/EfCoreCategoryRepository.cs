using DataAccess.Abstract;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Concrete.EfCore
{
    public class EfCoreCategoryRepository : EfCoreGenericRepository<Category, BlogContext>, ICategoryRepository
    {
        public Category GetByIdWithBlogs(int id)
        {
            using (var context=new BlogContext())
            {
                return context.Categories.Where(c => c.CategoryId == id).Include(b => b.Blogs).FirstOrDefault();
            }
        }
    }
}
