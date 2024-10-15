// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;

namespace ISL.ReIdentification.Core.Services.Foundations.CsvIdentificationRequests
{
    public partial class CsvIdentificationRequestService : ICsvIdentificationRequestService
    {
        private readonly IReIdentificationStorageBroker reIdentificationStorageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public CsvIdentificationRequestService(
            IReIdentificationStorageBroker reIdentificationStorageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.reIdentificationStorageBroker = reIdentificationStorageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }
        public ValueTask<CsvIdentificationRequest> AddCsvIdentificationRequestAsync(
            CsvIdentificationRequest csvIdentificationRequest) =>
        TryCatch(async () =>
        {
            await ValidateCsvIdentificationRequestOnAdd(csvIdentificationRequest);

            return await this.reIdentificationStorageBroker
                .InsertCsvIdentificationRequestAsync(csvIdentificationRequest);
        });

        public ValueTask<CsvIdentificationRequest> RetrieveCsvIdentificationRequestByIdAsync(
            Guid csvIdentificationRequestId) =>
        TryCatch(async () =>
        {
            ValidateCsvIdentificationRequestId(csvIdentificationRequestId);

            CsvIdentificationRequest maybeCsvIdentificationRequest =
                await this.reIdentificationStorageBroker
                    .SelectCsvIdentificationRequestByIdAsync(csvIdentificationRequestId);

            ValidateStorageCsvIdentificationRequest(maybeCsvIdentificationRequest, csvIdentificationRequestId);

            return maybeCsvIdentificationRequest;
        });

        public ValueTask<IQueryable<CsvIdentificationRequest>> RetrieveAllCsvIdentificationRequestsAsync() =>
        TryCatch(this.reIdentificationStorageBroker.SelectAllCsvIdentificationRequestsAsync);

        public ValueTask<CsvIdentificationRequest> ModifyCsvIdentificationRequestAsync(
            CsvIdentificationRequest csvIdentificationRequest) =>
        TryCatch(async () =>
        {
            await ValidateCsvIdentificationRequestOnModify(csvIdentificationRequest);

            CsvIdentificationRequest maybeCsvIdentificationRequest =
                await this.reIdentificationStorageBroker
                    .SelectCsvIdentificationRequestByIdAsync(csvIdentificationRequest.Id);

            ValidateStorageCsvIdentificationRequest(
                maybeCsvIdentificationRequest,
                csvIdentificationRequest.Id);

            ValidateAgainstStorageCsvIdentificationRequestOnModify(
                csvIdentificationRequest,
                maybeCsvIdentificationRequest);

            return await this.reIdentificationStorageBroker
                .UpdateCsvIdentificationRequestAsync(csvIdentificationRequest);
        });

        public ValueTask<CsvIdentificationRequest> RemoveCsvIdentificationRequestByIdAsync(
            Guid csvIdentificationRequestId) =>
        TryCatch(async () =>
        {
            ValidateCsvIdentificationRequestId(csvIdentificationRequestId);

            CsvIdentificationRequest maybeCsvIdentificationRequest =
                await this.reIdentificationStorageBroker
                    .SelectCsvIdentificationRequestByIdAsync(csvIdentificationRequestId);

            ValidateStorageCsvIdentificationRequest(maybeCsvIdentificationRequest, csvIdentificationRequestId);

            return await this.reIdentificationStorageBroker
                .DeleteCsvIdentificationRequestAsync(maybeCsvIdentificationRequest);
        });
    }
}
