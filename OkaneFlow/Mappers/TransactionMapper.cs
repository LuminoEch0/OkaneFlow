using Service.Models;
using OkaneFlow.ViewModels;
using OkaneFlow.Helpers.Enums;
using System.Collections.Generic;
using System.Linq;

namespace OkaneFlow.Mappers
{
    public static class TransactionMapper
    {
        public static TransactionModel ToModel(TransactionVM viewModel)
        {
            return new TransactionModel
            {
                TransactionID = viewModel.TransactionID,
                CategoryID = viewModel.CategoryID,
                Amount = viewModel.Amount,
                Description = viewModel.Description,
                Date = viewModel.Date,
                Type = (int)viewModel.Type
            };
        }

        public static TransactionVM ToViewModel(TransactionModel model)
        {
            return new TransactionVM
            {
                TransactionID = model.TransactionID,
                CategoryID = model.CategoryID,
                Amount = model.Amount,
                Description = model.Description,
                Date = model.Date,
                Type = (TransactionType)model.Type
            };
        }

        public static List<TransactionVM> ToViewModelList(IEnumerable<TransactionModel> models)
        {
            return models.Select(ToViewModel).ToList();
        }

        public static List<TransactionModel> ToModelList(IEnumerable<TransactionVM> viewModels)
        {
            return viewModels.Select(ToModel).ToList();
        }
    }
}
