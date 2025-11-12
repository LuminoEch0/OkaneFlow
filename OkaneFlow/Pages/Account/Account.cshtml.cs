using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace OkaneFlow.Pages.Account
{
    public class AccountModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public AccountModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        //public void OnGet()
        //{
        //    string connectionString = _configuration.GetConnectionString("DefaultConnection");

        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        string sql = "SELECT Id, Name, Description FROM Test";
        //        using (SqlCommand cmd = new SqlCommand(sql, conn))
        //        {
        //            conn.Open();
        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    TestItems.Add(new TestItem
        //                    {
        //                        Id = reader.GetInt32(0),
        //                        Name = reader.GetString(1),
        //                        Description = reader.GetString(2)
        //                    });
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
