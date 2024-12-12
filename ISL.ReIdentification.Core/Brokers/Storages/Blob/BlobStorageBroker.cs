// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ISL.Providers.Storages.Abstractions;

namespace ISL.ReIdentification.Core.Brokers.Storages.Blob
{
    public class BlobStorageBroker : IBlobStorageBroker
    {
        private readonly IStorageAbstractionProvider storageAbstractionProvider;

        public BlobStorageBroker(IStorageAbstractionProvider storageAbstractionProvider) =>
            this.storageAbstractionProvider = storageAbstractionProvider;

        public async ValueTask InsertFileAsync(Stream input, string fileName, string container) =>
            await this.storageAbstractionProvider.CreateFileAsync(input, fileName, container);

        public async ValueTask SelectByFileNameAsync(Stream output, string fileName, string container) =>
            await this.storageAbstractionProvider.RetrieveFileAsync(output, fileName, container);

        public async ValueTask DeleteFileAsync(string fileName, string container) =>
           await this.storageAbstractionProvider.DeleteFileAsync(fileName, container);

        public async ValueTask<string> GetDownloadLinkAsync(
            string fileName,
            string container,
            DateTimeOffset expiresOn) =>
            await this.storageAbstractionProvider.GetDownloadLinkAsync(fileName, container, expiresOn);

        public async ValueTask CreateContainerAsync(string container) =>
            await this.storageAbstractionProvider.CreateContainerAsync(container);

        public async ValueTask<List<string>> ListFilesInContainerAsync(string container) =>
            await this.storageAbstractionProvider.ListFilesInContainerAsync(container);

        public async ValueTask<string> CreateDirectorySasTokenAsync(
            string container, 
            string directoryPath, 
            string accessPolicyIdentifier, 
            DateTimeOffset expiresOn)
        {
            return await this.storageAbstractionProvider.CreateDirectorySasTokenAsync(
                container, directoryPath, accessPolicyIdentifier, expiresOn);
        }

        public async ValueTask<string> GetAccessTokenAsync(
            string path, 
            string container, 
            string accessLevel, 
            DateTimeOffset expiresOn) =>
                await this.storageAbstractionProvider.GetAccessTokenAsync(path, container, accessLevel, expiresOn);

        public async ValueTask<List<string>> RetrieveAllAccessPoliciesFromContainerAsync(string container) =>
            await this.storageAbstractionProvider.RetrieveAllAccessPoliciesFromContainerAsync(container);

        public async ValueTask CreateAndAssignAccessPoliciesToContainerAsync(
            string container, 
            List<string> policyNames) =>
            await this.storageAbstractionProvider.CreateAndAssignAccessPoliciesToContainerAsync(container, policyNames);

        public async ValueTask RemoveAccessPoliciesFromContainerAsync(string container) =>
            await this.storageAbstractionProvider.RemoveAccessPoliciesFromContainerAsync(container);

        public async ValueTask RemoveAccessPolicyByNameAsync(string container, string policyName) =>
            new NotImplementedException();

        public async ValueTask CreateFolderInContainerAsync(string container, string folder) =>
            await this.storageAbstractionProvider.CreateFolderInContainerAsync(container, folder);
    }
}