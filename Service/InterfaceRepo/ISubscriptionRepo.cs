using Service.Models;
using System;
using System.Collections.Generic;

namespace Service.RepoInterface
{
    public interface ISubscriptionRepo
    {
        List<SubscriptionModel> GetSubscriptions(Guid accountId);
        SubscriptionModel? GetSubscriptionById(Guid id);
        void CreateSubscription(SubscriptionModel model);
        void UpdateSubscription(SubscriptionModel model);
        void DeleteSubscription(Guid id);
        Task<int> GetSubscriptionCountByCategoryIdAsync(Guid categoryId);
    }
}
