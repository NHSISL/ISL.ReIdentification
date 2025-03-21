﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Identifiers;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.Documents;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Services.Foundations.Documents;
using ISL.ReIdentification.Core.Services.Foundations.ReIdentifications;

namespace ISL.ReIdentification.Core.Services.Orchestrations.Identifications
{
    public partial class IdentificationOrchestrationService : IIdentificationOrchestrationService
    {
        private readonly IReIdentificationService reIdentificationService;
        private readonly IAccessAuditService accessAuditService;
        private readonly IDocumentService documentService;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly IIdentifierBroker identifierBroker;
        private readonly ProjectStorageConfiguration projectStorageConfiguration;

        public IdentificationOrchestrationService(
            IReIdentificationService reIdentificationService,
            IAccessAuditService accessAuditService,
            IDocumentService documentService,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker,
            IIdentifierBroker identifierBroker,
            ProjectStorageConfiguration projectStorageConfiguration)
        {
            this.reIdentificationService = reIdentificationService;
            this.accessAuditService = accessAuditService;
            this.documentService = documentService;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.identifierBroker = identifierBroker;
            this.projectStorageConfiguration = projectStorageConfiguration;
        }

        public ValueTask<IdentificationRequest> ProcessIdentificationRequestAsync(
            IdentificationRequest identificationRequest) =>
        TryCatch(async () =>
        {
            ValidateIdentificationRequestIsNotNull(identificationRequest);

            var savedPseduoes = new Dictionary<string, string>();
            var noAccessItems = identificationRequest.IdentificationItems
                .FindAll(x => x.HasAccess == false).ToList();

            var transactionId = await this.identifierBroker.GetIdentifierAsync();
            List<AccessAudit> permissionAudits = new List<AccessAudit>();

            await this.loggingBroker.LogInformationAsync(
                $"Start ReId Check {await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync()}, TransactionId {transactionId}");

            await this.loggingBroker.LogInformationAsync(
                $"Start PDS Check {await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync()}, TransactionId {transactionId}");

            foreach (IdentificationItem item in identificationRequest.IdentificationItems)
            {
                savedPseduoes.Add(
                    item.RowNumber,
                    string.IsNullOrEmpty(item.Identifier)
                        ? item.Identifier
                        : item.Identifier);

                var accessAuditId = await this.identifierBroker.GetIdentifierAsync();

                var noAccessMessage = "User does not have access to the organisation(s) " +
                    "associated with patient.  Re-identification blocked.";

                var accessMessage = "User does have access to the organisation(s) " +
                    "associated with patient.  Item will be submitted for re-identification.";

                AccessAudit accessAudit = new AccessAudit
                {
                    Id = accessAuditId,
                    RequestId = identificationRequest.Id,
                    TransactionId = transactionId,
                    PseudoIdentifier = item.Identifier,
                    EntraUserId = identificationRequest.EntraUserId,
                    GivenName = identificationRequest.GivenName,
                    Surname = identificationRequest.Surname,
                    Email = identificationRequest.Email,
                    Reason = identificationRequest.Reason,
                    Organisation = identificationRequest.Organisation,
                    HasAccess = item.HasAccess,
                    Message = item.HasAccess ? accessMessage : noAccessMessage,
                    AuditType = "PDS Access"
                };

                permissionAudits.Add(accessAudit);

                if (item.HasAccess is false)
                {
                    item.Identifier = "0000000000";
                    item.Message = noAccessMessage;
                }
            }

            await this.accessAuditService.BulkAddAccessAuditAsync(permissionAudits);

            await this.loggingBroker.LogInformationAsync(
                $"Completed PDS Request {await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync()}, TransactionId {transactionId}");

            var hasAccessIdentificationItems =
                identificationRequest.IdentificationItems
                    .FindAll(x => x.HasAccess == true).ToList();

            if (hasAccessIdentificationItems.Count() == 0)
            {
                await this.loggingBroker.LogInformationAsync(
                    $"Completed ReId Check {await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync()}, TransactionId {transactionId}");

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

            List<AccessAudit> reIdentifyAudits = new List<AccessAudit>();

            await this.loggingBroker.LogInformationAsync(
                $"Start NECS Check {await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync()}, TransactionId {transactionId}");

            foreach (IdentificationItem item in reIdentifiedIdentificationRequest.IdentificationItems)
            {
                var record = identificationRequest.IdentificationItems
                    .First(request => request.RowNumber == item.RowNumber);

                var now = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
                var accessAuditId = await this.identifierBroker.GetIdentifierAsync();

                AccessAudit accessAudit = new AccessAudit
                {
                    Id = accessAuditId,
                    RequestId = identificationRequest.Id,
                    TransactionId = transactionId,
                    PseudoIdentifier = savedPseduoes[item.RowNumber],
                    EntraUserId = identificationRequest.EntraUserId,
                    GivenName = identificationRequest.GivenName,
                    Surname = identificationRequest.Surname,
                    Email = identificationRequest.Email,
                    Reason = identificationRequest.Reason,
                    Organisation = identificationRequest.Organisation,
                    HasAccess = item.HasAccess,
                    Message = $"Re-identification outcome: {item.Message}",
                    AuditType = "NECS Access"
                };

                reIdentifyAudits.Add(accessAudit);
                record.Identifier = item.Identifier;
                record.Message = item.Message;
                record.IsReidentified = true;
            }

            await this.accessAuditService.BulkAddAccessAuditAsync(reIdentifyAudits);

            await this.loggingBroker.LogInformationAsync(
                $"Completed NECS Request {await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync()}, TransactionId {transactionId}");

            await this.loggingBroker.LogInformationAsync(
                $"Completed ReId Check {await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync()}, TransactionId {transactionId}");

            return identificationRequest;
        });

        public ValueTask<AccessRequest> ExpireRenewImpersonationContextTokensAsync(AccessRequest accessRequest) =>
        TryCatch(async () =>
        {
            ValidateOnExpireRenewImpersonationContextTokensAsync(accessRequest);
            string container = accessRequest.ImpersonationContext.Id.ToString();
            DateTimeOffset currentDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            string timestamp = currentDateTimeOffset.ToString("yyyyMMddHHmmss");
            string inboxPolicyname = container + "-InboxPolicy-" + timestamp;
            string outboxPolicyname = container + "-OutboxPolicy-" + timestamp;
            string errorsPolicyname = container + "-ErrorsPolicy-" + timestamp;

            DateTimeOffset expiresOn = currentDateTimeOffset
                .AddMinutes(this.projectStorageConfiguration.TokenLifetimeMinutes);

            List<string> retrievedContainers = await this.documentService.RetrieveAllContainersAsync();

            if (!retrievedContainers.Contains(container))
            {
                await this.documentService.AddContainerAsync(container);
                await this.documentService.AddFolderAsync(container, this.projectStorageConfiguration.PickupFolder);
                await this.documentService.AddFolderAsync(container, this.projectStorageConfiguration.LandingFolder);
                await this.documentService.AddFolderAsync(container, this.projectStorageConfiguration.ErrorFolder);
            }

            List<string> maybeAccessPolicies = await this.documentService
                .RetrieveListOfAllAccessPoliciesAsync(container);

            if (maybeAccessPolicies.Count != 0)
            {
                await this.documentService.RemoveAllAccessPoliciesAsync(container);
            }

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
                    Permissions = new List<string>{ "write", "add", "create", "read", "list" }
                },
                new AccessPolicy
                {
                    PolicyName = errorsPolicyname,
                    Permissions = new List<string>{ "read", "list"}
                },
            };

            await this.documentService.CreateAndAssignAccessPoliciesAsync(container, accessPolicies);

            accessRequest.ImpersonationContext.InboxSasToken =
                await this.documentService.CreateSasTokenAsync(
                    container,
                    this.projectStorageConfiguration.PickupFolder,
                    inboxPolicyname,
                    expiresOn);

            accessRequest.ImpersonationContext.OutboxSasToken =
                await this.documentService.CreateSasTokenAsync(
                    container,
                    this.projectStorageConfiguration.LandingFolder,
                    outboxPolicyname,
                    expiresOn);

            accessRequest.ImpersonationContext.ErrorsSasToken =
                await this.documentService.CreateSasTokenAsync(
                    container,
                    this.projectStorageConfiguration.ErrorFolder,
                    errorsPolicyname,
                    expiresOn);

            return accessRequest;
        });

        public ValueTask AddDocumentAsync(Stream input, string fileName, string container) =>
        TryCatch(async () =>
        {
            ValidateOnAddDocument(input, fileName, container);
            await this.documentService.AddDocumentAsync(input, fileName, container);
        });

        public ValueTask RetrieveDocumentByFileNameAsync(Stream output, string fileName, string container) =>
        TryCatch(async () =>
        {
            ValidateOnRetrieveDocumentByFileName(output, fileName, container);
            await this.documentService.RetrieveDocumentByFileNameAsync(output, fileName, container);
        });


        public ValueTask RemoveDocumentByFileNameAsync(string fileName, string container) =>
        TryCatch(async () =>
        {
            ValidateOnRemoveDocumentByFileName(fileName, container);
            await this.documentService.RemoveDocumentByFileNameAsync(fileName, container);
        });
    }
}
