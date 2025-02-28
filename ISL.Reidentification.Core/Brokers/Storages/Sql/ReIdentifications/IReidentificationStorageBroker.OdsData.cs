// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial interface IReIdentificationStorageBroker
    {
        ValueTask<OdsData> InsertOdsDataAsync(OdsData odsData);
        ValueTask<IQueryable<OdsData>> SelectAllOdsDatasAsync();
        ValueTask<OdsData> SelectOdsDataByIdAsync(Guid odsDataId);
        ValueTask<OdsData> UpdateOdsDataAsync(OdsData odsData);
        ValueTask<OdsData> DeleteOdsDataAsync(OdsData odsData);
    }
}
