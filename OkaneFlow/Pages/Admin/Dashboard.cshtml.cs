using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interface;
using Service.Models;

namespace OkaneFlow.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly IUserService _userService;

        public DashboardModel(IUserService userService)
        {
            _userService = userService;
        }

        public int TotalUsers { get; set; }
        public int UsersLoggedInLast24Hours { get; set; }
        public int UsersLoggedInLast7Days { get; set; }

        public async Task OnGetAsync()
        {
            var users = await _userService.GetAllUsersAsync();
            TotalUsers = users.Count;

            var now = DateTime.UtcNow;
            UsersLoggedInLast24Hours = users.Count(u => u.LastLoginDate.HasValue && u.LastLoginDate.Value > now.AddHours(-24));
            UsersLoggedInLast7Days = users.Count(u => u.LastLoginDate.HasValue && u.LastLoginDate.Value > now.AddDays(-7));
        }
    }
}
