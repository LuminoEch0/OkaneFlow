using OkaneFlow.ViewModels;
using Service.Models;

namespace OkaneFlow.Mappers
{
    public static class BankAccountMapper
    {
        public static BankAccountModel ToModel(BankAccountVM dto)
        {
            return new BankAccountModel(
                dto.AccountID,
                dto.UserID,
                dto.AccountName,
                dto.CurrentBalance);
        }
        public static List<BankAccountVM> ToViewModelList(this IEnumerable<BankAccountModel> dtos)
        {
            return dtos.Select(ToViewModel).ToList();
        }
        public static BankAccountVM ToViewModel(BankAccountModel model)
        {
            return new BankAccountVM
            {
                AccountID = model.AccountID,
                UserID = model.UserID,
                AccountName = model.AccountName,
                CurrentBalance = model.CurrentBalance
            };
        }
    }
}
