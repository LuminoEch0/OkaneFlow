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
        private readonly Service.Interface.ICurrentUserService _currentUserService;

        public DebtService(IDebtRepo debtRepo, Service.Interface.ICurrentUserService currentUserService)
        {
            _debtRepo = debtRepo;
            _currentUserService = currentUserService;
        }

        public async Task<List<DebtModel>> GetDebtsByUserAsync(Guid userId)
        {
            if (userId != _currentUserService.UserGuid) throw new UnauthorizedAccessException();
            return await _debtRepo.GetDebtsAsync(userId);
        }

        public async Task<DebtModel?> GetDebtByIdAsync(Guid id)
        {
            var debt = await _debtRepo.GetDebtByIdAsync(id);
            if (debt != null && debt.UserID != _currentUserService.UserGuid) return null; // Or throw
            return debt;
        }

        public async Task CreateDebtAsync(DebtModel debt)
        {
            if (string.IsNullOrWhiteSpace(debt.Name))
            {
                throw new ArgumentException("Debt name cannot be empty.");
            }
            if (debt.InitialAmount < 0 || debt.RemainingAmount < 0)
            {
                throw new ArgumentException("Amounts cannot be negative.");
            }
            if (debt.UserID != _currentUserService.UserGuid)
            {
                throw new UnauthorizedAccessException("Cannot create debt for another user.");
            }
            await _debtRepo.CreateDebtAsync(debt);
        }

        public async Task UpdateDebtAsync(DebtModel debt)
        {
            if (string.IsNullOrWhiteSpace(debt.Name))
            {
                throw new ArgumentException("Debt name cannot be empty.");
            }
            if (debt.InitialAmount < 0 || debt.RemainingAmount < 0)
            {
                throw new ArgumentException("Amounts cannot be negative.");
            }
            var existing = await _debtRepo.GetDebtByIdAsync(debt.DebtID);
            if (existing == null || existing.UserID != _currentUserService.UserGuid)
            {
                throw new UnauthorizedAccessException("Cannot update debt.");
            }
            // Ensure ID matches
            debt.UserID = _currentUserService.UserGuid;
            await _debtRepo.UpdateDebtAsync(debt);
        }

        public async Task DeleteDebtAsync(Guid id)
        {
            var existing = await _debtRepo.GetDebtByIdAsync(id);
            if (existing == null || existing.UserID != _currentUserService.UserGuid)
            {
                throw new UnauthorizedAccessException("Cannot delete debt.");
            }
            await _debtRepo.DeleteDebtAsync(id);
        }
    }
}
