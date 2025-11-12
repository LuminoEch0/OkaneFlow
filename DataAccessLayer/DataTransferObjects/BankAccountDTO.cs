using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccessLayer.DataTransferObjects
{
    public class BankAccountDTO
    {
        public Guid AccountID { get; set; } //pk

        public Guid UserID { get; set; } //fk - user

        [Required]
        [StringLength(100)]
        public string? AccountName { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal CurrentBalance { get; set; } = 0.00m;
    }
}
