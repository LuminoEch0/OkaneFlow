using Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Service.RepoInterface
{
    public interface ICategoryRepo
    {
        public List<CategoryModel> GetCategories(Guid id);

        public CategoryModel? GetCategoryById(Guid id);

        public void UpdateCategory(CategoryModel dto);

        public void DeleteCategory(Guid id);

        public void CreateCategory(CategoryModel dto);

        public void AssignAmountAllocated(Guid categoryId, decimal amount);

        public CategoryModel GetUnassignedCategory(Guid accountId);
    }
}
