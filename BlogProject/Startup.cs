using BlogProject.EmailServices;
using Business.Abstract;
using Business.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EfCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShopApp.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogProject
{
    public class Startup
    {
        private IConfiguration _configuration;
        public Startup(IConfiguration configuration) //app settingdeki email bilgileri için
        {
            _configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer("server=.\\SQLEXPRESS;database=blogProjectDb;integrated security=true;"));
            services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options => {
                // password
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = true;

                // Lockout                
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.AllowedForNewUsers = true;

                // options.User.AllowedUserNameCharacters = "";
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            });

            services.ConfigureApplicationCookie(options => {
                options.LoginPath = "/account/login";
                options.LogoutPath = "/account/logout";
                options.AccessDeniedPath = "/account/accessdenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.Cookie = new CookieBuilder
                {
                    HttpOnly = true,
                    Name = ".MyBlog.Security.Cookie",
                    SameSite=SameSiteMode.Strict
                };
            });


            services.AddScoped<IBlogRepository, EfCoreBlogRepository>();
            services.AddScoped<ICategoryRepository, EfCoreCategoryRepository>();
            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<IBlogService, BlogManager>();

            //email onay için
            services.AddScoped<IEmailSender, SmtpEmailSender>(i =>
                new SmtpEmailSender(
                    _configuration["EmailSender:Host"],
                    _configuration.GetValue<int>("EmailSender:Port"),
                    _configuration.GetValue<bool>("EmailSender:EnableSSL"),
                    _configuration["EmailSender:UserName"],
                    _configuration["EmailSender:Password"])
                );
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //SeedDatabase.Seed();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "contact",
                    pattern: "contact",
                    defaults: new { controller = "Home", action = "Contact" }
                );
                endpoints.MapControllerRoute(
                    name: "about",
                    pattern: "about",
                    defaults: new { controller = "Home", action = "About" }
                );
                
                endpoints.MapControllerRoute(
                    name: "adminbloglist",
                    pattern: "admin/blogs",
                    defaults: new { controller = "Admin", action = "BlogList" }
                );
                endpoints.MapControllerRoute(
                    name: "adminblogcreate",
                    pattern: "admin/blogs/create",
                    defaults: new { controller = "Admin", action = "BlogCreate" }
                );
                endpoints.MapControllerRoute(
                     name: "adminblogedit",
                     pattern: "admin/blogs/{id?}",
                     defaults: new { controller = "Admin", action = "BlogEdit" }
                 );
                //
                endpoints.MapControllerRoute(
                    name: "admincategorylist",
                    pattern: "admin/categories",
                    defaults: new { controller = "Admin", action = "CategoryList" }
                );
                endpoints.MapControllerRoute(
                    name: "admincategorycreate",
                    pattern: "admin/categories/create",
                    defaults: new { controller = "Admin", action = "CategoryCreate" }
                );
                endpoints.MapControllerRoute(
                     name: "admincategoryedit",
                     pattern: "admin/categories/{id?}",
                     defaults: new { controller = "Admin", action = "CategoryEdit" }
                 );
                endpoints.MapControllerRoute(
                    name: "search",
                    pattern: "search",
                    defaults: new { controller = "Blog", action = "Search" }
                );
                endpoints.MapControllerRoute(
                    name: "blogdetails",
                    pattern: "{url}",
                    defaults: new { controller = "Blog", action = "Details" }
                );
                endpoints.MapControllerRoute(
                    name: "blogs",
                    pattern: "blogs/{category?}",
                    defaults: new { controller = "Blog", action = "List" }
                );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }
}
