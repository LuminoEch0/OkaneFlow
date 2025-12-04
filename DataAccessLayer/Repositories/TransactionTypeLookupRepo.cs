using DataAccessLayer.DataTransferObjects;
using Microsoft.Data.SqlClient;
using System.Data;


namespace DataAccessLayer.Repositories
{
    public class TransactionTypeLookupRepo
    {
        private readonly ConnectionManager _dbManager;

        public TransactionTypeLookupRepo(ConnectionManager dbManager)
        {
            _dbManager = dbManager;
        }
        public List<TransactionTypeLookupDTO> GetAllTransactionTypesAsync()
        {
            var transactionTypes = new List<TransactionTypeLookupDTO>();
            string sql = "SELECT * FROM TransactionTypeLookup";

            using (IDbConnection connection = _dbManager.GetOpenConnection())
            {
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactionTypes.Add(new TransactionTypeLookupDTO
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
