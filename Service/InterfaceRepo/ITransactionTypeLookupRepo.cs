using Service.Models;


namespace Service.RepoInterface
{
    public interface ITransactionTypeLookupRepo
    {
        public List<TransactionTypeLookupModel> GetAllTransactionTypesAsync();
    }
}
