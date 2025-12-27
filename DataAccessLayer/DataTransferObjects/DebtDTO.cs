using System;

namespace DataAccessLayer.DataTransferObjects
{
    public class DebtDTO
    {
        public Guid DebtID { get; set; }
        public Guid UserID { get; set; }
        public Guid? AccountID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AssociatedEntity { get; set; } = string.Empty;
        public decimal InitialAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public bool IsInterestEnabled { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime DueDate { get; set; }
        public int Type { get; set; }
    }
}
