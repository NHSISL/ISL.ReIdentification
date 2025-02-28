// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using Microsoft.EntityFrameworkCore;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial class ReIdentificationStorageBroker
    {
        public DbSet<CsvIdentificationRequest> CsvIdentificationRequests { get; set; }

        public async ValueTask<CsvIdentificationRequest> InsertCsvIdentificationRequestAsync(
            CsvIdentificationRequest csvIdentificationRequest) =>
                await InsertAsync(csvIdentificationRequest);

        public async ValueTask<IQueryable<CsvIdentificationRequest>> SelectAllCsvIdentificationRequestsAsync() =>
            await SelectAllAsync<CsvIdentificationRequest>();

        public async ValueTask<CsvIdentificationRequest> SelectCsvIdentificationRequestByIdAsync(
            Guid csvIdentificationRequestId) =>
                await SelectAsync<CsvIdentificationRequest>(csvIdentificationRequestId);

        public async ValueTask<CsvIdentificationRequest> UpdateCsvIdentificationRequestAsync(
            CsvIdentificationRequest csvIdentificationRequest) =>
                await UpdateAsync(csvIdentificationRequest);

        public async ValueTask<CsvIdentificationRequest> DeleteCsvIdentificationRequestAsync(
            CsvIdentificationRequest csvIdentificationRequest) =>
                await DeleteAsync(csvIdentificationRequest);
    }
}