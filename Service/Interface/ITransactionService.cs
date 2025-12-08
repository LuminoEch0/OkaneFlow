using DataAccessLayer.Repositories;
using Service.Mappers;
using Service.Models;

namespace Service.Interface
{
    public interface ITransactionService
    {
        public List<TransactionModel> GetTransactionsByAccountId(Guid accountId);

        public void CreateTransaction(TransactionModel transaction, Guid accountId);        

        public void DeleteTransaction(Guid id);

        public TransactionModel? GetTransactionById(Guid id);
        public void UpdateTransaction(TransactionModel transaction);
    }
}
