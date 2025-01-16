// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Brokers.CsvHelpers;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Securities;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Orchestrations.Identifications;
using ISL.ReIdentification.Core.Services.Orchestrations.Persists;

namespace ISL.ReIdentification.Core.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationService : IIdentificationCoordinationService
    {
        private readonly IAccessOrchestrationService accessOrchestrationService;
        private readonly IPersistanceOrchestrationService persistanceOrchestrationService;
        private readonly IIdentificationOrchestrationService identificationOrchestrationService;
        private readonly ICsvHelperBroker csvHelperBroker;
        private readonly ISecurityBroker securityBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ProjectStorageConfiguration projectStorageConfiguration;

        public IdentificationCoordinationService(
            IAccessOrchestrationService accessOrchestrationService,
            IPersistanceOrchestrationService persistanceOrchestrationService,
            IIdentificationOrchestrationService identificationOrchestrationService,
            ICsvHelperBroker csvHelperBroker,
            ISecurityBroker securityBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker,
            ProjectStorageConfiguration projectStorageConfiguration)
        {
            this.accessOrchestrationService = accessOrchestrationService;
            this.persistanceOrchestrationService = persistanceOrchestrationService;
            this.identificationOrchestrationService = identificationOrchestrationService;
            this.csvHelperBroker = csvHelperBroker;
            this.securityBroker = securityBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.projectStorageConfiguration = projectStorageConfiguration;
        }

        public ValueTask<AccessRequest> ProcessIdentificationRequestsAsync(AccessRequest accessRequest) =>
        TryCatch(async () =>
        {
            ValidateOnProcessIdentificationRequests(accessRequest);
            EntraUser currentUser = await this.securityBroker.GetCurrentUser();
            accessRequest.IdentificationRequest.EntraUserId = currentUser.EntraUserId;
            accessRequest.IdentificationRequest.Email = currentUser.Email;
            accessRequest.IdentificationRequest.JobTitle = currentUser.JobTitle;
            accessRequest.IdentificationRequest.DisplayName = currentUser.DisplayName;
            accessRequest.IdentificationRequest.GivenName = currentUser.GivenName;
            accessRequest.IdentificationRequest.Surname = currentUser.Surname;

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
            await ConvertCsvIdentificationRequestToIdentificationRequest(accessRequest);

            return await this.persistanceOrchestrationService.PersistCsvIdentificationRequestAsync(accessRequest);
        });

        public ValueTask<AccessRequest> ProcessCsvIdentificationRequestAsync(
            Guid csvIdentificationRequestId, string reason) =>
        TryCatch(async () =>
        {
            ValidateOnProcessCsvIdentificationRequest(csvIdentificationRequestId, reason);

            AccessRequest maybeAccessRequest = await this.persistanceOrchestrationService
                .RetrieveCsvIdentificationRequestByIdAsync(csvIdentificationRequestId);

            AccessRequest csvConvertedAccessRequest =
                await ConvertCsvIdentificationRequestToIdentificationRequest(
                    maybeAccessRequest);

            EntraUser currentUser = await this.securityBroker.GetCurrentUser();

            IdentificationRequest currentUserIdentificationRequest =
                OverrideIdentificationRequestUserDetails(
                    csvConvertedAccessRequest.IdentificationRequest,
                    currentUser,
                    reason);

            csvConvertedAccessRequest.IdentificationRequest = currentUserIdentificationRequest;

            var returnedAccessRequest = await this.accessOrchestrationService
                    .ValidateAccessForIdentificationRequestAsync(csvConvertedAccessRequest);

            IdentificationRequest returnedIdentificationOrchestrationIdentificationRequest =
                await this.identificationOrchestrationService.
                    ProcessIdentificationRequestAsync(returnedAccessRequest.IdentificationRequest);

            AccessRequest processedAccessRequest = returnedAccessRequest.DeepClone();
            processedAccessRequest.IdentificationRequest = returnedIdentificationOrchestrationIdentificationRequest;

            AccessRequest reIdentifiedAccessRequest = await ConvertIdentificationRequestToCsvIdentificationRequest(
                    processedAccessRequest);

            DateTimeOffset dateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            string timestamp = dateTimeOffset.ToString("yyyyMMddHHmms");
            string fileName = $"{reIdentifiedAccessRequest.CsvIdentificationRequest.Filepath}_{timestamp}.csv";
            reIdentifiedAccessRequest.CsvIdentificationRequest.Filepath = fileName;

            return reIdentifiedAccessRequest;
        });

        public ValueTask<AccessRequest> PersistsImpersonationContextAsync(AccessRequest accessRequest) =>
        TryCatch(async () =>
        {
            ValidateOnPersistImpersonationContext(accessRequest);

            return await this.persistanceOrchestrationService.PersistImpersonationContextAsync(accessRequest);
        });

        public ValueTask<AccessRequest> ProcessImpersonationContextRequestAsync(AccessRequest accessRequest) =>
        TryCatch(async () =>
        {
            ValidateOnProcessImpersonationContextRequestAsync(accessRequest, this.projectStorageConfiguration);
            var filepathData = await ExtractFromFilepath(accessRequest.CsvIdentificationRequest.Filepath);

            try
            {
                AccessRequest csvConvertedAccessRequest =
                    await ConvertCsvIdentificationRequestToIdentificationRequest(
                        accessRequest);

                AccessRequest returnedAccessRequestWithImpersonationContext =
                    await this.persistanceOrchestrationService
                        .RetrieveImpersonationContextByIdAsync(filepathData.ImpersonationContextId);

                IdentificationRequest currentIdentificationRequest =
                    OverrideIdentificationRequestUserDetails(
                        csvConvertedAccessRequest.IdentificationRequest,
                        returnedAccessRequestWithImpersonationContext.ImpersonationContext);

                csvConvertedAccessRequest.IdentificationRequest = currentIdentificationRequest;

                AccessRequest returnedAccessRequest =
                    await this.accessOrchestrationService
                        .ValidateAccessForIdentificationRequestAsync(csvConvertedAccessRequest);

                IdentificationRequest returnedIdentificationRequest =
                    await this.identificationOrchestrationService
                        .ProcessIdentificationRequestAsync(returnedAccessRequest.IdentificationRequest);

                AccessRequest processedAccessRequest = returnedAccessRequest.DeepClone();
                processedAccessRequest.IdentificationRequest = returnedIdentificationRequest;

                AccessRequest reIdentifiedAccessRequest =
                    await ConvertIdentificationRequestToCsvIdentificationRequest(processedAccessRequest);

                MemoryStream csvData = new MemoryStream(reIdentifiedAccessRequest.CsvIdentificationRequest.Data);

                await this.identificationOrchestrationService
                    .AddDocumentAsync(
                        csvData, filepathData.PickupFilepath, projectStorageConfiguration.Container);

                await this.identificationOrchestrationService
                    .RemoveDocumentByFileNameAsync(
                        filepathData.LandingFilepath, projectStorageConfiguration.Container);

                return reIdentifiedAccessRequest;
            }
            catch (Exception)
            {
                MemoryStream csvData = new MemoryStream(accessRequest.CsvIdentificationRequest.Data);

                await this.identificationOrchestrationService
                    .AddDocumentAsync(
                        csvData, filepathData.ErrorFilepath, projectStorageConfiguration.Container);

                await this.identificationOrchestrationService
                    .RemoveDocumentByFileNameAsync(
                        filepathData.LandingFilepath, projectStorageConfiguration.Container);

                throw;
            }
        });

        public ValueTask<AccessRequest> ExpireRenewImpersonationContextTokensAsync(Guid impersonationContextId) =>
        TryCatch(async () =>
        {
            ValidateOnExpireRenewImpersonationContextTokens(impersonationContextId);

            AccessRequest retrievedImpersonationContext = await this.persistanceOrchestrationService
                .RetrieveImpersonationContextByIdAsync(impersonationContextId);

            bool isPreviouslyApproved = retrievedImpersonationContext.ImpersonationContext.IsApproved;

            if (!isPreviouslyApproved)
            {
                retrievedImpersonationContext.ImpersonationContext.IsApproved = true;

                retrievedImpersonationContext.ImpersonationContext.UpdatedDate =
                    await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

                await this.persistanceOrchestrationService
                    .PersistImpersonationContextAsync(retrievedImpersonationContext);
            }

            AccessRequest tokensAccessRequest = await this.identificationOrchestrationService
                .ExpireRenewImpersonationContextTokensAsync(retrievedImpersonationContext, isPreviouslyApproved);

            await this.persistanceOrchestrationService.SendGeneratedTokensNotificationAsync(tokensAccessRequest);

            return tokensAccessRequest;
        });

        virtual async internal ValueTask<AccessRequest> ConvertCsvIdentificationRequestToIdentificationRequest(
            AccessRequest accessRequest)
        {
            string data = Encoding.UTF8.GetString(accessRequest.CsvIdentificationRequest.Data);

            Dictionary<string, int> fieldMappings =
                new Dictionary<string, int>
                {
                    {
                        nameof(CsvIdentificationItem.Identifier),
                        accessRequest.CsvIdentificationRequest.IdentifierColumnIndex
                    }
                };

            var mappedItems =
                await this.csvHelperBroker.MapCsvToObjectAsync<CsvIdentificationItem>(
                    data: data,
                    hasHeaderRecord: accessRequest.CsvIdentificationRequest.HasHeaderRecord,
                    fieldMappings: fieldMappings);

            var identificationItems = new List<IdentificationItem>();

            for (var index = 0; index < mappedItems.Count; index++)
            {
                var identificationItem = new IdentificationItem
                {
                    HasAccess = false,
                    Identifier = mappedItems[index].Identifier,
                    IsReidentified = false,
                    Message = string.Empty,
                    RowNumber = accessRequest.CsvIdentificationRequest.HasHeaderRecord
                        ? (index + 2).ToString()
                        : (index + 1).ToString()
                };

                identificationItems.Add(identificationItem);
            }

            ValidateCsvData(identificationItems);
            accessRequest.IdentificationRequest = new IdentificationRequest();
            accessRequest.IdentificationRequest.Id = accessRequest.CsvIdentificationRequest.Id;
            accessRequest.IdentificationRequest.IdentificationItems = identificationItems;
            accessRequest.IdentificationRequest.EntraUserId = accessRequest.CsvIdentificationRequest.RecipientEntraUserId;
            accessRequest.IdentificationRequest.Email = accessRequest.CsvIdentificationRequest.RecipientEmail;
            accessRequest.IdentificationRequest.JobTitle = accessRequest.CsvIdentificationRequest.RecipientJobTitle;
            accessRequest.IdentificationRequest.DisplayName = accessRequest.CsvIdentificationRequest.RecipientDisplayName;
            accessRequest.IdentificationRequest.GivenName = accessRequest.CsvIdentificationRequest.RecipientFirstName;
            accessRequest.IdentificationRequest.Surname = accessRequest.CsvIdentificationRequest.RecipientLastName;
            accessRequest.IdentificationRequest.Reason = accessRequest.CsvIdentificationRequest.Reason;
            accessRequest.IdentificationRequest.Organisation = accessRequest.CsvIdentificationRequest.Organisation;

            return accessRequest;
        }

        virtual internal async ValueTask<AccessRequest> ConvertIdentificationRequestToCsvIdentificationRequest(
            AccessRequest accessRequest)
        {
            bool hasHeaderRecord = accessRequest.CsvIdentificationRequest.HasHeaderRecord;
            List<IdentificationItem> identificationItems = accessRequest.IdentificationRequest.IdentificationItems;
            string data = Encoding.UTF8.GetString(accessRequest.CsvIdentificationRequest.Data);

            var mappedItems =
                 await this.csvHelperBroker.MapCsvToObjectAsync<dynamic>(
                     data: data,
                     hasHeaderRecord: hasHeaderRecord,
                     fieldMappings: null);

            var reidentifiedItems = UpdateIdentifierColumnValues(mappedItems,
                identificationItems,
                accessRequest.CsvIdentificationRequest.IdentifierColumnIndex);

            string csvIdentificationRequestData =
                await this.csvHelperBroker.MapObjectToCsvAsync(
                    reidentifiedItems,
                    addHeaderRecord: hasHeaderRecord,
                    fieldMappings: null,
                    shouldAddTrailingComma: false);

            byte[] csvIdentificationRequestDataByteArray = Encoding.UTF8.GetBytes(csvIdentificationRequestData);
            accessRequest.CsvIdentificationRequest.Data = csvIdentificationRequestDataByteArray;

            return accessRequest;
        }

        virtual internal List<dynamic> UpdateIdentifierColumnValues(
            List<dynamic> originalRecords,
            List<IdentificationItem> updatedValues,
            int identifierColumnIndex)
        {
            var reIdentifiedRecords = originalRecords.DeepClone();

            for (int i = 0; i < reIdentifiedRecords.Count; i++)
            {
                if (reIdentifiedRecords[i] is IDictionary<string, object> expandoObject)
                {
                    var key = expandoObject.Keys.ElementAt(identifierColumnIndex);
                    expandoObject[key] = updatedValues[i].Identifier;
                }
            }

            return reIdentifiedRecords;
        }

        private IdentificationRequest OverrideIdentificationRequestUserDetails(
            IdentificationRequest identificationRequest,
            EntraUser currentUser,
            string reason)
        {
            identificationRequest.EntraUserId = currentUser.EntraUserId;
            identificationRequest.Email = currentUser.Email;
            identificationRequest.JobTitle = currentUser.JobTitle;
            identificationRequest.DisplayName = currentUser.DisplayName;
            identificationRequest.GivenName = currentUser.GivenName;
            identificationRequest.Surname = currentUser.Surname;
            identificationRequest.Reason = reason;

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

        virtual internal async ValueTask<(
            string LandingFilepath, string PickupFilepath, string ErrorFilepath, Guid ImpersonationContextId)>
            ExtractFromFilepath(string filepath)
        {
            string[] filepathParts = filepath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            string container = filepathParts[0];
            Guid impersonationContextId = Guid.Parse(filepathParts[1]);
            string landingFilepath = string.Join("/", filepathParts, 2, filepathParts.Length - 2);
            DateTimeOffset dateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            string timestamp = dateTimeOffset.ToString("yyyyMMddHHmms");
            string nameWithoutExtension = Path.GetFileNameWithoutExtension(landingFilepath);

            string pickFilepath = landingFilepath
                .Replace(projectStorageConfiguration.LandingFolder, projectStorageConfiguration.PickupFolder)
                .Replace(nameWithoutExtension, nameWithoutExtension + "_" + timestamp);

            string errorFilepath = landingFilepath
                .Replace(projectStorageConfiguration.LandingFolder, projectStorageConfiguration.ErrorFolder)
                .Replace(nameWithoutExtension, nameWithoutExtension + "_" + timestamp);

            return (landingFilepath, pickFilepath, errorFilepath, impersonationContextId);
        }
    }
}
