using Service.Models;
using System;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface ISubscriptionService
    {
        List<SubscriptionModel> GetSubscriptionsByAccount(Guid accountId);
        SubscriptionModel? GetSubscriptionById(Guid id);
        void CreateSubscription(SubscriptionModel subscription); // Logic for Category check here
        void UpdateSubscription(SubscriptionModel subscription);
        void DeleteSubscription(Guid id);
        List<SubscriptionModel> GetUpcomingInsufficientSubscriptions(Guid accountId);
    }
}
