// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.Hashing;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Services.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Services.Foundations.Notifications;

namespace ISL.ReIdentification.Core.Services.Orchestrations.Persists
{
    public partial class PersistanceOrchestrationService : IPersistanceOrchestrationService
    {
        private readonly IImpersonationContextService impersonationContextService;
        private readonly ICsvIdentificationRequestService csvIdentificationRequestService;
        private readonly INotificationService notificationService;
        private readonly ILoggingBroker loggingBroker;
        private readonly IHashBroker hashBroker;

        public PersistanceOrchestrationService(
            IImpersonationContextService impersonationContextService,
            ICsvIdentificationRequestService csvIdentificationRequestService,
            INotificationService notificationService,
            ILoggingBroker loggingBroker,
            IHashBroker hashBroker)
        {
            this.impersonationContextService = impersonationContextService;
            this.csvIdentificationRequestService = csvIdentificationRequestService;
            this.notificationService = notificationService;
            this.loggingBroker = loggingBroker;
            this.hashBroker = hashBroker;
        }

        public ValueTask<AccessRequest> PersistImpersonationContextAsync(AccessRequest accessRequest) =>
            throw new NotImplementedException();

        public ValueTask<AccessRequest> PersistCsvIdentificationRequestAsync(AccessRequest accessRequest) =>
            // Validate the AccessRequest is not null

            // validate AccessRequest.CsvIdentificationRequest is not null

            // Call IHashBroker GenerateSha256Hash

            // Call csvIdentificationRequestService.RetrieveAllCsvIdentificationRequestsAsync filter on RecipientEntraUserId and Sha256Hash and

            // if exists, return new AccessRequest with CsvIdentificationRequest

            // if not populate CsvIdentificationRequest.Sha256Hash with generated hash

            // Call csvIdentificationRequestService.AddCsvIdentificationRequestAsync passing in AccessRequest.CsvIdentificationRequest

            // Create new AccessRequest with returned CsvIdentificationRequest

            // Call notificationService.SendPendingApprovalNotificationAsync

            // Return AccessRequest

            // Handle exceptions

            throw new NotImplementedException();

        public ValueTask<AccessRequest> RetrieveCsvIdentificationRequestByIdAsync(
            Guid csvIdentificationRequestId) =>
        TryCatch(async () =>
        {
            ValidateOnRetrieveCsvIdentificationRequestByIdAsync(csvIdentificationRequestId);

            CsvIdentificationRequest csvIdentificationRequest = await this.csvIdentificationRequestService
                .RetrieveCsvIdentificationRequestByIdAsync(csvIdentificationRequestId);

            return new AccessRequest { CsvIdentificationRequest = csvIdentificationRequest };
        });
    }
}
