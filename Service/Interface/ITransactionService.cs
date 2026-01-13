using Service.Models;

namespace Service.Interface
{
    public interface ITransactionService
    {
        public Task<List<TransactionModel>> GetTransactionsByAccountIdAsync(Guid accountId);

        public Task CreateTransactionAsync(TransactionModel transaction, Guid accountId);

        public Task DeleteTransactionAsync(Guid id);

        public Task<TransactionModel?> GetTransactionByIdAsync(Guid id);
        public Task UpdateTransactionAsync(TransactionModel transaction);
    }
}
