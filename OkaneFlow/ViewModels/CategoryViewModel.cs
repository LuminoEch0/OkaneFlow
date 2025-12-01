namespace OkaneFlow.ViewModels
{
    public class CategoryViewModel
    {

        public Guid CategoryID { get; set; }

        public Guid AccountID { get; set; }

        public string? CategoryName { get; set; }

        public decimal AllocatedAmount { get; set; }

        public decimal AmountUsed { get; set; } //this is the amount used

        public CategoryViewModel() { }

        public CategoryViewModel(Guid accountId, string? categoryName, decimal allocatedAmount, decimal amountUsed)
        {
            CategoryID = Guid.NewGuid();
            AccountID = accountId;
            CategoryName = categoryName;
            AllocatedAmount = allocatedAmount;
            AmountUsed = amountUsed;
        }
        public CategoryViewModel(Guid categoryId, Guid accountId, string? categoryName, decimal allocatedAmount, decimal amountUsed)
        {
            CategoryID = categoryId;
            AccountID = accountId;
            CategoryName = categoryName;
            AllocatedAmount = allocatedAmount;
            AmountUsed = amountUsed;
        }
        public CategoryViewModel(decimal allocatedAmount, decimal amountUsed)
        {
            AllocatedAmount = allocatedAmount;
            AmountUsed = amountUsed;
        }
    }
}
