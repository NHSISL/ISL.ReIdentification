// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Foundations.PdsDatas;
using ISL.ReIdentification.Core.Services.Foundations.UserAccesses;
using Xeptions;

namespace ISL.ReIdentification.Core.Services.Orchestrations.Accesses
{
    public partial class AccessOrchestrationService : IAccessOrchestrationService
    {
        private readonly IUserAccessService userAccessService;
        private readonly IPdsDataService pdsDataService;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public AccessOrchestrationService(
            IUserAccessService userAccessService,
            IPdsDataService pdsDataService,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.userAccessService = userAccessService;
            this.pdsDataService = pdsDataService;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<AccessRequest> ProcessImpersonationContextRequestAsync(AccessRequest accessRequest) =>
            throw new NotImplementedException();

        public ValueTask<AccessRequest> ValidateAccessForIdentificationRequestAsync(
            AccessRequest accessRequest) =>
            TryCatch(async () =>
            {
                ValidateAccessRequestIsNotNull(accessRequest);

                List<string> userOrgs =
                    await this.userAccessService
                    .RetrieveAllActiveOrganisationsUserHasAccessTo(accessRequest.IdentificationRequest.EntraUserId);

                AccessRequest validatedAccessRequest = await CheckUserAccessToPatientsAsync(accessRequest, userOrgs);

                return validatedAccessRequest;
            });


        virtual internal async ValueTask<AccessRequest> CheckUserAccessToPatientsAsync(
            AccessRequest accessRequest, List<string> userOrgs)
        {
            var exceptions = new List<Exception>();

            foreach (var identificationItem in accessRequest.IdentificationRequest.IdentificationItems)
            {
                try
                {
                    identificationItem.HasAccess =
                        await this.pdsDataService
                            .OrganisationsHaveAccessToThisPatient(identificationItem.Identifier, userOrgs);
                }
                catch (Exception ex)
                {
                    var exception = ex.DeepClone() as Xeption;
                    exception.AddData("IdentificationItemError", identificationItem.RowNumber);
                    exceptions.Add(exception);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(
                    $"Unable to validate access for {exceptions.Count} identification requests.",
                    exceptions);
            }

            return accessRequest;
        }
    }
}
