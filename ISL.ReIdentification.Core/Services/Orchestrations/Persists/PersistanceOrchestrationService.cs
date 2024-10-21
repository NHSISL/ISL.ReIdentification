// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
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

        public PersistanceOrchestrationService(
            IImpersonationContextService impersonationContextService,
            ICsvIdentificationRequestService csvIdentificationRequestService,
            INotificationService notificationService,
            ILoggingBroker loggingBroker)
        {
            this.impersonationContextService = impersonationContextService;
            this.csvIdentificationRequestService = csvIdentificationRequestService;
            this.notificationService = notificationService;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<AccessRequest> PersistImpersonationContextAsync(AccessRequest accessRequest)
        {
            var maybeImpersonationContexts = await this.impersonationContextService
                .RetrieveAllImpersonationContextsAsync();

            IQueryable<ImpersonationContext> matchedImpersonationContexts = maybeImpersonationContexts
                .Where(impersonationContext =>
                    impersonationContext.Id == accessRequest.ImpersonationContext.Id);

            if (matchedImpersonationContexts.Any())
            {
                throw new NotImplementedException();
            }
            else
            {
                var createdImpersonationContext = await this.impersonationContextService
                    .AddImpersonationContextAsync(accessRequest.ImpersonationContext);

                var returnedAccessRequest = new AccessRequest { ImpersonationContext = createdImpersonationContext };
                await this.notificationService.SendPendingApprovalNotificationAsync(returnedAccessRequest);

                return returnedAccessRequest;
            }
        }

        public ValueTask<AccessRequest> PersistCsvIdentificationRequestAsync(AccessRequest accessRequest) =>
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
