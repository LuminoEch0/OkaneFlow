using DataAccessLayer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Service;
using System.Globalization;
using OkaneFlow.Helpers;
using DataAccessLayer.Repositories; // register CurrentUserService
using Service.RepoInterface;
using Service.Interface;


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
            builder.Services.AddScoped<IBankAccountRepo, BankAccountRepo>();
            builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
            builder.Services.AddScoped<ITransactionRepo, TransactionRepo>();
            builder.Services.AddScoped<ITransactionTypeLookupRepo, TransactionTypeLookupRepo>();
            builder.Services.AddScoped<IUserRepo, UserRepo>();
            builder.Services.AddScoped<ISubscriptionRepo, SubscriptionRepo>();
            builder.Services.AddScoped<IDebtRepo, DebtRepo>();
            builder.Services.AddScoped<IUserPreferenceRepo, UserPreferenceRepo>();

            builder.Services.AddScoped<IBankAccountService, BankAccountService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();
            builder.Services.AddScoped<ITransactionTypeLookupService, TransactionTypeLookupService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
            builder.Services.AddScoped<IDebtService, DebtService>();
            builder.Services.AddScoped<IUserPreferenceService, UserPreferenceService>();

            // HttpContext accessor and current user helper (web project)
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService>();

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

            // Load user preferences for all pages
            app.UseUserPreferences();

            app.MapStaticAssets();//- Maps static assets (like CSS, JS, images) to be served from the wwwroot folder.

            app.MapRazorPages()
               .WithStaticAssets();//Maps Razor Pages endpoints and ensures static assets are available to them.

            app.MapFallbackToPage("/Dashboard/MainDashboard/Dashboard");

            app.Run();
        }
    }
}
