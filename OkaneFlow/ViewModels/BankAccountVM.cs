using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OkaneFlow.ViewModels
{
    public class BankAccountVM
    {
        public Guid AccountID { get; set; } //pk

        public Guid UserID { get; set; } //fk - user

        [Required]
        [StringLength(100)]
        public string? AccountName { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal CurrentBalance { get; set; } = 0.00m;

        public BankAccountVM() { }

        public BankAccountVM(Guid userId, string? accountName, decimal currentBalance)
        {
            AccountID = Guid.NewGuid();
            UserID = userId;
            AccountName = accountName;
            CurrentBalance = currentBalance;
        }
        public BankAccountVM(Guid accountId, Guid userId, string? accountName, decimal currentBalance)
        {
            AccountID = accountId;
            UserID = userId;
            AccountName = accountName;
            CurrentBalance = currentBalance;
        }
    }
}
