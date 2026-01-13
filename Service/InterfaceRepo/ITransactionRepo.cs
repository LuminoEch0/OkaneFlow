using Service.Models;

namespace Service.RepoInterface
{
    public interface ITransactionRepo
    {
        public Task<List<TransactionModel>> GetTransactionsByAccountIdAsync(Guid accountId);

        public Task AddTransactionAsync(TransactionModel transaction);

        public Task<TransactionModel?> GetTransactionByIdAsync(Guid id);

        public Task UpdateTransactionAsync(TransactionModel transaction);

        public Task DeleteTransactionAsync(Guid id);

        public Task UpdateCategoryForTransactionsAsync(Guid oldCategoryId, Guid newCategoryId, int expenseTypeId);
    }
}
