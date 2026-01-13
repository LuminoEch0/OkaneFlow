using Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Service.RepoInterface
{
    public interface ICategoryRepo
    {
        public Task<List<CategoryModel>> GetCategoriesAsync(Guid id);

        public Task<CategoryModel?> GetCategoryByIdAsync(Guid id);

        public Task UpdateCategoryAsync(CategoryModel dto);

        public Task DeleteCategoryAsync(Guid id);

        public Task CreateCategoryAsync(CategoryModel dto);

        public Task AssignAmountAllocatedAsync(Guid categoryId, decimal amount);

        public Task<CategoryModel> GetUnallocatedCategoryAsync(Guid accountId);
    }
}
