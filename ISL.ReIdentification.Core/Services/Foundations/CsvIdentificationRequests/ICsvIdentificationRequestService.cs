// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;

namespace ISL.ReIdentification.Core.Services.Foundations.CsvIdentificationRequests
{
    public interface ICsvIdentificationRequestService
    {
        ValueTask<CsvIdentificationRequest> AddCsvIdentificationRequestAsync(
            CsvIdentificationRequest csvIdentificationRequest);

        ValueTask<CsvIdentificationRequest> RetrieveCsvIdentificationRequestByIdAsync(
            Guid csvIdentificationRequestById);

        ValueTask<IQueryable<CsvIdentificationRequest>> RetrieveAllCsvIdentificationRequestsAsync();

        ValueTask<CsvIdentificationRequest> ModifyCsvIdentificationRequestAsync(
            CsvIdentificationRequest csvIdentificationRequest);

        ValueTask<CsvIdentificationRequest> RemoveCsvIdentificationRequestByIdAsync(
            Guid csvIdentificationRequestById);
    }
}
