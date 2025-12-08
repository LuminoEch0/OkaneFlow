using DataAccessLayer.DataTransferObjects;
using Microsoft.Data.SqlClient;
using System.Data;


namespace DataAccessLayer.Repositories.Interface
{
    public interface ITransactionTypeLookupRepo
    {
        public List<TransactionTypeLookupDTO> GetAllTransactionTypesAsync();
    }
}
