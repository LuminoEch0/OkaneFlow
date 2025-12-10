using DataAccessLayer.DataTransferObjects;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataAccessLayer.Repositories.Interface
{
    public interface ICategoryRepo
    {
        public List<CategoryDTO> GetCategories(Guid id);

        public CategoryDTO? GetCategoryById(Guid id);

        public void UpdateCategory(CategoryDTO dto);

        public void DeleteCategory(Guid id);

        public void CreateCategory(CategoryDTO dto);

        public void AssignAmountAllocated(Guid categoryId, decimal amount);

        public CategoryDTO GetUnassignedCategory(Guid accountId);
    }
}
