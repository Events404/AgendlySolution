using Agendly.Data;
using DataAccess.Repository.IRepository;
using DataAccess.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Models;
using Utility;

namespace Agendly
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));




            //builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            //builder.Services.AddControllersWithViews();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
              Option => {
                  Option.Password.RequiredLength = 8;
                  Option.Password.RequireDigit = false;
                  Option.SignIn.RequireConfirmedAccount = true;
              })

           .AddEntityFrameworkStores<ApplicationDbContext>()
           .AddDefaultTokenProviders();
            builder.Services.AddScoped<EventIRepository, EventRepository>();
            builder.Services.AddScoped<CategoryIRepository, CategoryRepository>();


            builder.Services.AddAuthorization(); // إضافة هذه الخدمة

            // Add Controllers (if needed for MVC controllers)
            builder.Services.AddControllers(); // إضافة هذه الخدمة إذا كنت تستخدم MVC controllers

            // If you use Razor Pages, also add this
            builder.Services.AddRazorPages();

            builder.Services.AddSingleton<IEmailSender, EmailSender>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"); // This enables Areas routing
            app.MapRazorPages();


            app.Run();
        }
    }
}
