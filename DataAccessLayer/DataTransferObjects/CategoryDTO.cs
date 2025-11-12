using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.DataTransferObjects
{
    public class CategoryDTO
    {
        public Guid CategoryID { get; set; }

        public Guid AccountID { get; set; }

        public string? CategoryName { get; set; }

        public decimal AllocatedAmount { get; set; }

        public decimal AmountUsed { get; set; } //this is the amount used
    }
}
