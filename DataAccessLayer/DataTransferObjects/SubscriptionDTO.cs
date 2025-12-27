using System;

namespace DataAccessLayer.DataTransferObjects
{
    public class SubscriptionDTO
    {
        public Guid SubscriptionID { get; set; }
        public Guid AccountID { get; set; }
        public Guid CategoryID { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Frequency { get; set; } = string.Empty; // e.g. "Monthly", "Yearly"
        public DateTime StartDate { get; set; }
        public string? Description { get; set; }
    }
}
