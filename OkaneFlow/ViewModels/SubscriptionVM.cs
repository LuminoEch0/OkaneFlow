using System;

namespace OkaneFlow.ViewModels
{
    public class SubscriptionVM
    {
        public Guid SubscriptionID { get; set; }
        public Guid AccountID { get; set; }
        public Guid? CategoryID { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Frequency { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string? Description { get; set; }

        public SubscriptionVM() { }

        public SubscriptionVM(Guid accountId, Guid? categoryId, string name, decimal amount, string frequency, DateTime startDate, string? description)
        {
            SubscriptionID = Guid.NewGuid();
            AccountID = accountId;
            CategoryID = categoryId;
            Name = name;
            Amount = amount;
            Frequency = frequency;
            StartDate = startDate;
            Description = description;
        }
    }
}
