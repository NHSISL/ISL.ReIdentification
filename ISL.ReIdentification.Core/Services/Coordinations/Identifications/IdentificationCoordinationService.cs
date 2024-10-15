// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.CsvHelpers;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Securities;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Coordinations.Identifications;
using ISL.ReIdentification.Core.Services.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Orchestrations.Persists;

namespace ISL.ReIdentification.Core.Services.Orchestrations.Identifications
{
    public partial class IdentificationCoordinationService : IIdentificationCoordinationService
    {
        private readonly IAccessOrchestrationService accessOrchestrationService;
        private readonly IPersistanceOrchestrationService persistanceOrchestrationService;
        private readonly IIdentificationOrchestrationService identificationOrchestrationService;
        private readonly ICsvHelperBroker csvHelperBroker;
        private readonly ISecurityBroker securityBroker;
        private readonly ILoggingBroker loggingBroker;

        public IdentificationCoordinationService(
            IAccessOrchestrationService accessOrchestrationService,
            IPersistanceOrchestrationService persistanceOrchestrationService,
            IIdentificationOrchestrationService identificationOrchestrationService,
            ICsvHelperBroker csvHelperBroker,
            ISecurityBroker securityBroker,
            ILoggingBroker loggingBroker)
        {
            this.accessOrchestrationService = accessOrchestrationService;
            this.persistanceOrchestrationService = persistanceOrchestrationService;
            this.identificationOrchestrationService = identificationOrchestrationService;
            this.csvHelperBroker = csvHelperBroker;
            this.securityBroker = securityBroker;
            this.loggingBroker = loggingBroker;
        }

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


        public ValueTask<AccessRequest> PersistsCsvIdentificationRequestAsync(AccessRequest accessRequest) =>
        TryCatch(async () =>
        {
            ValidateOnPersistsCsvIdentificationRequest(accessRequest);

            return await this.persistanceOrchestrationService.PersistCsvIdentificationRequestAsync(accessRequest);
        });

        public async ValueTask<AccessRequest> ProcessCsvIdentificationRequestAsync(Guid csvIdentificationRequestId)
        {
            AccessRequest maybeAccessRequest = await this.persistanceOrchestrationService
                .RetrieveCsvIdentificationRequestAsync(csvIdentificationRequestId);

            IdentificationRequest identificationRequest =
                ConvertCsvIdentificationRequestToIdentificationRequest(maybeAccessRequest.CsvIdentificationRequest);

            AccessRequest convertedAccessRequest = new AccessRequest();
            convertedAccessRequest.IdentificationRequest = identificationRequest;

            // override UserIdentifier

            var returnedAccessRequest = await this.accessOrchestrationService
                    .ValidateAccessForIdentificationRequestAsync(convertedAccessRequest);

            IdentificationRequest returnedIdentificationOrchestrationIdentificationRequest =
                await this.identificationOrchestrationService.
                    ProcessIdentificationRequestAsync(returnedAccessRequest.IdentificationRequest);

            CsvIdentificationRequest csvIdentificationRequest =
                ConvertIdentificationRequestToCsvIdentificationRequest(
                    returnedIdentificationOrchestrationIdentificationRequest);

            AccessRequest reIdentifiedAccessRequest = new AccessRequest();
            reIdentifiedAccessRequest.CsvIdentificationRequest = csvIdentificationRequest;

            return reIdentifiedAccessRequest;
        }

        public async ValueTask<AccessRequest> ProcessImpersonationContextRequestAsync(AccessRequest accessRequest) =>
            throw new System.NotImplementedException();

        virtual internal IdentificationRequest ConvertCsvIdentificationRequestToIdentificationRequest(
            CsvIdentificationRequest csvIdentificationRequest)
        {
            // perform conversion

            return new IdentificationRequest();
        }

        virtual internal CsvIdentificationRequest ConvertIdentificationRequestToCsvIdentificationRequest(
            IdentificationRequest identificationRequest)
        {
            // perform conversion

            return new CsvIdentificationRequest();
        }
    }
}
