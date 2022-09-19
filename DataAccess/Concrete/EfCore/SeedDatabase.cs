using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Concrete.EfCore
{
    public class SeedDatabase
    {
        public static void Seed()
        {
            var context = new BlogContext();
            if (context.Database.GetPendingMigrations().Count() == 0)
            {
                if (context.Categories.Count() == 0)
                {
                    context.Categories.AddRange(Categories);
                }

                if (context.Blogs.Count() == 0)
                {
                    context.Blogs.AddRange(Blogs);
                }
            }
            context.SaveChanges();
        }
        private static Category[] Categories =
        {
            new Category(){CategoryName="Yazılım"},
            new Category(){CategoryName="Dizi-Film"},
            new Category(){CategoryName="Seyahat"}
        };
        private static Blog[] Blogs =
        {
            new Blog(){BlogTitle="Avangers",BlogContent="içerik",BlogThumbnailImage="resim1.jpg",BlogImage="resim1.jpg",BlogCreateDate=new DateTime(2022,08,14),BlogStatus=true,CategoryId=1},
            new Blog(){BlogTitle="Tatil Tavsiyesi",BlogContent="içerik",BlogThumbnailImage="resim4.jpg",BlogImage="resim3.jpg",BlogCreateDate=new DateTime(2022,07,11),BlogStatus=true,CategoryId=2},
            new Blog(){BlogTitle="Harry Potter",BlogContent="içerik",BlogThumbnailImage="resim2.jpg",BlogImage="resim2.jpg",BlogCreateDate=new DateTime(2022,06,13),BlogStatus=true, CategoryId = 1},
            new Blog(){BlogTitle="Asp.net web programlama",BlogContent="içerik",BlogThumbnailImage="resim4.jpg",BlogImage="resim4.jpg",BlogCreateDate=new DateTime(2022,05,15),BlogStatus=true, CategoryId = 2},
            new Blog(){BlogTitle="Python Programlama",BlogContent="içerik",BlogThumbnailImage="resim5.jpg",BlogImage="resim5.jpg",BlogCreateDate=new DateTime(2022,04,11),BlogStatus=true, CategoryId = 1},
        };
    }
}
