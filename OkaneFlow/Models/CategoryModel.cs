using Microsoft.Identity.Client;

namespace OkaneFlow.Models
{
    public class CategoryModel
    {

        public Guid CategoryID { get; set; }

        public Guid AccountID { get; set; }

        public string? CategoryName { get; set; }

        public decimal AllocatedAmount { get; private set; }

        public decimal AmountUsed { get; private set; } //this is the amount used

        public CategoryModel() { }

        public CategoryModel(Guid accountId, string? categoryName, decimal allocatedAmount, decimal amountUsed)
        {
            CategoryID = Guid.NewGuid();
            AccountID = accountId;
            CategoryName = categoryName;
            AllocatedAmount = allocatedAmount;
            AmountUsed = amountUsed;
        }
        public CategoryModel(Guid categoryId, Guid accountId, string? categoryName, decimal allocatedAmount, decimal amountUsed)
        {
            CategoryID = categoryId;
            AccountID = accountId;
            CategoryName = categoryName;
            AllocatedAmount = allocatedAmount;
            AmountUsed = amountUsed;
        }
        public CategoryModel(decimal allocatedAmount, decimal amountUsed)
        {
            AllocatedAmount = allocatedAmount;
            AmountUsed = amountUsed;
        }

        public void UpdateAllocatedAmount(decimal amount)
        {
            AllocatedAmount = amount;
        }
    }
}
