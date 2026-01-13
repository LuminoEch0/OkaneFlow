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

        public async Task<List<SubscriptionModel>> GetSubscriptionsByAccountAsync(Guid accountId)
        {
            return await Task.Run(() => _subscriptionRepo.GetSubscriptions(accountId));
        }

        public async Task<SubscriptionModel?> GetSubscriptionByIdAsync(Guid id)
        {
            return await Task.Run(() => _subscriptionRepo.GetSubscriptionById(id));
        }

        public async Task CreateSubscriptionAsync(SubscriptionModel subscription)
        {
            if (string.IsNullOrWhiteSpace(subscription.Name))
            {
                throw new ArgumentException("Subscription name cannot be empty.");
            }

            if (subscription.Amount < 0)
            {
                throw new ArgumentException("Amount cannot be negative.");
            }

            if (subscription.StartDate.Date <= DateTime.Now.Date)
            {
                var account = await _bankAccountService.GetAccountByIdAsync(subscription.AccountID);
                if (account == null) throw new InvalidOperationException("Bank account not found.");

                if (account.CurrentBalance < subscription.Amount)
                {
                    throw new InvalidOperationException($"Insufficient funds. Current balance ({account.CurrentBalance}) is less than subscription amount ({subscription.Amount}).");
                }
            }

            if (subscription.CategoryID == Guid.Empty)
            {
                var categories = await _categoryService.GetAllCategoriesAsync(subscription.AccountID);
                var subCategory = categories.FirstOrDefault(c => c.CategoryName != null && c.CategoryName.Equals("Subscriptions", StringComparison.OrdinalIgnoreCase));

                if (subCategory != null)
                {
                    subscription.CategoryID = subCategory.CategoryID;
                }
                else
                {
                    var newCategory = new CategoryModel(subscription.AccountID, "Subscriptions", 0, 0);
                    await _categoryService.CreateCategoryAsync(newCategory);
                    subscription.CategoryID = newCategory.CategoryID;
                }
            }

            var targetCategory = await _categoryService.GetCategoryByIdAsync(subscription.CategoryID);
            if (targetCategory != null)
            {
                targetCategory.AllocatedAmount += subscription.Amount;
                await _categoryService.UpdateCategoryDetailsAsync(targetCategory);
            }

            await Task.Run(() => _subscriptionRepo.CreateSubscription(subscription));

            if (subscription.StartDate.Date <= DateTime.Now.Date)
            {
                int expenseTypeId = await _typeLookupService.GetTypeIdFromNameAsync("Expense");
                var transaction = new TransactionModel
                {
                    Amount = subscription.Amount,
                    CategoryID = subscription.CategoryID,
                    Description = $"Subscription: {subscription.Name}",
                    Date = DateTime.Now,
                    Type = expenseTypeId
                };

                await _transactionService.CreateTransactionAsync(transaction, subscription.AccountID);
            }
        }

        public async Task<List<SubscriptionModel>> GetUpcomingInsufficientSubscriptionsAsync(Guid accountId)
        {
            var account = await _bankAccountService.GetAccountByIdAsync(accountId);
            if (account == null) return new List<SubscriptionModel>();

            var subscriptions = await GetSubscriptionsByAccountAsync(accountId);
            var upcoming = new List<SubscriptionModel>();

            foreach (var sub in subscriptions)
            {
                DateTime nextDate = sub.StartDate;
                while (nextDate < DateTime.Now.Date)
                {
                    if (sub.Frequency == "Monthly") nextDate = nextDate.AddMonths(1);
                    else if (sub.Frequency == "Yearly") nextDate = nextDate.AddYears(1);
                    else if (sub.Frequency == "Weekly") nextDate = nextDate.AddDays(7);
                    else break;
                }

                if (nextDate <= DateTime.Now.AddDays(7) && account.CurrentBalance < sub.Amount)
                {
                    upcoming.Add(sub);
                }
            }
            return upcoming;
        }


        public async Task UpdateSubscriptionAsync(SubscriptionModel subscription)
        {
            if (string.IsNullOrWhiteSpace(subscription.Name))
            {
                throw new ArgumentException("Subscription name cannot be empty.");
            }
            if (subscription.Amount < 0)
            {
                throw new ArgumentException("Amount cannot be negative.");
            }
            await Task.Run(() => _subscriptionRepo.UpdateSubscription(subscription));
        }

        public async Task DeleteSubscriptionAsync(Guid id)
        {
            await Task.Run(() => _subscriptionRepo.DeleteSubscription(id));
        }
    }
}
