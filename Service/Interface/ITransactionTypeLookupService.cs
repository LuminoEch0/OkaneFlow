namespace Service.Interface
{
    public interface ITransactionTypeLookupService
    {
        public Task<int> GetTypeIdFromNameAsync(string typeName);
        public Task<string> GetTypeNameFromIdAsync(int typeId);
    }
}
