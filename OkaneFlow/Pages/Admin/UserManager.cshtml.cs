using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interface;
using Service.Models;

namespace OkaneFlow.Pages.Admin
{
    public class UserManagerModel : PageModel
    {
        private readonly IUserService _userService;

        public UserManagerModel(IUserService userService)
        {
            _userService = userService;
        }

        public List<UserModel> Users { get; set; } = new List<UserModel>();

        public async Task OnGetAsync()
        {
            Users = await _userService.GetAllUsersAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            await _userService.DeleteUserAsync(id);
            return RedirectToPage();
        }
    }
}
