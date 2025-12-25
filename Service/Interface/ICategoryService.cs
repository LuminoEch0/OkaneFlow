using Service.Models;

namespace Service.Interface
{

    public interface ICategoryService
    {
        public List<CategoryModel> GetAllCategories(Guid id);

        public CategoryModel? GetCategoryById(Guid categoryId);

        public void UpdateCategoryDetails(CategoryModel category);

        public void DeleteCategory(Guid categoryId);
        public void CreateCategory(CategoryModel category);

        public void AssignAmountToAllocate(Guid categoryId, decimal amount);
        public decimal GetUnallocatedAmount(Guid accountId, IBankAccountService bankAccountService);
    }
}