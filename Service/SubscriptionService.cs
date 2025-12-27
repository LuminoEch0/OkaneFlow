using Service.Interface;
using Service.Models;
using Service.RepoInterface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepo _subscriptionRepo;
        private readonly ICategoryService _categoryService;
        private readonly ITransactionService _transactionService;
        private readonly IBankAccountService _bankAccountService;
        private readonly ITransactionTypeLookupService _typeLookupService;

        public SubscriptionService(
            ISubscriptionRepo subscriptionRepo,
            ICategoryService categoryService,
            ITransactionService transactionService,
            IBankAccountService bankAccountService,
            ITransactionTypeLookupService typeLookupService)
        {
            _subscriptionRepo = subscriptionRepo;
            _categoryService = categoryService;
            _transactionService = transactionService;
            _bankAccountService = bankAccountService;
            _typeLookupService = typeLookupService;
        }

        public List<SubscriptionModel> GetSubscriptionsByAccount(Guid accountId)
        {
            return _subscriptionRepo.GetSubscriptions(accountId);
        }

        public SubscriptionModel? GetSubscriptionById(Guid id)
        {
            return _subscriptionRepo.GetSubscriptionById(id);
        }

        public void CreateSubscription(SubscriptionModel subscription)
        {
            if (string.IsNullOrWhiteSpace(subscription.Name))
            {
                throw new ArgumentException("Subscription name cannot be empty.");
            }

            if (subscription.Amount < 0)
            {
                throw new ArgumentException("Amount cannot be negative.");
            }

            // 1. Transaction Logic (Immediate Payment)
            if (subscription.StartDate.Date <= DateTime.Now.Date)
            {
                var account = _bankAccountService.GetAccountById(subscription.AccountID);
                if (account == null) throw new InvalidOperationException("Bank account not found.");

                if (account.CurrentBalance < subscription.Amount)
                {
                    // Strict Blocking
                    throw new InvalidOperationException($"Insufficient funds. Current balance ({account.CurrentBalance}) is less than subscription amount ({subscription.Amount}).");
                }
            }

            // 2. Category Logic (Find or Create)
            if (subscription.CategoryID == Guid.Empty)
            {
                var categories = _categoryService.GetAllCategories(subscription.AccountID);
                var subCategory = categories.FirstOrDefault(c => c.CategoryName != null && c.CategoryName.Equals("Subscriptions", StringComparison.OrdinalIgnoreCase));

                if (subCategory != null)
                {
                    subscription.CategoryID = subCategory.CategoryID;
                }
                else
                {
                    // Create it
                    var newCategory = new CategoryModel(subscription.AccountID, "Subscriptions", 0, 0);
                    _categoryService.CreateCategory(newCategory);
                    subscription.CategoryID = newCategory.CategoryID;
                }
            }

            // 3. Auto-Allocation Logic
            // Increase the Category's allocated amount by the subscription amount
            var targetCategory = _categoryService.GetCategoryById(subscription.CategoryID);
            if (targetCategory != null)
            {
                targetCategory.AllocatedAmount += subscription.Amount;
                _categoryService.UpdateCategoryDetails(targetCategory);
            }

            // 4. Save Subscription
            _subscriptionRepo.CreateSubscription(subscription);

            // 5. Create Transaction if due now
            if (subscription.StartDate.Date <= DateTime.Now.Date)
            {
                int expenseTypeId = _typeLookupService.GetTypeIdFromName("Expense");
                var transaction = new TransactionModel
                {
                    // AccountID is passed as argument to CreateTransaction, not part of Model apparently based on previous error, checking Service definition... 
                    // Wait, Service/Models/TransactionModel.cs defined [CategoryID, Amount, Description, Date, Type]. It does NOT have AccountID.
                    // The TransactionService.CreateTransaction(model, accountId) takes accountId as 2nd arg.
                    // So we remove AccountID from initializer.

                    Amount = subscription.Amount,
                    CategoryID = subscription.CategoryID,
                    Description = $"Subscription: {subscription.Name}",
                    Date = DateTime.Now,
                    Type = expenseTypeId
                };

                _transactionService.CreateTransaction(transaction, subscription.AccountID);
            }
        }

        // Helper method for Alerts
        public List<SubscriptionModel> GetUpcomingInsufficientSubscriptions(Guid accountId)
        {
            var account = _bankAccountService.GetAccountById(accountId);
            if (account == null) return new List<SubscriptionModel>();

            var subscriptions = GetSubscriptionsByAccount(accountId);
            var upcoming = new List<SubscriptionModel>();

            foreach (var sub in subscriptions)
            {
                // Simple logic checks next occurrence based on Frequency
                // This is a naive implementation for "next 7 days"
                DateTime nextDate = sub.StartDate;
                while (nextDate < DateTime.Now.Date)
                {
                    if (sub.Frequency == "Monthly") nextDate = nextDate.AddMonths(1);
                    else if (sub.Frequency == "Yearly") nextDate = nextDate.AddYears(1);
                    else if (sub.Frequency == "Weekly") nextDate = nextDate.AddDays(7);
                    else break; // Should not happen
                }

                if (nextDate <= DateTime.Now.AddDays(7) && account.CurrentBalance < sub.Amount)
                {
                    upcoming.Add(sub);
                }
            }
            return upcoming;
        }


        public void UpdateSubscription(SubscriptionModel subscription)
        {
            if (string.IsNullOrWhiteSpace(subscription.Name))
            {
                throw new ArgumentException("Subscription name cannot be empty.");
            }
            if (subscription.Amount < 0)
            {
                throw new ArgumentException("Amount cannot be negative.");
            }
            _subscriptionRepo.UpdateSubscription(subscription);
        }

        public void DeleteSubscription(Guid id)
        {
            _subscriptionRepo.DeleteSubscription(id);
        }
    }
}
