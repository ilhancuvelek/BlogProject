using Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface ICategoryService: IValidator<Category>
    {
        void Create(Category category);
        void Update(Category category);
        void Delete(Category category);
        Category GetById(int id);
        List<Category> GetAll();

        Category GetByIdWithBlogs(int id);
    }
}
