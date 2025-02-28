// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial interface IReIdentificationStorageBroker
    {
        ValueTask<CsvIdentificationRequest> InsertCsvIdentificationRequestAsync(
            CsvIdentificationRequest csvIdentificationRequest);

        ValueTask<IQueryable<CsvIdentificationRequest>> SelectAllCsvIdentificationRequestsAsync();

        ValueTask<CsvIdentificationRequest> SelectCsvIdentificationRequestByIdAsync(
            Guid csvIdentificationRequestId);

        ValueTask<CsvIdentificationRequest> UpdateCsvIdentificationRequestAsync(
            CsvIdentificationRequest csvIdentificationRequest);

        ValueTask<CsvIdentificationRequest> DeleteCsvIdentificationRequestAsync(
            CsvIdentificationRequest csvIdentificationRequest);
    }
}
