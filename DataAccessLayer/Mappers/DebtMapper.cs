using DataAccessLayer.DataTransferObjects;
using Service.Models;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer.Mappers
{
    public static class DebtMapper
    {
        public static DebtModel ToModel(DebtDTO dto)
        {
            return new DebtModel
            {
                DebtID = dto.DebtID,
                UserID = dto.UserID,
                AccountID = dto.AccountID,
                Name = dto.Name,
                AssociatedEntity = dto.AssociatedEntity,
                InitialAmount = dto.InitialAmount,
                RemainingAmount = dto.RemainingAmount,
                IsInterestEnabled = dto.IsInterestEnabled,
                InterestRate = dto.InterestRate,
                DueDate = dto.DueDate,
                Type = dto.Type
            };
        }

        public static DebtDTO ToDTO(DebtModel model)
        {
            return new DebtDTO
            {
                DebtID = model.DebtID,
                UserID = model.UserID,
                AccountID = model.AccountID,
                Name = model.Name,
                AssociatedEntity = model.AssociatedEntity,
                InitialAmount = model.InitialAmount,
                RemainingAmount = model.RemainingAmount,
                IsInterestEnabled = model.IsInterestEnabled,
                InterestRate = model.InterestRate,
                DueDate = model.DueDate,
                Type = model.Type
            };
        }

        public static List<DebtModel> ToModelList(List<DebtDTO> dtos)
        {
            return dtos.Select(ToModel).ToList();
        }
    }
}
