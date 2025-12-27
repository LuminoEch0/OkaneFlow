using Service.Interface;
using Service.Models;
using Service.RepoInterface;
using System;
using System.Collections.Generic;

namespace Service
{
    public class DebtService : IDebtService
    {
        private readonly IDebtRepo _debtRepo;

        public DebtService(IDebtRepo debtRepo)
        {
            _debtRepo = debtRepo;
        }

        public List<DebtModel> GetDebtsByUser(Guid userId)
        {
            return _debtRepo.GetDebts(userId);
        }

        public DebtModel? GetDebtById(Guid id)
        {
            return _debtRepo.GetDebtById(id);
        }

        public void CreateDebt(DebtModel debt)
        {
            if (string.IsNullOrWhiteSpace(debt.Name))
            {
                throw new ArgumentException("Debt name cannot be empty.");
            }
            if (debt.InitialAmount < 0 || debt.RemainingAmount < 0)
            {
                throw new ArgumentException("Amounts cannot be negative.");
            }
            _debtRepo.CreateDebt(debt);
        }

        public void UpdateDebt(DebtModel debt)
        {
            if (string.IsNullOrWhiteSpace(debt.Name))
            {
                throw new ArgumentException("Debt name cannot be empty.");
            }
            if (debt.InitialAmount < 0 || debt.RemainingAmount < 0)
            {
                throw new ArgumentException("Amounts cannot be negative.");
            }
            _debtRepo.UpdateDebt(debt);
        }

        public void DeleteDebt(Guid id)
        {
            _debtRepo.DeleteDebt(id);
        }
    }
}
