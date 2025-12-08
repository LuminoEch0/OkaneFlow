namespace Service.Interface
{
    public interface ITransactionTypeLookupService
    {
        public int GetTypeIdFromName(string typeName);

        public string GetTypeNameFromId(int typeId);
    }
}
