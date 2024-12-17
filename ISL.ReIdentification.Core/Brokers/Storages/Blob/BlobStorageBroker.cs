// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ISL.Providers.Storages.Abstractions;
using ISL.Providers.Storages.Abstractions.Models;

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

        public async ValueTask<string> CreateSasTokenAsync(
            string container,
            string path,
            string accessPolicyIdentifier,
            DateTimeOffset expiresOn)
        {
            return await this.storageAbstractionProvider.CreateSasTokenAsync(
                container,
                path,
                accessPolicyIdentifier,
                expiresOn);
        }

        public async ValueTask<string> CreateSasTokenAsync(
            string container,
            string path,
            DateTimeOffset expiresOn,
            List<string> permissions)
        {
            return await this.storageAbstractionProvider.CreateSasTokenAsync(
                container,
                path,
                expiresOn,
                permissions);
        }

        public async ValueTask<List<string>> RetrieveListOfAllAccessPoliciesAsync(string container) =>
            await this.storageAbstractionProvider.RetrieveListOfAllAccessPoliciesAsync(container);

        public async ValueTask<List<Policy>> RetrieveAllAccessPoliciesAsync(string container) =>
            await this.storageAbstractionProvider.RetrieveAllAccessPoliciesAsync(container);

        public async ValueTask<Policy> RetrieveAccessPolicyByNameAsync(string container, string policyName) =>
            await this.storageAbstractionProvider.RetrieveAccessPolicyByNameAsync(container, policyName);

        public async ValueTask CreateAndAssignAccessPoliciesAsync(string container, List<Policy> policies) =>
            await this.storageAbstractionProvider.CreateAndAssignAccessPoliciesAsync(container, policies);

        public async ValueTask RemoveAllAccessPoliciesAsync(string container) =>
            await this.storageAbstractionProvider.RemoveAllAccessPoliciesAsync(container);

        public async ValueTask RemoveAccessPolicyByNameAsync(string container, string policyName) =>
            await this.storageAbstractionProvider.RemoveAccessPolicyByNameAsync(container, policyName);

        public async ValueTask CreateFolderInContainerAsync(string container, string folder) =>
            await this.storageAbstractionProvider.CreateFolderInContainerAsync(container, folder);
    }
}