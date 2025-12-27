using DataAccessLayer.DataTransferObjects;
using Service.Models;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer.Mappers
{
    public static class SubscriptionMapper
    {
        public static SubscriptionModel ToModel(SubscriptionDTO dto)
        {
            return new SubscriptionModel
            {
                SubscriptionID = dto.SubscriptionID,
                AccountID = dto.AccountID,
                CategoryID = dto.CategoryID,
                Name = dto.Name,
                Amount = dto.Amount,
                Frequency = dto.Frequency,
                StartDate = dto.StartDate,
                Description = dto.Description
            };
        }

        public static SubscriptionDTO ToDTO(SubscriptionModel model)
        {
            return new SubscriptionDTO
            {
                SubscriptionID = model.SubscriptionID,
                AccountID = model.AccountID,
                CategoryID = model.CategoryID,
                Name = model.Name,
                Amount = model.Amount,
                Frequency = model.Frequency,
                StartDate = model.StartDate,
                Description = model.Description
            };
        }

        public static List<SubscriptionModel> ToModelList(List<SubscriptionDTO> dtos)
        {
            return dtos.Select(ToModel).ToList();
        }
    }
}
