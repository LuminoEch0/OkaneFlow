using DataAccessLayer;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Service;
using System.Globalization;
using OkaneFlow.Helpers; // register CurrentUserService

namespace OkaneFlow
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // 1. Add services to the container.
            builder.Services.AddRazorPages(options =>
            {
                // MANDATORY VALIDATION: Apply authorization to all pages by default
                options.Conventions.AuthorizeFolder("/");
                // Exclude the Login, Register and Logout pages
                options.Conventions.AllowAnonymousToPage("/Account/Login");
                options.Conventions.AllowAnonymousToPage("/Account/Register");
                options.Conventions.AllowAnonymousToPage("/Account/Logout");
            });

            // 2. Configure Cookie Authentication (The secure session management system)
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login"; // The page to redirect to when unauthorized
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                });

            // The rest of the services
            builder.Services.AddSingleton<ConnectionManager>();

            // Repositories and services
            builder.Services.AddScoped<BankAccountRepo>();
            builder.Services.AddScoped<CategoryRepo>();
            builder.Services.AddScoped<TransactionRepo>();
            builder.Services.AddScoped<TransactionTypeLookupRepo>();
            builder.Services.AddScoped<UserRepo>();

            builder.Services.AddScoped<BankAccountService>();
            builder.Services.AddScoped<CategoryService>();
            builder.Services.AddScoped<TransactionService>();
            builder.Services.AddScoped<TransactionTypeLookupService>();
            builder.Services.AddScoped<UserService>();

            // HttpContext accessor and current user helper (web project)
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<CurrentUserService>();

            var cultureInfo = new CultureInfo("en-NL");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Add services to the container.

            app.UseHttpsRedirection();// Redirect HTTP requests to HTTPS

            app.UseRouting();//Enables routing capabilities in the middleware pipeline

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();//- Maps static assets (like CSS, JS, images) to be served from the wwwroot folder.

            app.MapRazorPages()
               .WithStaticAssets();//Maps Razor Pages endpoints and ensures static assets are available to them.

            app.MapFallbackToPage("/Dashboard/MainDashboard/Dashboard");

            app.Run();
        }
    }
}
