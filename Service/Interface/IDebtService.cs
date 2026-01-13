using Service.Models;
using System;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface IDebtService
    {
        Task<List<DebtModel>> GetDebtsByUserAsync(Guid userId);
        Task<DebtModel?> GetDebtByIdAsync(Guid id);
        Task CreateDebtAsync(DebtModel debt);
        Task UpdateDebtAsync(DebtModel debt);
        Task DeleteDebtAsync(Guid id);
    }
}
