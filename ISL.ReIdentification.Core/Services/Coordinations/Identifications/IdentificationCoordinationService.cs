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
using ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
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
            EntraUser currentUser = await this.securityBroker.GetCurrentUserAsync();
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

            EntraUser currentUser = await this.securityBroker.GetCurrentUserAsync();

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

        public ValueTask<AccessRequest> ProcessImpersonationContextRequestAsync(string container, string filepath) =>
        TryCatch(async () =>
        {
            ValidateOnProcessImpersonationContextRequestAsync(container, filepath, this.projectStorageConfiguration);
            var filepathData = await ExtractFromFilepath(filepath);

            AccessRequest accessRequestWithCsvIdentificationRequest =
                await CreateAccessRequestWithCsvIdentificationRequestAsync(
                    container,
                    filepath,
                    filepathData.ErrorFilepath);

            try
            {
                AccessRequest csvConvertedAccessRequest =
                    await ConvertCsvIdentificationRequestToIdentificationRequest(
                        accessRequestWithCsvIdentificationRequest);

                IdentificationRequest currentIdentificationRequest =
                    OverrideIdentificationRequestUserDetails(
                        csvConvertedAccessRequest.IdentificationRequest,
                        csvConvertedAccessRequest.ImpersonationContext);

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

                await this.identificationOrchestrationService.AddDocumentAsync(
                    csvData,
                    filepathData.PickupFilepath,
                    container);

                await this.identificationOrchestrationService.RemoveDocumentByFileNameAsync(
                    filepath,
                    container);

                return reIdentifiedAccessRequest;
            }
            catch (Exception)
            {
                MemoryStream csvData = new MemoryStream(
                    accessRequestWithCsvIdentificationRequest.CsvIdentificationRequest.Data);

                await this.identificationOrchestrationService.AddDocumentAsync(
                    csvData,
                    filepathData.ErrorFilepath,
                    container);

                await this.identificationOrchestrationService.RemoveDocumentByFileNameAsync(
                    filepath,
                    container);

                throw;
            }
        });

        public ValueTask<AccessRequest> ExpireRenewImpersonationContextTokensAsync(Guid impersonationContextId) =>
        TryCatch(async () =>
        {
            ValidateOnExpireRenewImpersonationContextTokens(impersonationContextId);

            AccessRequest retrievedImpersonationContext = await this.persistanceOrchestrationService
                .RetrieveImpersonationContextByIdAsync(impersonationContextId);

            if (retrievedImpersonationContext.ImpersonationContext.IsApproved == false)
            {
                throw new InvalidAccessIdentificationCoordinationException(message: "Project not approved. " +
                    "Please contact responsible person to approve this request.");
            }

            AccessRequest tokensAccessRequest = await this.identificationOrchestrationService
                .ExpireRenewImpersonationContextTokensAsync(retrievedImpersonationContext);

            await this.persistanceOrchestrationService.SendGeneratedTokensNotificationAsync(tokensAccessRequest);

            return tokensAccessRequest;
        });

        public ValueTask ImpersonationContextApprovalAsync(Guid impersonationContextId, bool isApproved) =>
        TryCatch(async () =>
        {
            ValidateOnImpersonationContextApproval(impersonationContextId);

            AccessRequest retrievedImpersonationContext = await this.persistanceOrchestrationService
                .RetrieveImpersonationContextByIdAsync(impersonationContextId);

            EntraUser currentEntraUser = await this.securityBroker.GetCurrentUserAsync();

            ValidateUserAccessOnImpersonationContextApproval(
                retrievedImpersonationContext.ImpersonationContext.ResponsiblePersonEntraUserId.ToUpper(),
                currentEntraUser.EntraUserId.ToUpper());

            bool isPreviouslyApproved = retrievedImpersonationContext.ImpersonationContext.IsApproved;

            if (isPreviouslyApproved == isApproved)
            {
                return;
            }

            retrievedImpersonationContext.ImpersonationContext.IsApproved = isApproved;

            retrievedImpersonationContext.ImpersonationContext.UpdatedDate =
                await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            AccessRequest updatedImpersonationContext = await this.persistanceOrchestrationService
                .PersistImpersonationContextAsync(retrievedImpersonationContext);

            if (isApproved == false)
            {
                AccessRequest tokensAccessRequest = await this.identificationOrchestrationService
                    .ExpireRenewImpersonationContextTokensAsync(retrievedImpersonationContext);

                await this.persistanceOrchestrationService.SendApprovalNotificationAsync(tokensAccessRequest);

                return;
            }

            await this.persistanceOrchestrationService
                .SendApprovalNotificationAsync(updatedImpersonationContext);
        });

        virtual async internal ValueTask<AccessRequest> ConvertCsvIdentificationRequestToIdentificationRequest(
            AccessRequest accessRequest)
        {
            string data = Encoding.UTF8.GetString(accessRequest.CsvIdentificationRequest.Data);

            if (data == string.Empty)
            {
                throw new InvalidCsvIdentificationCoordinationException(
                    message: "The uploaded file is empty.");
            }

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

                    Identifier = string.IsNullOrEmpty(mappedItems[index].Identifier)
                        ? mappedItems[index].Identifier
                        : mappedItems[index].Identifier,

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
            identificationRequest.EntraUserId = impersonationContext.ResponsiblePersonEntraUserId;
            identificationRequest.Email = impersonationContext.ResponsiblePersonEmail;
            identificationRequest.JobTitle = impersonationContext.ResponsiblePersonJobTitle;
            identificationRequest.DisplayName = impersonationContext.ResponsiblePersonDisplayName;
            identificationRequest.GivenName = impersonationContext.ResponsiblePersonFirstName;
            identificationRequest.Surname = impersonationContext.ResponsiblePersonLastName;

            return identificationRequest;
        }

        virtual internal async ValueTask<(
            string LandingFilepath, string PickupFilepath, string ErrorFilepath)>
            ExtractFromFilepath(string filepath)
        {
            DateTimeOffset dateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            string timestamp = dateTimeOffset.ToString("yyyyMMddHHmms");
            string nameWithoutExtension = Path.GetFileNameWithoutExtension(filepath);

            string pickFilepath = filepath
                .Replace(projectStorageConfiguration.LandingFolder, projectStorageConfiguration.PickupFolder)
                .Replace(nameWithoutExtension, nameWithoutExtension + "_" + timestamp);

            string errorFilepath = filepath
                .Replace(projectStorageConfiguration.LandingFolder, projectStorageConfiguration.ErrorFolder)
                .Replace(nameWithoutExtension, nameWithoutExtension + "_" + timestamp);

            return (filepath, pickFilepath, errorFilepath);
        }

        virtual internal async ValueTask<AccessRequest> CreateAccessRequestWithCsvIdentificationRequestAsync(
            string container,
            string filepath,
            string errorFilepath)
        {
            Stream retrievedFileStream = new MemoryStream();

            await this.identificationOrchestrationService
                .RetrieveDocumentByFileNameAsync(retrievedFileStream, filepath, container);

            AccessRequest retrievedAccessRequest =
                await this.persistanceOrchestrationService
                    .RetrieveImpersonationContextByIdAsync(Guid.Parse(container));

            byte[] fileData = ReadAllBytesFromStream(retrievedFileStream);
            AccessRequest accessRequest = retrievedAccessRequest;
            accessRequest.CsvIdentificationRequest = new CsvIdentificationRequest();

            try
            {
                accessRequest.CsvIdentificationRequest.Data = fileData;

                accessRequest.CsvIdentificationRequest.IdentifierColumnIndex = GetColumnIndexFromStream(
                        retrievedFileStream,
                        retrievedAccessRequest.ImpersonationContext.IdentifierColumn);

                accessRequest.CsvIdentificationRequest.HasHeaderRecord = true;
                accessRequest.CsvIdentificationRequest.Id = accessRequest.ImpersonationContext.Id;

                accessRequest.CsvIdentificationRequest.RecipientEntraUserId =
                    accessRequest.ImpersonationContext.RequesterEntraUserId;

                accessRequest.CsvIdentificationRequest.RecipientEmail =
                    accessRequest.ImpersonationContext.RequesterEmail;

                accessRequest.CsvIdentificationRequest.RecipientJobTitle =
                    accessRequest.ImpersonationContext.RequesterJobTitle;

                accessRequest.CsvIdentificationRequest.RecipientDisplayName =
                    accessRequest.ImpersonationContext.RequesterDisplayName;

                accessRequest.CsvIdentificationRequest.RecipientFirstName =
                    accessRequest.ImpersonationContext.RequesterFirstName;

                accessRequest.CsvIdentificationRequest.RecipientLastName =
                    accessRequest.ImpersonationContext.RequesterLastName;

                accessRequest.CsvIdentificationRequest.Reason = accessRequest.ImpersonationContext.Reason;
                accessRequest.CsvIdentificationRequest.Organisation = accessRequest.ImpersonationContext.Organisation;

                return accessRequest;
            }
            catch (Exception)
            {
                MemoryStream csvData = new MemoryStream(fileData);

                await this.identificationOrchestrationService.AddDocumentAsync(
                    csvData,
                    errorFilepath,
                    container);

                await this.identificationOrchestrationService.RemoveDocumentByFileNameAsync(
                    filepath,
                    container);

                throw;
            }
        }

        private static byte[] ReadAllBytesFromStream(Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private static int GetColumnIndexFromStream(Stream stream, string columnName)
        {
            stream.Position = 0;

            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                string headerLine = reader.ReadLine();
                string[] headers = headerLine?.Split(",") ?? Array.Empty<string>();
                int columnIndex = Array.IndexOf(headers, columnName);

                if (columnIndex != -1)
                {
                    return columnIndex;
                }
            }

            throw new InvalidCsvIdentificationCoordinationException(
                message: "Invalid csv file. Please check that the provided file has a column name " +
                    "that matches the identifier column name given when creating the project.");
        }
    }
}
