using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.DataTransferObjects
{
    public class TransactionDTO
    {
        public Guid TransactionID { get; set; } // pk

        public Guid CategoryID { get; set; } // fk - category

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [StringLength(256)]
        public string? Description { get; set; }

        public DateTime Date { get; set; }

        public int Type { get; set; } // enum 0:expense, 1:income
    }
}
