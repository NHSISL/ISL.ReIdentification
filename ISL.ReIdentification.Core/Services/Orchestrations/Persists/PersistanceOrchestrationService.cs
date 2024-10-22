// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.Hashing;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
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
        TryCatch(async () =>
        {
            ValidateAccessRequestIsNotNull(accessRequest);
            var maybeImpersonationContexts = await this.impersonationContextService
                .RetrieveAllImpersonationContextsAsync();

            ImpersonationContext? matchedImpersonationContext = maybeImpersonationContexts
                .Where(impersonationContext =>
                    impersonationContext.Id == accessRequest.ImpersonationContext.Id)
                .FirstOrDefault();

            if (matchedImpersonationContext is not null)
            {
                if (accessRequest.ImpersonationContext.IsApproved == matchedImpersonationContext.IsApproved)
                {
                    return accessRequest;
                }

                var modifiedImpersonationContext = await this.impersonationContextService
                        .ModifyImpersonationContextAsync(accessRequest.ImpersonationContext);

                var returnedAccessRequest =
                    new AccessRequest { ImpersonationContext = modifiedImpersonationContext };

                if (accessRequest.ImpersonationContext.IsApproved == true)
                {
                    await this.notificationService.SendApprovedNotificationAsync(returnedAccessRequest);
                }
                else
                {
                    await this.notificationService.SendDeniedNotificationAsync(returnedAccessRequest);
                }

                return returnedAccessRequest;
            }
            else
            {
                var createdImpersonationContext = await this.impersonationContextService
                    .AddImpersonationContextAsync(accessRequest.ImpersonationContext);

                var returnedAccessRequest = new AccessRequest { ImpersonationContext = createdImpersonationContext };
                await this.notificationService.SendPendingApprovalNotificationAsync(returnedAccessRequest);

                return returnedAccessRequest;
            }
        });

        public ValueTask<AccessRequest> PersistCsvIdentificationRequestAsync(AccessRequest accessRequest) =>
        TryCatch(async () =>
        {
            ValidateAccessRequestIsNotNull(accessRequest);
            ValidateOnPersistCsvIdentificationRequestAsync(accessRequest.CsvIdentificationRequest);

            string outputHash =
                this.hashBroker.GenerateSha256Hash(new MemoryStream(accessRequest.CsvIdentificationRequest.Data));

            IQueryable<CsvIdentificationRequest> returnedCsvIdentificationRequests =
                await this.csvIdentificationRequestService.RetrieveAllCsvIdentificationRequestsAsync();

            IQueryable<CsvIdentificationRequest> matchedCsvIdentificationRequests =
                returnedCsvIdentificationRequests.Where(request =>
                    request.RecipientEntraUserId == accessRequest.CsvIdentificationRequest.RecipientEntraUserId
                    && outputHash == accessRequest.CsvIdentificationRequest.Sha256Hash);

            if (matchedCsvIdentificationRequests.Any())
            {
                return new AccessRequest { CsvIdentificationRequest = matchedCsvIdentificationRequests.First() };
            }

            accessRequest.CsvIdentificationRequest.Sha256Hash = outputHash;

            var outputCsvIdentificationRequest =
                await this.csvIdentificationRequestService
                    .AddCsvIdentificationRequestAsync(accessRequest.CsvIdentificationRequest);

            var returnedAccessRequest = new AccessRequest { CsvIdentificationRequest = outputCsvIdentificationRequest };
            await this.notificationService.SendPendingApprovalNotificationAsync(returnedAccessRequest);

            return returnedAccessRequest;
        });

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
