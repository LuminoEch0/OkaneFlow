using DataAccessLayer.DataTransferObjects;
using OkaneFlow.Models;
using OkaneFlow.Helpers.Enums;

namespace OkaneFlow.Mappers
{
    public static class TransactionMapper
    {
        public static TransactionModel ToModel(TransactionDTO dto)
        {
            return new TransactionModel
            {
                TransactionID = dto.TransactionID,
                CategoryID = dto.CategoryID,
                Amount = dto.Amount,
                Description = dto.Description,
                Date = dto.Date,
                Type = (TransactionType)dto.Type
            };
        }

        public static TransactionDTO ToDTO(TransactionModel model)
        {
            return new TransactionDTO
            {
                TransactionID = model.TransactionID,
                CategoryID = model.CategoryID,
                Amount = model.Amount,
                Description = model.Description,
                Date = model.Date,
                Type = (int)model.Type
            };
        }
    }
}
