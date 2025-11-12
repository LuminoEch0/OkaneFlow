using DataAccessLayer.Repositories;
using OkaneFlow.Mappers;
using OkaneFlow.Models;

namespace OkaneFlow.Services.Dashboard
{

    public class CategoryService
    {
        private readonly CategoryRepository _repository;

        public CategoryService(CategoryRepository repository)
        {
            _repository = repository;
        }

        public List<CategoryModel> GetAllCategories(Guid id)
        {
            var dtos = _repository.GetCategories(id);
            return CategoryMapper.ToModelList(dtos);
        }

        public CategoryModel? GetCategoryById(Guid categoryId)
        {
            var dto = _repository.GetBankAccountById(categoryId);
            return dto == null ? null : CategoryMapper.ToModel(dto);
        }

        public void UpdateCategoryDetails(CategoryModel category)
        {
            var dto = CategoryMapper.ToDTO(category);
            _repository.UpdateCategory(dto);
        }

        public void DeleteCategory(Guid categoryId)
        {
            var account = GetCategoryById(categoryId);
            _repository.DeleteBankAccount(categoryId);
        }
        public void CreateAccount(CategoryModel category)
        {
            if (category.AllocatedAmount < 0 || category.AmountUsed < 0)
            {
                throw new ArgumentException("Allocated Amount or Amount used cannot be negative.");
            }

            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                throw new ArgumentException("Category name cannot be empty.");
            }

            //remeber to to pass teh account id somewhere

            var dto = CategoryMapper.ToDTO(category);
            _repository.CreateBankAccount(dto);
        }
    }
}