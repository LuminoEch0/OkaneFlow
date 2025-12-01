using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service.Models
{
    public class TransactionModel
    {
        public Guid TransactionID { get; set; } // pk

        public Guid CategoryID { get; set; } // fk - category

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [StringLength(256)]
        public string? Description { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public int Type { get; set; } // enum 0:expense, 1:income

        public void ApplyTransaction(decimal amount, bool isExpense)
        {
            if (isExpense)
            {
                Amount -= amount;
            }
            else
            {
                Amount += amount;
            }
        }
    }
}
