// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Orchestrations.Accesses;

namespace ISL.ReIdentification.Core.Services.Orchestrations.Identifications
{
    public partial class IdentificationCoordinationService : IIdentificationCoordinationService
    {
        private readonly IAccessOrchestrationService accessOrchestrationService;
        private readonly IIdentificationOrchestrationService identificationOrchestrationService;
        private readonly ILoggingBroker loggingBroker;

        public IdentificationCoordinationService(
            IAccessOrchestrationService accessOrchestrationService,
            IIdentificationOrchestrationService identificationOrchestrationService,
            ILoggingBroker loggingBroker)
        {
            this.accessOrchestrationService = accessOrchestrationService;
            this.identificationOrchestrationService = identificationOrchestrationService;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<AccessRequest> ProcessImpersonationContextRequestAsync(AccessRequest accessRequest) =>
            throw new System.NotImplementedException();

        public ValueTask<AccessRequest> ProcessIdentificationRequestsAsync(AccessRequest accessRequest) =>
        TryCatch(async () =>
        {
            ValidateOnProcessIdentificationRequests(accessRequest);

            var returnedAccessRequest =
                await this.accessOrchestrationService.ValidateAccessForIdentificationRequestAsync(accessRequest);

            var returnedIdentificationRequest =
                await this.identificationOrchestrationService
                    .ProcessIdentificationRequestAsync(returnedAccessRequest.IdentificationRequest);

            returnedAccessRequest.IdentificationRequest = returnedIdentificationRequest;

            return returnedAccessRequest;
        });

        public ValueTask<AccessRequest> ProcessCsvIdentificationRequestAsync(AccessRequest accessRequest) =>
            throw new System.NotImplementedException();
    }
}
