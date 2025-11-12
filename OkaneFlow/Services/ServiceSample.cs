//using OkaneFlow.Models;
//using OkaneFlow.Pages;
//using Microsoft.Data.SqlClient;
//using OkaneFlow.Data;
//using System.Data;

//namespace OkaneFlow.Services
//{
//    // Service to handle all Debt and DebtorRecord operations
//    public class DebtService
//    {
//        // Use the Interface for dependency injection
//        private readonly IDbConnectionManager _dbManager;

//        // SQL statements kept as constants for maintainability
//        private const string SQL_GET_BY_ID = "SELECT * FROM Debts WHERE DebtID = @DebtId";
//        private const string SQL_INSERT =
//            "INSERT INTO Debts (DebtID, AccountID, Description, TotalOriginalBillAmount, DatePaid) " +
//            "VALUES (@DebtId, @AccountId, @Desc, @TotalAmount, @DatePaid)";
//        private const string SQL_UPDATE_SETTLEMENT =
//            "UPDATE Debts SET IsFullySettled = @IsSettled WHERE DebtID = @DebtId";

//        public DebtService(IDbConnectionManager dbManager)
//        {
//            _dbManager = dbManager;
//        }

//        /// <summary>
//        /// Retrieves a single Debt record by its ID.
//        /// </summary>
//        public Debt GetDebtById(Guid debtId)
//        {
//            using (IDbConnection connection = _dbManager.GetOpenConnection())
//            using (IDbCommand command = new SqlCommand(SQL_GET_BY_ID, (SqlConnection)connection))
//            {
//                command.Parameters.Add(new SqlParameter("@DebtId", debtId));

//                using (IDataReader reader = command.ExecuteReader())
//                {
//                    if (reader.Read())
//                    {
//                        // Manual mapping of results to the Debt Model
//                        return new Debt
//                        {
//                            //DebtID = reader.GetGuid(reader.GetOrdinal("DebtID")),
//                            //AccountID = reader.GetGuid(reader.GetOrdinal("AccountID")),
//                            //Description = reader.GetString(reader.GetOrdinal("Description")),
//                            //TotalOriginalBillAmount = reader.GetDecimal(reader.GetOrdinal("TotalOriginalBillAmount")),
//                            //DatePaid = reader.GetDateTime(reader.GetOrdinal("DatePaid")),
//                            //IsFullySettled = reader.GetBoolean(reader.GetOrdinal("IsFullySettled"))
//                        };
//                    }
//                    return null; // Debt not found
//                }
//            }
//        }

//        /// <summary>
//        /// Inserts a new Debt record into the database.
//        /// </summary>
//        public void CreateDebt(Debt newDebt)
//        {
//            // Use Guid.NewGuid() to create a unique ID before inserting
//            newDebt.DebtID = Guid.NewGuid();

//            using (IDbConnection connection = _dbManager.GetOpenConnection())
//            using (IDbCommand command = new SqlCommand(SQL_INSERT, (SqlConnection)connection))
//            {
//                // Parameterized query to prevent SQL injection (CRITICAL)
//                command.Parameters.Add(new SqlParameter("@DebtId", newDebt.DebtID));
//                command.Parameters.Add(new SqlParameter("@AccountId", newDebt.AccountID));
//                command.Parameters.Add(new SqlParameter("@Desc", newDebt.Description));
//                command.Parameters.Add(new SqlParameter("@TotalAmount", newDebt.TotalOriginalBillAmount));
//                command.Parameters.Add(new SqlParameter("@DatePaid", newDebt.DatePaid));

//                command.ExecuteNonQuery();
//            }
//        }

//        // Additional methods would be here: UpdateDebt, DeleteDebt, GetDebtsByAccount, etc.
//    }
//}
