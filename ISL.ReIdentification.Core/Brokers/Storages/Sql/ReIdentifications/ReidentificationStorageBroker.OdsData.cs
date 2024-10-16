// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using Microsoft.EntityFrameworkCore;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial class ReIdentificationStorageBroker
    {
        public DbSet<OdsData> OdsDatas { get; set; }

        public async ValueTask<OdsData> InsertOdsDataAsync(OdsData odsData) =>
            await InsertAsync(odsData);

        public async ValueTask<IQueryable<OdsData>> SelectAllOdsDatasAsync() =>
            await SelectAllAsync<OdsData>();

        public async ValueTask<OdsData> SelectOdsDataByIdAsync(Guid odsDataId) =>
            await SelectAsync<OdsData>(odsDataId);
        public async ValueTask<OdsData> UpdateOdsDataAsync(OdsData odsData) =>
            await UpdateAsync(odsData);

        public async ValueTask<OdsData> DeleteOdsDataAsync(OdsData odsData) =>
            await DeleteAsync(odsData);
    }
}