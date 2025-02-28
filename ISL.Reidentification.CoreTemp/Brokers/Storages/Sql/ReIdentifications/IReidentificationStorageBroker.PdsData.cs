// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial interface IReIdentificationStorageBroker
    {
        ValueTask<PdsData> InsertPdsDataAsync(PdsData pdsData);
        ValueTask<IQueryable<PdsData>> SelectAllPdsDatasAsync();
        ValueTask<PdsData> SelectPdsDataByIdAsync(Guid pdsDataId);
        ValueTask<PdsData> UpdatePdsDataAsync(PdsData pdsData);
        ValueTask<PdsData> DeletePdsDataAsync(PdsData pdsData);
    }
}
