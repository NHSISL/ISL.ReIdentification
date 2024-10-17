// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using Microsoft.EntityFrameworkCore;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial class ReIdentificationStorageBroker
    {
        public DbSet<PdsData> PdsDatas { get; set; }

        public async ValueTask<PdsData> InsertPdsDataAsync(PdsData pdsData) =>
            await InsertAsync(pdsData);

        public async ValueTask<IQueryable<PdsData>> SelectAllPdsDatasAsync() =>
            await SelectAllAsync<PdsData>();

        public async ValueTask<PdsData> SelectPdsDataByIdAsync(Guid pdsDataId) =>
            await SelectAsync<PdsData>(pdsDataId);

        public async ValueTask<PdsData> UpdatePdsDataAsync(PdsData pdsData) =>
            await UpdateAsync(pdsData);

        public async ValueTask<PdsData> DeletePdsDataAsync(PdsData pdsData) =>
            await DeleteAsync(pdsData);
    }
}