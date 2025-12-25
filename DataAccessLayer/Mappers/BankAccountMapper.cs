using DataAccessLayer.DataTransferObjects;
using Service.Models;
using System.Linq;

namespace DataAccessLayer.Mappers
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
        public static List<BankAccountDTO> ToDTOList(IEnumerable<BankAccountModel> model)
        {
            return model.Select(ToDTO).ToList();
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
