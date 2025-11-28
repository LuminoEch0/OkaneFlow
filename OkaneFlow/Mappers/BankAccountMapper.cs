using DataAccessLayer.DataTransferObjects;
using OkaneFlow.Models;
using System.Linq;

namespace OkaneFlow.Mappers
{
    public static class BankAccountMapper
    {
        public static BankAccountModel ToModel(BankAccountDTO dto)
        {
            return new BankAccountModel(
                dto.AccountID,
                dto.UserID,
                dto.AccountName,
                dto.CurrentBalance);
        }
        public static List<BankAccountModel> ToModelList(IEnumerable<BankAccountDTO> dtos)
        {
            return dtos.Select(ToModel).ToList();
        }
        public static BankAccountDTO ToDTO(BankAccountModel model)
        {
            return new BankAccountDTO
            {
                AccountID = model.AccountID,
                UserID = model.UserID,
                AccountName = model.AccountName,
                CurrentBalance = model.CurrentBalance
            };  
        }
    }
}
