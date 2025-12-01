using OkaneFlow.ViewModels;
using Service.Models;

namespace OkaneFlow.Mappers
{
    public static class CategoryMapper
    {
        public static CategoryModel ToModel(CategoryViewModel viewModel)
        {
            return new CategoryModel(
                viewModel.CategoryID,
                viewModel.AccountID,
                viewModel.CategoryName,
                viewModel.AllocatedAmount,
                viewModel.AmountUsed);
        }
        public static List<CategoryViewModel> ToViewModelList(this IEnumerable<CategoryModel> models)
        {
            return models.Select(ToViewModel).ToList();
        }
        public static CategoryViewModel ToViewModel(CategoryModel model)
        {
            return new CategoryViewModel
            {
                CategoryID = model.CategoryID,
                AccountID = model.AccountID,
                CategoryName = model.CategoryName,
                AllocatedAmount = model.AllocatedAmount,
                AmountUsed = model.AmountUsed
            };
        }
    }
}
