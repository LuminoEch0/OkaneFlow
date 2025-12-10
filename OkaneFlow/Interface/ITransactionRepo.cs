using DataAccessLayer.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Interface
{
    public interface ITransactionRepo
    {
        public List<TransactionDTO> GetTransactionsByAccountId(Guid accountId);

        public void AddTransaction(TransactionDTO transaction);

        public TransactionDTO? GetTransactionById(Guid id);

        public void UpdateTransaction(TransactionDTO transaction);

        public void DeleteTransaction(Guid id);
    }
}
