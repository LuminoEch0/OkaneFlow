using OkaneFlow.Helpers.Enums;
using System;

namespace OkaneFlow.ViewModels
{
    public class DebtVM
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
        public DebtType Type { get; set; } // 0: OwedByMe, 1: OwedToMe

        public DebtVM() { }

        public DebtVM(Guid userId, Guid? accountId, string name, string associatedEntity, decimal initialAmount, decimal remainingAmount, bool isInterestEnabled, decimal interestRate, DateTime dueDate, DebtType type)
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
