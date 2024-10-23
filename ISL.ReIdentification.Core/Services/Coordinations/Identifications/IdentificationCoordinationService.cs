// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.CsvHelpers;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Securities;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Securities;
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

        public ValueTask<AccessRequest> ProcessCsvIdentificationRequestAsync(Guid csvIdentificationRequestId) =>
            TryCatch(async () =>
            {
                ValidateCsvIdentificationRequestId(csvIdentificationRequestId);

                AccessRequest maybeAccessRequest = await this.persistanceOrchestrationService
                    .RetrieveCsvIdentificationRequestByIdAsync(csvIdentificationRequestId);

                IdentificationRequest identificationRequest =
                    await ConvertCsvIdentificationRequestToIdentificationRequest(
                        maybeAccessRequest.CsvIdentificationRequest);

                AccessRequest convertedAccessRequest = new AccessRequest();
                EntraUser currentUser = await this.securityBroker.GetCurrentUser();

                IdentificationRequest currentUserIdentificationRequest =
                    OverrideIdentificationRequestUserDetails(identificationRequest, currentUser);

                convertedAccessRequest.IdentificationRequest = currentUserIdentificationRequest;

                var returnedAccessRequest = await this.accessOrchestrationService
                        .ValidateAccessForIdentificationRequestAsync(convertedAccessRequest);

                IdentificationRequest returnedIdentificationOrchestrationIdentificationRequest =
                    await this.identificationOrchestrationService.
                        ProcessIdentificationRequestAsync(returnedAccessRequest.IdentificationRequest);

                CsvIdentificationRequest csvIdentificationRequest =
                    await ConvertIdentificationRequestToCsvIdentificationRequest(
                        returnedIdentificationOrchestrationIdentificationRequest);

                AccessRequest reIdentifiedAccessRequest = new AccessRequest();
                reIdentifiedAccessRequest.CsvIdentificationRequest = csvIdentificationRequest;

                return reIdentifiedAccessRequest;
            });

        public ValueTask<AccessRequest> PersistsImpersonationContextAsync(AccessRequest accessRequest) =>
        TryCatch(async () =>
        {
            ValidateImpersonationContext(accessRequest);
            
            return await this.persistanceOrchestrationService.PersistImpersonationContextAsync(accessRequest);
        });

        public ValueTask<AccessRequest> ProcessImpersonationContextRequestAsync(AccessRequest accessRequest) =>
        TryCatch(async () =>
        {
            ValidateImpersonationContext(accessRequest);

            IdentificationRequest identificationRequest =
                await ConvertCsvIdentificationRequestToIdentificationRequest(
                    accessRequest.CsvIdentificationRequest);

            AccessRequest convertedAccessRequest = new AccessRequest();
            string filepath = accessRequest.CsvIdentificationRequest.Filepath;
            string[] filepathParts = filepath.Split('/');
            string container = filepathParts[1];
            Guid impersonationContextId = Guid.Parse(filepathParts[2]);
            string fileName = filepathParts[4];

            AccessRequest returnedAccessRequestWithImpersonationContext =
                await this.persistanceOrchestrationService
                    .RetrieveImpersonationContextByIdAsync(impersonationContextId);

            IdentificationRequest currentIdentificationRequest =
                OverrideIdentificationRequestUserDetails(
                    identificationRequest, returnedAccessRequestWithImpersonationContext.ImpersonationContext);

            convertedAccessRequest.IdentificationRequest = currentIdentificationRequest;

            AccessRequest returnedAccessRequest =
                await this.accessOrchestrationService
                    .ValidateAccessForIdentificationRequestAsync(convertedAccessRequest);

            IdentificationRequest returnedIdentificationRequest =
                await this.identificationOrchestrationService
                    .ProcessIdentificationRequestAsync(returnedAccessRequest.IdentificationRequest);

            CsvIdentificationRequest reIdentifiedCsvIdentificationRequest =
                await ConvertIdentificationRequestToCsvIdentificationRequest(returnedIdentificationRequest);

            AccessRequest reIdentifiedAccessRequest = new AccessRequest
            {
                CsvIdentificationRequest = reIdentifiedCsvIdentificationRequest
            };

            MemoryStream csvData = new MemoryStream(reIdentifiedCsvIdentificationRequest.Data);
            await this.identificationOrchestrationService.AddDocumentAsync(csvData, fileName, container);

            return reIdentifiedAccessRequest;
        });

        virtual async internal ValueTask<IdentificationRequest> ConvertCsvIdentificationRequestToIdentificationRequest(
            CsvIdentificationRequest csvIdentificationRequest)
        {
            string data = Encoding.UTF8.GetString(csvIdentificationRequest.Data);

            var mappedItems =
                await this.csvHelperBroker.MapCsvToObjectAsync<CsvIdentificationItem>(
                    data: data,
                    hasHeaderRecord: true,
                    fieldMappings: null);

            var identificationItems = new List<IdentificationItem>();

            foreach (var mappedItem in mappedItems)
            {
                var identificationItem = new IdentificationItem
                {
                    HasAccess = false,
                    Identifier = mappedItem.Identifier,
                    IsReidentified = false,
                    Message = String.Empty,
                    RowNumber = mappedItem.RowNumber
                };

                identificationItems.Add(identificationItem);
            }

            var identificationRequest = new IdentificationRequest();
            identificationRequest.DisplayName = csvIdentificationRequest.RecipientDisplayName;
            identificationRequest.Email = csvIdentificationRequest.RecipientEmail;
            identificationRequest.EntraUserId = csvIdentificationRequest.RecipientEntraUserId;
            identificationRequest.GivenName = csvIdentificationRequest.RecipientFirstName;
            identificationRequest.JobTitle = csvIdentificationRequest.RecipientJobTitle;
            identificationRequest.Organisation = csvIdentificationRequest.Organisation;
            identificationRequest.Purpose = csvIdentificationRequest.Purpose;
            identificationRequest.Reason = csvIdentificationRequest.Reason;
            identificationRequest.Surname = csvIdentificationRequest.RecipientLastName;
            identificationRequest.IdentificationItems = identificationItems;

            return identificationRequest;
        }

        virtual internal async ValueTask<CsvIdentificationRequest> ConvertIdentificationRequestToCsvIdentificationRequest(
            IdentificationRequest identificationRequest)
        {
            List<IdentificationItem> identificationItems = identificationRequest.IdentificationItems;
            List<CsvIdentificationItem> identificationsData = identificationItems
                .Select(identificationItem =>
                {
                    var csvIdentificationItem = new CsvIdentificationItem();
                    csvIdentificationItem.RowNumber = identificationItem.RowNumber;
                    csvIdentificationItem.Identifier = identificationItem.Identifier;

                    return csvIdentificationItem;
                })
                    .ToList();

            string csvIdentificationRequestData =
                await this.csvHelperBroker.MapObjectToCsvAsync(
                    identificationsData,
                    addHeaderRecord: true,
                    fieldMappings: null,
                    shouldAddTrailingComma: false);

            byte[] csvIdentificationRequestDataByteArray = Encoding.UTF8.GetBytes(csvIdentificationRequestData);

            var csvIdentificationRequest = new CsvIdentificationRequest();
            csvIdentificationRequest.RecipientDisplayName = identificationRequest.DisplayName;
            csvIdentificationRequest.RecipientEmail = identificationRequest.Email;
            csvIdentificationRequest.RecipientEntraUserId = identificationRequest.EntraUserId;
            csvIdentificationRequest.RecipientFirstName = identificationRequest.GivenName;
            csvIdentificationRequest.RecipientJobTitle = identificationRequest.JobTitle;
            csvIdentificationRequest.Organisation = identificationRequest.Organisation;
            csvIdentificationRequest.Purpose = identificationRequest.Purpose;
            csvIdentificationRequest.RecipientLastName = identificationRequest.Surname;
            csvIdentificationRequest.Reason = identificationRequest.Reason;
            csvIdentificationRequest.Data = csvIdentificationRequestDataByteArray;

            return csvIdentificationRequest;
        }

        private IdentificationRequest OverrideIdentificationRequestUserDetails(
            IdentificationRequest identificationRequest,
            EntraUser currentUser)
        {
            identificationRequest.EntraUserId = currentUser.EntraUserId;
            identificationRequest.Email = currentUser.Email;
            identificationRequest.JobTitle = currentUser.JobTitle;
            identificationRequest.DisplayName = currentUser.DisplayName;
            identificationRequest.GivenName = currentUser.GivenName;
            identificationRequest.Surname = currentUser.Surname;

            return identificationRequest;
        }

        private IdentificationRequest OverrideIdentificationRequestUserDetails(
            IdentificationRequest identificationRequest,
            ImpersonationContext impersonationContext)
        {
            identificationRequest.EntraUserId = impersonationContext.RequesterEntraUserId;
            identificationRequest.Email = impersonationContext.RequesterEmail;
            identificationRequest.JobTitle = impersonationContext.RequesterJobTitle;
            identificationRequest.DisplayName = impersonationContext.RequesterDisplayName;
            identificationRequest.GivenName = impersonationContext.RequesterFirstName;
            identificationRequest.Surname = impersonationContext.RequesterLastName;

            return identificationRequest;
        }
    }
}
