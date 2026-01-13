using Service.Models;

namespace Service.Interface
{

    public interface ICategoryService
    {
        public Task<List<CategoryModel>> GetAllCategoriesAsync(Guid id);

        public Task<CategoryModel?> GetCategoryByIdAsync(Guid categoryId);

        public Task UpdateCategoryDetailsAsync(CategoryModel category);

        public Task DeleteCategoryAsync(Guid categoryId);
        public Task CreateCategoryAsync(CategoryModel category);

        public Task AssignAmountToAllocateAsync(Guid categoryId, decimal amount);
        public Task<decimal> GetUnallocatedAmountAsync(Guid accountId, IBankAccountService bankAccountService);
    }
}