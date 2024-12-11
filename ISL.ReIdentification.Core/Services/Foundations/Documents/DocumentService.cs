// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Storages.Blob;

namespace ISL.ReIdentification.Core.Services.Foundations.Documents
{
    public partial class DocumentService : IDocumentService
    {
        private readonly IBlobStorageBroker blobStorageBroker;
        private readonly ILoggingBroker loggingBroker;

        public DocumentService(
            IBlobStorageBroker blobStorageBroker,
            ILoggingBroker loggingBroker)
        {
            this.blobStorageBroker = blobStorageBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask AddDocumentAsync(Stream input, string fileName, string container) =>
        TryCatch(async () =>
        {
            ValidateOnAddDocument(input, fileName, container);
            await this.blobStorageBroker.InsertFileAsync(input, fileName, container);
        });

        public ValueTask RetrieveDocumentByFileNameAsync(Stream output, string fileName, string container) =>
        TryCatch(async () =>
        {
            ValidateOnRetrieveDocumentByFileName(output, fileName, container);
            await this.blobStorageBroker.SelectByFileNameAsync(output, fileName, container);
        });

        public ValueTask RemoveDocumentByFileNameAsync(string fileName, string container) =>
        TryCatch(async () =>
        {
            ValidateOnRemoveDocumentByFileName(fileName, container);
            await this.blobStorageBroker.DeleteFileAsync(fileName, container);
        });

        public ValueTask<string> GetDownloadLinkAsync(string fileName, string container) =>
           throw new NotImplementedException();

        public ValueTask<List<string>> RetrieveAllAccessPoliciesFromContainerAsync(string container) =>
        TryCatch(async () =>
        {
            ValidateStorageArgumentsOnRetrieveAccessPolicies(container);

            return await this.blobStorageBroker.RetrieveAllAccessPoliciesFromContainerAsync(container);
        });

        public ValueTask<List<string>> ListFilesInContainerAsync(string container) =>
        TryCatch(async () => 
        {
            ValidateOnListFilesInContainer(container);

            return await this.blobStorageBroker.ListFilesInContainerAsync(container);
        });

        public ValueTask RemoveAllAccessPoliciesFromContainerAsync(string container) =>
        TryCatch(async () =>
        {
            ValidateStorageArgumentsOnRemoveAccessPolicies(container);
            await this.blobStorageBroker.RemoveAccessPoliciesFromContainerAsync(container);
        });

        public ValueTask AddContainerAsync(string container) =>
        TryCatch(async () =>
        {
            ValidateOnAddContainer(container);
            await this.blobStorageBroker.CreateContainerAsync(container);
        });

        public ValueTask AddFolderAsync(string container, string folder) =>
        TryCatch(async () =>
        {
            ValidateOnAddFolder(container, folder);
            await this.blobStorageBroker.CreateFolderInContainerAsync(container, folder);
        });

        public ValueTask<string> GetDownloadLinkAsync(string fileName, string container, DateTimeOffset expiresOn) =>
        TryCatch(async () =>
        {
            ValidateOnGetDownloadLink(fileName, container, expiresOn);
            
            return await this.blobStorageBroker.GetDownloadLinkAsync(fileName, container, expiresOn);
        });

        public ValueTask<string> CreateDirectorySasTokenAsync(
            string container, string directoryPath, string accessPolicyIdentifier, DateTimeOffset expiresOn) =>
        TryCatch(async () =>
        {
            ValidateOnCreateDirectorySasToken(container, directoryPath, accessPolicyIdentifier, expiresOn);

            return await this.blobStorageBroker
                .CreateDirectorySasTokenAsync(container, directoryPath, accessPolicyIdentifier, expiresOn);
        });
    }
}