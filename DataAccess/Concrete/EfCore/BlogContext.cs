using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Concrete.EfCore
{
    public class BlogContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlite("Data Source=blogDb");
            optionsBuilder.UseSqlServer("server=.\\SQLEXPRESS;database=blogProjectDb;integrated security=true;");
        }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Category> Categories { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    //modelBuilder.Entity<BlogCategory>().HasKey(c => new { c.CategoryId, c.BlogId });

        //    modelBuilder.Entity<Blog>()
        //    .HasOne<Category>(s => s.Category)
        //    .WithMany(g => g.Blogs)
        //    .HasForeignKey(s => s.CategoryId);
        //}
    }
}
