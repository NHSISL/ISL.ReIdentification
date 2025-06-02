// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Securities;
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;

namespace ISL.ReIdentification.Core.Services.Foundations.CsvIdentificationRequests
{
    public partial class CsvIdentificationRequestService : ICsvIdentificationRequestService
    {
        private readonly IReIdentificationStorageBroker reIdentificationStorageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ISecurityBroker securityBroker;
        private readonly ILoggingBroker loggingBroker;

        public CsvIdentificationRequestService(
            IReIdentificationStorageBroker reIdentificationStorageBroker,
            IDateTimeBroker dateTimeBroker,
            ISecurityBroker securityBroker,
            ILoggingBroker loggingBroker)
        {
            this.reIdentificationStorageBroker = reIdentificationStorageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.securityBroker = securityBroker;
            this.loggingBroker = loggingBroker;
        }
        public ValueTask<CsvIdentificationRequest> AddCsvIdentificationRequestAsync(
            CsvIdentificationRequest csvIdentificationRequest) =>
        TryCatch(async () =>
        {
            CsvIdentificationRequest csvIdentificationRequestWithAddAuditApplied =
                await ApplyAddAuditAsync(csvIdentificationRequest);

            await ValidateCsvIdentificationRequestOnAdd(csvIdentificationRequestWithAddAuditApplied);

            return await this.reIdentificationStorageBroker
                .InsertCsvIdentificationRequestAsync(csvIdentificationRequestWithAddAuditApplied);
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
            CsvIdentificationRequest csvIdentificationRequestWithModifyAuditApplied =
                await ApplyModifyAuditAsync(csvIdentificationRequest);

            await ValidateCsvIdentificationRequestOnModify(csvIdentificationRequestWithModifyAuditApplied);

            CsvIdentificationRequest maybeCsvIdentificationRequest =
                await this.reIdentificationStorageBroker
                    .SelectCsvIdentificationRequestByIdAsync(csvIdentificationRequestWithModifyAuditApplied.Id);

            ValidateStorageCsvIdentificationRequest(
                maybeCsvIdentificationRequest,
                csvIdentificationRequest.Id);

            ValidateAgainstStorageCsvIdentificationRequestOnModify(
                csvIdentificationRequest,
                maybeCsvIdentificationRequest);

            return await this.reIdentificationStorageBroker
                .UpdateCsvIdentificationRequestAsync(csvIdentificationRequest);
        });

        public ValueTask<CsvIdentificationRequest> RemoveCsvIdentificationRequestByIdAsync(Guid csvIdentificationRequestId) =>
            TryCatch(async () =>
            {
                ValidateCsvIdentificationRequestId(csvIdentificationRequestId);

                CsvIdentificationRequest maybeCsvIdentificationRequest = 
                    await this.reIdentificationStorageBroker.SelectCsvIdentificationRequestByIdAsync(
                        csvIdentificationRequestId);

                ValidateStorageCsvIdentificationRequest(maybeCsvIdentificationRequest, csvIdentificationRequestId);

                CsvIdentificationRequest csvIdentificationRequestWithDeleteAuditApplied = 
                    await ApplyDeleteAuditAsync(maybeCsvIdentificationRequest);

                CsvIdentificationRequest updatedCsvIdentificationRequest =
                    await this.reIdentificationStorageBroker.UpdateCsvIdentificationRequestAsync(
                        csvIdentificationRequestWithDeleteAuditApplied);

                await ValidateAgainstStorageCsvIdentificationRequestOnDeleteAsync(
                    updatedCsvIdentificationRequest,
                    csvIdentificationRequestWithDeleteAuditApplied);

                return await this.reIdentificationStorageBroker.DeleteCsvIdentificationRequestAsync(
                    updatedCsvIdentificationRequest);
            });

        virtual internal async ValueTask<CsvIdentificationRequest> ApplyAddAuditAsync(
            CsvIdentificationRequest csvIdentificationRequest)
        {
            ValidateCsvIdentificationRequestIsNotNull(csvIdentificationRequest);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            csvIdentificationRequest.CreatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            csvIdentificationRequest.CreatedDate = auditDateTimeOffset;
            csvIdentificationRequest.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            csvIdentificationRequest.UpdatedDate = auditDateTimeOffset;

            return csvIdentificationRequest;
        }

        virtual internal async ValueTask<CsvIdentificationRequest> ApplyModifyAuditAsync(
            CsvIdentificationRequest csvIdentificationRequest)
        {
            ValidateCsvIdentificationRequestIsNotNull(csvIdentificationRequest);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            csvIdentificationRequest.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            csvIdentificationRequest.UpdatedDate = auditDateTimeOffset;

            return csvIdentificationRequest;
        }

        virtual internal async ValueTask<CsvIdentificationRequest> ApplyDeleteAuditAsync(CsvIdentificationRequest csvIdentificationRequest)
        {
            ValidateCsvIdentificationRequestIsNotNull(csvIdentificationRequest);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            csvIdentificationRequest.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            csvIdentificationRequest.UpdatedDate = auditDateTimeOffset;
            return csvIdentificationRequest;
        }
    }
}
