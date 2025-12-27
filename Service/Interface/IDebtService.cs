using Service.Models;
using System;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface IDebtService
    {
        List<DebtModel> GetDebtsByUser(Guid userId);
        DebtModel? GetDebtById(Guid id);
        void CreateDebt(DebtModel debt);
        void UpdateDebt(DebtModel debt);
        void DeleteDebt(Guid id);
    }
}
