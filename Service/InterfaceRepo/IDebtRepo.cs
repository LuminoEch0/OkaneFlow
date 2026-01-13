using Service.Models;
using System;
using System.Collections.Generic;

namespace Service.RepoInterface
{
    public interface IDebtRepo
    {
        Task<List<DebtModel>> GetDebtsAsync(Guid userId);
        Task<DebtModel?> GetDebtByIdAsync(Guid id);
        Task CreateDebtAsync(DebtModel model);
        Task UpdateDebtAsync(DebtModel model);
        Task DeleteDebtAsync(Guid id);
    }
}
