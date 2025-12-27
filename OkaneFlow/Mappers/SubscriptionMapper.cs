using OkaneFlow.ViewModels;
using Service.Models;
using System.Collections.Generic;
using System.Linq;

namespace OkaneFlow.Mappers
{
    public static class SubscriptionMapper
    {
        public static SubscriptionModel ToModel(SubscriptionVM viewModel)
        {
            return new SubscriptionModel
            {
                SubscriptionID = viewModel.SubscriptionID,
                AccountID = viewModel.AccountID,
                CategoryID = viewModel.CategoryID ?? Guid.Empty,
                Name = viewModel.Name,
                Amount = viewModel.Amount,
                Frequency = viewModel.Frequency,
                StartDate = viewModel.StartDate,
                Description = viewModel.Description
            };
        }

        public static List<SubscriptionVM> ToViewModelList(this IEnumerable<SubscriptionModel> models)
        {
            return models.Select(ToViewModel).ToList();
        }

        // Note: Model always has a CategoryID (Guid), so no null check needed here if model is valid.
        public static SubscriptionVM ToViewModel(SubscriptionModel model)
        {
            return new SubscriptionVM
            {
                SubscriptionID = model.SubscriptionID,
                AccountID = model.AccountID,
                CategoryID = model.CategoryID,
                Name = model.Name,
                Amount = model.Amount,
                Frequency = model.Frequency,
                StartDate = model.StartDate,
                Description = model.Description
            };
        }
    }
}
