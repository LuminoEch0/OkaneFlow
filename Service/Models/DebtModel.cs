using System;

namespace Service.Models
{
    public class DebtModel
    {
        public Guid DebtID { get; set; }
        public Guid UserID { get; set; }
        public Guid? AccountID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AssociatedEntity { get; set; } = string.Empty; // Person/Business
        public decimal InitialAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public bool IsInterestEnabled { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime DueDate { get; set; }
        public int Type { get; set; } // 0: OwedByMe, 1: OwedToMe

        public DebtModel() { }

        public DebtModel(Guid userId, Guid? accountId, string name, string associatedEntity, decimal initialAmount, decimal remainingAmount, bool isInterestEnabled, decimal interestRate, DateTime dueDate, int type)
        {
            DebtID = Guid.NewGuid();
            UserID = userId;
            AccountID = accountId;
            Name = name;
            AssociatedEntity = associatedEntity;
            InitialAmount = initialAmount;
            RemainingAmount = remainingAmount;
            IsInterestEnabled = isInterestEnabled;
            InterestRate = interestRate;
            DueDate = dueDate;
            Type = type;
        }
    }
}
