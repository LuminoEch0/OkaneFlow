using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class TransactionTypeLookupService
    {
        private readonly TransactionTypeLookupRepo _lookupRepo;
        public TransactionTypeLookupService(TransactionTypeLookupRepo lookupRepo)
        {
            _lookupRepo = lookupRepo;
        }

        public int GetTypeIdFromName(string typeName)
        {
            // 1. Get all types from the DAL (if not cached)
            var allTypes = _lookupRepo.GetAllTransactionTypesAsync();

            // 2. Find the ID matching the name
            var lookup = allTypes.FirstOrDefault(t => t.TypeName.Equals(typeName, StringComparison.OrdinalIgnoreCase));

            if (lookup == null)
            {
                throw new ArgumentException($"Invalid category type: {typeName}");
            }
            return lookup.TypeID;
        }

        public string GetTypeNameFromId(int typeId)
        {
            // 1. Get all types from the DAL (if not cached)
            var allTypes = _lookupRepo.GetAllTransactionTypesAsync();
            // 2. Find the name matching the ID
            var lookup = allTypes.FirstOrDefault(t => t.TypeID == typeId);
            if (lookup == null)
            {
                throw new ArgumentException($"Invalid category type ID: {typeId}");
            }
            return lookup.TypeName;
        }
    }
}
