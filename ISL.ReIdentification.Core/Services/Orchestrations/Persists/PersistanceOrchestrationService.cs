// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Hashing;
using ISL.ReIdentification.Core.Brokers.Identifiers;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.Documents;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Persists;
using ISL.ReIdentification.Core.Services.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Services.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Services.Foundations.Documents;
using ISL.ReIdentification.Core.Services.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Services.Foundations.Notifications;

namespace ISL.ReIdentification.Core.Services.Orchestrations.Persists
{
    public partial class PersistanceOrchestrationService : IPersistanceOrchestrationService
    {
        private readonly IImpersonationContextService impersonationContextService;
        private readonly ICsvIdentificationRequestService csvIdentificationRequestService;
        private readonly INotificationService notificationService;
        private readonly IAccessAuditService accessAuditService;
        private readonly IDocumentService documentService;
        private readonly ILoggingBroker loggingBroker;
        private readonly IHashBroker hashBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly IIdentifierBroker identifierBroker;
        private readonly CsvReIdentificationConfigurations csvReIdentificationConfigurations;
        private readonly ProjectStorageConfiguration projectStorageConfiguration;

        public PersistanceOrchestrationService(
            IImpersonationContextService impersonationContextService,
            ICsvIdentificationRequestService csvIdentificationRequestService,
            INotificationService notificationService,
            IAccessAuditService accessAuditService,
            IDocumentService documentService,
            ILoggingBroker loggingBroker,
            IHashBroker hashBroker,
            IDateTimeBroker dateTimeBroker,
            IIdentifierBroker identifierBroker,
            CsvReIdentificationConfigurations csvReIdentificationConfigurations,
            ProjectStorageConfiguration projectStorageConfiguration)
        {
            this.impersonationContextService = impersonationContextService;
            this.csvIdentificationRequestService = csvIdentificationRequestService;
            this.notificationService = notificationService;
            this.accessAuditService = accessAuditService;
            this.documentService = documentService;
            this.loggingBroker = loggingBroker;
            this.hashBroker = hashBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.identifierBroker = identifierBroker;
            this.csvReIdentificationConfigurations = csvReIdentificationConfigurations;
        }

        public ValueTask<AccessRequest> PersistImpersonationContextAsync(AccessRequest accessRequest) =>
        TryCatch(async () =>
        {
            ValidateOnPersistImpersonationContextAsync(accessRequest);

            var maybeImpersonationContexts = await this.impersonationContextService
                .RetrieveAllImpersonationContextsAsync();

            ImpersonationContext matchedImpersonationContext = maybeImpersonationContexts
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

                var modifiedAccessRequest =
                    new AccessRequest { ImpersonationContext = modifiedImpersonationContext };

                await (accessRequest.ImpersonationContext.IsApproved
                    ? this.notificationService.SendImpersonationApprovedNotificationAsync(modifiedAccessRequest)
                    : this.notificationService.SendImpersonationDeniedNotificationAsync(modifiedAccessRequest));

                return modifiedAccessRequest;
            }

            var createdImpersonationContext = await this.impersonationContextService
                .AddImpersonationContextAsync(accessRequest.ImpersonationContext);

            var createdAccessRequest = new AccessRequest { ImpersonationContext = createdImpersonationContext };
            await this.notificationService.SendImpersonationPendingApprovalNotificationAsync(createdAccessRequest);

            return createdAccessRequest;
        });

        public ValueTask<AccessRequest> RetrieveImpersonationContextByIdAsync(Guid impersonationContextId) =>
        TryCatch(async () =>
        {
            ValidateOnRetrieveImpersonationContextByIdAsync(impersonationContextId);

            ImpersonationContext impersonationContext = await this.impersonationContextService
                .RetrieveImpersonationContextByIdAsync(impersonationContextId);

            return new AccessRequest { ImpersonationContext = impersonationContext };
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
            await this.notificationService.SendCsvPendingApprovalNotificationAsync(returnedAccessRequest);

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

        public ValueTask PurgeCsvReIdentificationRecordsThatExpired() =>
        TryCatch(async () =>
        {
            ValidateOnPurgeCsvIdentificationRecordsThatExpiredAsync(this.csvReIdentificationConfigurations);

            DateTimeOffset dateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            DateTimeOffset expiryDate =
                dateTimeOffset.AddMinutes(this.csvReIdentificationConfigurations.ExpireAfterMinutes * -1);

            IQueryable<CsvIdentificationRequest> csvIdentificationRequests = await this.csvIdentificationRequestService
                .RetrieveAllCsvIdentificationRequestsAsync();

            List<CsvIdentificationRequest> expiredCsvIdentificationRequests = csvIdentificationRequests.Where(request =>
                request.Data != null && request.CreatedDate < expiryDate).ToList();

            foreach (CsvIdentificationRequest csvIdentificationRequest in expiredCsvIdentificationRequests)
            {
                var accessAuditId = await this.identifierBroker.GetIdentifierAsync();
                csvIdentificationRequest.Data = null;
                csvIdentificationRequest.UpdatedDate = dateTimeOffset;

                await this.csvIdentificationRequestService.ModifyCsvIdentificationRequestAsync(csvIdentificationRequest);

                AccessAudit accessAudit = new AccessAudit
                {
                    Id = accessAuditId,
                    RequestId = csvIdentificationRequest.Id,
                    PseudoIdentifier = "PURGED",
                    EntraUserId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    TransactionId = Guid.Empty,
                    GivenName = "PURGED",
                    Surname = "PURGED",
                    Email = "PURGED",
                    Purpose = "PURGED",
                    Reason = "PURGED",
                    Organisation = "PURGED",
                    HasAccess = false,
                    Message = $"Purged on {dateTimeOffset}",
                    CreatedBy = "System",
                    CreatedDate = dateTimeOffset,
                    UpdatedBy = "System",
                    UpdatedDate = dateTimeOffset
                };

                await this.accessAuditService.AddAccessAuditAsync(accessAudit);
            }
        });

        virtual internal async ValueTask<AccessRequest> CreateOrRenewTokens(AccessRequest accessRequest)
        {
            // use the retrieve list all access policies to check the container
            string container = accessRequest.ImpersonationContext.Id.ToString();
            string inboxPolicyname = container + "-InboxPolicy";
            string outboxPolicyname = container + "-OutboxPolicy";
            string errorsPolicyname = container + "-ErrorsPolicy";
            DateTimeOffset currentDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            DateTimeOffset expiresOn = currentDateTimeOffset
                .AddMinutes(this.projectStorageConfiguration.TokenLifetimeMinutes);

            List<string> maybeAccessPolicies = await this.documentService
                .RetrieveAllAccessPoliciesFromContainerAsync(container);

            // if any are found, remove all access policies from container
            if (maybeAccessPolicies.Any())
            {
                await this.documentService.RemoveAllAccessPoliciesFromContainerAsync(container);
            }

            // create the appropriate access policies - one for each folder

            List<AccessPolicy> accessPolicies = new List<AccessPolicy>
            {
                new AccessPolicy
                {
                    PolicyName = inboxPolicyname,
                    Permissions = new List<string>{ "read", "list"}
                },
                new AccessPolicy
                {
                    PolicyName = outboxPolicyname,
                    Permissions = new List<string>{ "write", "add", "create"}
                },
                new AccessPolicy
                {
                    PolicyName = errorsPolicyname,
                    Permissions = new List<string>{ "read", "list"}
                },
            };

            await this.documentService.CreateAndAssignAccessPoliciesAsync(container, accessPolicies);

            // create the access tokens for each one.
            string inboxSasToken = await this.documentService.CreateDirectorySasTokenAsync(
                container,
                this.projectStorageConfiguration.PickupFolder,
                inboxPolicyname,
                expiresOn);

            string outboxSasToken = await this.documentService.CreateDirectorySasTokenAsync(
                container,
                this.projectStorageConfiguration.PickupFolder,
                outboxPolicyname,
                expiresOn);

            string errorsSasToken = await this.documentService.CreateDirectorySasTokenAsync(
                container,
                this.projectStorageConfiguration.PickupFolder,
                errorsPolicyname,
                expiresOn);

            // add to the
            //accessRequest.ImpersonationContext.InboxSasToken = inboxSasToken;
            //accessRequest.ImpersonationContext.OutboxSasToken = outboxSasToken;
            //accessRequest.ImpersonationContext.ErrorsSasToken = errorsSasToken;

            return accessRequest;
        }
    }
}
