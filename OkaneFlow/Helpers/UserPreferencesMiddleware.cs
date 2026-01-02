using Service.Interface;
using Service.Models;

namespace OkaneFlow.Helpers
{
    /// <summary>
    /// Provides user preferences to views via ViewData.
    /// This is registered as middleware to make preferences available on every page.
    /// </summary>
    public class UserPreferencesMiddleware
    {
        private readonly RequestDelegate _next;

        public UserPreferencesMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUser, IUserPreferenceService preferenceService)
        {
            // Only load preferences for authenticated users
            if (currentUser.IsAuthenticated && currentUser.UserGuid != Guid.Empty)
            {
                try
                {
                    var prefs = await preferenceService.GetOrCreateAsync(currentUser.UserGuid);
                    context.Items["UserPreferences"] = prefs;
                }
                catch
                {
                    // If preference loading fails, use defaults
                    context.Items["UserPreferences"] = new UserPreferenceModel
                    {
                        DarkMode = false,
                        EmailNotifications = true,
                        Currency = "EUR",
                        DateFormat = "dd/MM/yyyy"
                    };
                }
            }
            else
            {
                // Default preferences for non-authenticated users
                context.Items["UserPreferences"] = new UserPreferenceModel
                {
                    DarkMode = false,
                    EmailNotifications = true,
                    Currency = "EUR",
                    DateFormat = "dd/MM/yyyy"
                };
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Extension method to register the middleware.
    /// </summary>
    public static class UserPreferencesMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserPreferences(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserPreferencesMiddleware>();
        }
    }
}
