using OkaneFlow.ViewModels;
using Service.Models;
using System.Collections.Generic;
using System.Linq;

namespace OkaneFlow.Mappers
{
    public static class DebtMapper
    {
        public static DebtModel ToModel(DebtVM viewModel)
        {
            return new DebtModel
            {
                DebtID = viewModel.DebtID,
                UserID = viewModel.UserID,
                AccountID = viewModel.AccountID,
                Name = viewModel.Name,
                AssociatedEntity = viewModel.AssociatedEntity,
                InitialAmount = viewModel.InitialAmount,
                RemainingAmount = viewModel.RemainingAmount,
                IsInterestEnabled = viewModel.IsInterestEnabled,
                InterestRate = viewModel.InterestRate,
                DueDate = viewModel.DueDate,
                Type = (int)viewModel.Type
            };
        }

        public static List<DebtVM> ToViewModelList(this IEnumerable<DebtModel> models)
        {
            return models.Select(ToViewModel).ToList();
        }

        public static DebtVM ToViewModel(DebtModel model)
        {
            return new DebtVM
            {
                DebtID = model.DebtID,
                UserID = model.UserID,
                AccountID = model.AccountID,
                Name = model.Name,
                AssociatedEntity = model.AssociatedEntity,
                InitialAmount = model.InitialAmount,
                RemainingAmount = model.RemainingAmount,
                IsInterestEnabled = model.IsInterestEnabled,
                InterestRate = model.InterestRate,
                DueDate = model.DueDate,
                Type = (OkaneFlow.Helpers.Enums.DebtType)model.Type
            };
        }
    }
}
