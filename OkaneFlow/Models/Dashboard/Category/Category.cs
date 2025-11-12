using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OkaneFlow.Models.Dashboard.Category
{
    public class Category
    {
        public Guid CategoryID { get; set; } // pk

        public Guid AccountID { get; set; } // fk - bank account

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal AllocatedAmount { get; set; } = 0.00m;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal CurrentBalance { get; private set; } = 0.00m;

        public void ApplyTransaction(decimal amount, bool isExpense)
        {
            if (isExpense)
            {
                CurrentBalance -= amount;
            }
            else
            {
                CurrentBalance += amount;
            }
        }
    }
}
