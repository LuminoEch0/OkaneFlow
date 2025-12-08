using DataAccessLayer.Repositories;
using DataAccessLayer.Repositories.Interface;
using Service.Interface;
using Service.Mappers;
using Service.Models;

namespace Service
{

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepo _repository;

        public CategoryService(ICategoryRepo repository)
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
            var dto = _repository.GetCategoryById(categoryId);
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
            _repository.DeleteCategory(categoryId);
        }
        public void CreateCategory(CategoryModel category)
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
            _repository.CreateCategory(dto);
        }

        public void AssignAmountToAllocate(Guid categoryId, decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Amount used cannot be negative.");
            }
            _repository.AssignAmountAllocated(categoryId, amount);
        }

        public decimal GetUnallocatedAmount(Guid accountId, IBankAccountService bankAccountService)
        {
            var categories = GetAllCategories(accountId);
            var totalAllocated = categories.Sum(c => c.AllocatedAmount);
            var account = bankAccountService.GetAccountById(accountId);
            if (account == null) return 0;
            return account.CurrentBalance - totalAllocated;
        }


    }
}