// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Services.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Services.Foundations.Notifications;

namespace ISL.ReIdentification.Core.Services.Orchestrations.Persists
{
    public class PersistanceOrchestrationService : IPersistanceOrchestrationService
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

        public ValueTask<AccessRequest> PersistImpersonationContextAsync(AccessRequest accessRequest) =>
            throw new NotImplementedException();

        public ValueTask<AccessRequest> PersistCsvIdentificationRequestAsync(AccessRequest accessRequest) =>
            throw new NotImplementedException();

        public ValueTask<AccessRequest> RetrieveCsvIdentificationRequestAsync(Guid csvIdentificationRequestId) =>
            throw new NotImplementedException();
    }
}
