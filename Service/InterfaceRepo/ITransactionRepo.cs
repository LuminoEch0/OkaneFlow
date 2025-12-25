using Service.Models;

namespace Service.RepoInterface
{
    public interface ITransactionRepo
    {
        public List<TransactionModel> GetTransactionsByAccountId(Guid accountId);

        public void AddTransaction(TransactionModel transaction);

        public TransactionModel? GetTransactionById(Guid id);

        public void UpdateTransaction(TransactionModel transaction);

        public void DeleteTransaction(Guid id);
    }
}
