using OkaneFlow.Helpers;
using OkaneFlow.Models;
using OkaneFlow.Services.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OkaneFlow.Pages.Dashboard.Category
{
    public class CategoryPageModel : PageModel
    {
        private readonly CategoryService _accountService;
        public CategoryPageModel(CategoryService accountService)
        {
            _accountService = accountService;
        }
        public List<CategoryModel> Categories { get; set; } = new List<CategoryModel>();

        public void OnGet(Guid id)
        {
            Categories = _accountService.GetAllCategories(id);
        }
    }
}
