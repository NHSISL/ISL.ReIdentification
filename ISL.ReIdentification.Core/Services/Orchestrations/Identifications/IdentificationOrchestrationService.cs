// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Identifiers;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Services.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Services.Foundations.ReIdentifications;

namespace ISL.ReIdentification.Core.Services.Orchestrations.Identifications
{
    public partial class IdentificationOrchestrationService : IIdentificationOrchestrationService
    {
        private readonly IReIdentificationService reIdentificationService;
        private readonly IAccessAuditService accessAuditService;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly IIdentifierBroker identifierBroker;

        public IdentificationOrchestrationService(
            IReIdentificationService reIdentificationService,
            IAccessAuditService accessAuditService,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker,
            IIdentifierBroker identifierBroker)
        {
            this.reIdentificationService = reIdentificationService;
            this.accessAuditService = accessAuditService;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.identifierBroker = identifierBroker;
        }

        public ValueTask<IdentificationRequest> ProcessIdentificationRequestAsync(
            IdentificationRequest identificationRequest) =>
        TryCatch(async () =>
        {
            ValidateIdentificationRequestIsNotNull(identificationRequest);

            foreach (IdentificationItem item in identificationRequest.IdentificationItems)
            {
                var now = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
                var accessAuditId = await this.identifierBroker.GetIdentifierAsync();

                AccessAudit accessAudit = new AccessAudit
                {
                    Id = accessAuditId,
                    PseudoIdentifier = item.Identifier,
                    EntraUserId = identificationRequest.EntraUserId,
                    GivenName = identificationRequest.GivenName,
                    Surname = identificationRequest.Surname,
                    Email = identificationRequest.Email,
                    Reason = identificationRequest.Reason,
                    Organisation = identificationRequest.Organisation,
                    HasAccess = item.HasAccess,
                    Message = item.Message,
                    CreatedBy = "System",
                    CreatedDate = now,
                    UpdatedBy = "System",
                    UpdatedDate = now
                };

                await this.accessAuditService.AddAccessAuditAsync(accessAudit);

                if (item.HasAccess is false)
                {
                    item.Identifier = "0000000000";
                    item.Message = "Failed to Re-Identify. User do not have access to the organisation(s) associated with patient.";
                }
            }

            var hasAccessIdentificationItems =
                identificationRequest.IdentificationItems
                    .FindAll(x => x.HasAccess == true).ToList();

            if (hasAccessIdentificationItems.Count() == 0)
            {
                return identificationRequest;
            }

            IdentificationRequest hasAccessIdentificationRequest = new IdentificationRequest
            {
                Id = identificationRequest.Id,
                IdentificationItems = hasAccessIdentificationItems,
                EntraUserId = identificationRequest.EntraUserId,
                GivenName = identificationRequest.GivenName,
                Surname = identificationRequest.Surname,
                DisplayName = identificationRequest.DisplayName,
                JobTitle = identificationRequest.JobTitle,
                Email = identificationRequest.Email,
                Organisation = identificationRequest.Organisation,
                Reason = identificationRequest.Reason
            };

            var reIdentifiedIdentificationRequest =
                await this.reIdentificationService.ProcessReIdentificationRequest(
                    hasAccessIdentificationRequest);

            foreach (IdentificationItem item in reIdentifiedIdentificationRequest.IdentificationItems)
            {
                var record = identificationRequest.IdentificationItems
                    .First(request => request.RowNumber == item.RowNumber);

                record.Identifier = item.Identifier;
                record.Message = item.Message;
                record.IsReidentified = true;
            }

            return identificationRequest;
        });

        public ValueTask AddDocumentAsync(Stream input, string fileName, string container) =>
            throw new NotImplementedException();

        public ValueTask RetrieveDocumentByFileNameAsync(Stream output, string fileName, string container) =>
            throw new NotImplementedException();

        public ValueTask RemoveDocumentByFileNameAsync(string filename, string container) =>
            throw new NotImplementedException();
    }
}
