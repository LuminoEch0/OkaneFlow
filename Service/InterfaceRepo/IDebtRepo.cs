using Service.Models;
using System;
using System.Collections.Generic;

namespace Service.RepoInterface
{
    public interface IDebtRepo
    {
        List<DebtModel> GetDebts(Guid userId);
        DebtModel? GetDebtById(Guid id);
        void CreateDebt(DebtModel model);
        void UpdateDebt(DebtModel model);
        void DeleteDebt(Guid id);
    }
}
