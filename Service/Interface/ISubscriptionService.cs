using Service.Models;
using System;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface ISubscriptionService
    {
        Task<List<SubscriptionModel>> GetSubscriptionsByAccountAsync(Guid accountId);
        Task<SubscriptionModel?> GetSubscriptionByIdAsync(Guid id);
        Task CreateSubscriptionAsync(SubscriptionModel subscription); // Logic for Category check here
        Task UpdateSubscriptionAsync(SubscriptionModel subscription);
        Task DeleteSubscriptionAsync(Guid id);
        Task<List<SubscriptionModel>> GetUpcomingInsufficientSubscriptionsAsync(Guid accountId);
    }
}
