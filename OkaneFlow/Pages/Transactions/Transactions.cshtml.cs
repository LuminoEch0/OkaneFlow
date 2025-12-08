using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OkaneFlow.Helpers;
using Service.Interface;
using OkaneFlow.ViewModels;
using OkaneFlow.Mappers;

namespace OkaneFlow.Pages.Transactions
{
    public class TransactionsModel : PageModel
    {
        private readonly ITransactionService _transactionService;
        private readonly IBankAccountService _bankAccountService;
        private readonly ICategoryService _categoryService;
        private readonly ICurrentUserService _currentUser;


        public TransactionsModel(ITransactionService transactionService, IBankAccountService bankAccountService, ICategoryService categoryService, ICurrentUserService currentUser)
        {
            _transactionService = transactionService;
            _bankAccountService = bankAccountService;
            _categoryService = categoryService;
            _currentUser = currentUser;
        }

        public List<TransactionVM> Transactions { get; set; } = new();
        public List<BankAccountVM> BankAccounts { get; set; } = new();
        public Dictionary<Guid, string> CategoryNames { get; set; } = new Dictionary<Guid, string>();

        [BindProperty(SupportsGet = true)]
        public Guid AccountId { get; set; }

        public IActionResult OnGet(Guid? accountId)
        {
            var tempBankAccount = _bankAccountService.GetAllBankAccounts(_currentUser.UserGuid);
            BankAccounts = Mappers.BankAccountMapper.ToViewModelList(tempBankAccount);

            if (accountId.HasValue)
            {
                AccountId = accountId.Value;
                Transactions = TransactionMapper.ToViewModelList(_transactionService.GetTransactionsByAccountId(AccountId));
            }
            else if (BankAccounts.Any())
            {
                // Default to the first account if none selected
                AccountId = BankAccounts.First().AccountID;
                Transactions = TransactionMapper.ToViewModelList(_transactionService.GetTransactionsByAccountId(AccountId));
            }

            if (AccountId != Guid.Empty)
            {
                var categories = _categoryService.GetAllCategories(AccountId);
                CategoryNames = categories.ToDictionary(c => c.CategoryID, c => c.CategoryName ?? "Unknown");
            }

            return Page();
        }
    }
}