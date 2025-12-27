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

        public IActionResult OnGet(Guid id)
        {
            var debt = _debtService.GetDebtById(id);
            if (debt == null) return NotFound();
            Debt = DebtMapper.ToViewModel(debt);
            return Page();
        }

        public IActionResult OnPost()
        {
            var debt = _debtService.GetDebtById(id);
            if (debt == null) return NotFound();

            _debtService.DeleteDebt(id);

            return RedirectToPage("/Debt/DebtPage");
        }
    }
}
