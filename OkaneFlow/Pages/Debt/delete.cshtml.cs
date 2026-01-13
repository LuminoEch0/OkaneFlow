using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Mappers;
using OkaneFlow.ViewModels;
using Service.Interface;
using System;

namespace OkaneFlow.Pages.Debt
{
    public class deleteModel : PageModel
    {
        private readonly IDebtService _debtService;
        public deleteModel(IDebtService debtService)
        {
            _debtService = debtService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid id { get; set; } // DebtID

        public DebtVM Debt { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var debt = await _debtService.GetDebtByIdAsync(id);
            if (debt == null) return NotFound();
            Debt = DebtMapper.ToViewModel(debt);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var debt = await _debtService.GetDebtByIdAsync(id);
            if (debt == null) return NotFound();

            await _debtService.DeleteDebtAsync(id);

            return RedirectToPage("/Debt/DebtPage");
        }
    }
}
