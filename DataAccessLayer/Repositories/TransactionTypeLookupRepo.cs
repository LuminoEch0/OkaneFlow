using DataAccessLayer.DataTransferObjects;
using Service.RepoInterface;
using Service.Models;
using Microsoft.Data.SqlClient;
using System.Data;


namespace DataAccessLayer.Repositories
{
    public class TransactionTypeLookupRepo : ITransactionTypeLookupRepo
    {
        private readonly ConnectionManager _dbManager;

        public TransactionTypeLookupRepo(ConnectionManager dbManager)
        {
            _dbManager = dbManager;
        }
        public List<TransactionTypeLookupModel> GetAllTransactionTypesAsync()
        {
            var transactionTypes = new List<TransactionTypeLookupModel>();
            string sql = "SELECT * FROM TransactionTypeLookup";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactionTypes.Add(new TransactionTypeLookupModel
                            {
                                TypeID = reader.GetInt32(reader.GetOrdinal("TypeID")),
                                TypeName = reader.GetString(reader.GetOrdinal("TypeName")),
                            });
                        }
                    }
                }
            }
            if (transactionTypes.Count == 0)
            {
                throw new InvalidOperationException("No transaction types found in the database.");
            }
            return transactionTypes;
        }
    }
}
