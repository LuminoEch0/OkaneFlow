using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service.Models
{
    public class BankAccountModel
    {
        public Guid AccountID { get; set; } //pk

        public Guid UserID { get; set; } //fk - user

        [Required]
        [StringLength(100)]
        public string? AccountName { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal CurrentBalance { get; private set; } = 0.00m;

        public BankAccountModel() { }

        public BankAccountModel(string? accountName, decimal currentBalance)
        {
            AccountID = Guid.NewGuid();
            UserID = Guid.NewGuid();
            AccountName = accountName;
            CurrentBalance = currentBalance;
        }
        public BankAccountModel(Guid accountId, Guid userId, string? accountName, decimal currentBalance)
        {
            AccountID = accountId;
            UserID = userId;
            AccountName = accountName;
            CurrentBalance = currentBalance;
        }
        public BankAccountModel(decimal currentBalance)
        {
            currentBalance = CurrentBalance;
        }

        public void UpdateBalance(decimal amount)
        {
            CurrentBalance += amount;
        }
    }
}
