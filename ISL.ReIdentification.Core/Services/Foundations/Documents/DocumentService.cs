// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ISL.Providers.Storages.Abstractions.Models;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Storages.Blob;
using ISL.ReIdentification.Core.Models.Foundations.Documents;

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

        public async ValueTask CreateAndAssignAccessPoliciesAsync(string container, List<AccessPolicy> accessPolicies)
        {
            List<Policy> policies = ConvertToPolicyList(accessPolicies);
            await this.blobStorageBroker.CreateAndAssignAccessPoliciesAsync(container, policies);
        }


        public ValueTask<List<string>> RetrieveListOfAllAccessPoliciesAsync(string container) =>
        TryCatch(async () =>
        {
            ValidateStorageArgumentsOnRetrieveAccessPolicies(container);

            return await this.blobStorageBroker.RetrieveListOfAllAccessPoliciesAsync(container);
        });

        public ValueTask<List<AccessPolicy>> RetrieveAllAccessPoliciesAsync(string container) =>
        TryCatch(async () =>
        {
            ValidateStorageArgumentsOnRetrieveAccessPolicies(container);

            List<Policy> retrievedPolicies = await this.blobStorageBroker
                .RetrieveAllAccessPoliciesAsync(container);

            List<AccessPolicy> accessPolicies = ConvertToAccessPolicyList(retrievedPolicies);

            return accessPolicies;
        });

        public ValueTask<AccessPolicy> RetrieveAccessPolicyByNameAsync(string container, string policyName) =>
        TryCatch(async () =>
        {
            ValidateStorageArgumentsOnRetrieveAccessPolicyByName(container, policyName);

            List<string> policyNames = await this.blobStorageBroker
                .RetrieveListOfAllAccessPoliciesAsync(container);

            ValidateAccessPolicyExists(policyName, policyNames);

            Policy retrievedPolicy = await this.blobStorageBroker
                .RetrieveAccessPolicyByNameAsync(container, policyName);

            AccessPolicy accessPolicy = ConvertToAccessPolicy(retrievedPolicy);

            return accessPolicy;
        });

        public ValueTask<List<string>> ListFilesInContainerAsync(string container) =>
        TryCatch(async () =>
        {
            ValidateOnListFilesInContainer(container);

            return await this.blobStorageBroker.ListFilesInContainerAsync(container);
        });

        public ValueTask RemoveAllAccessPoliciesAsync(string container) =>
        TryCatch(async () =>
        {
            ValidateStorageArgumentsOnRemoveAccessPolicies(container);
            await this.blobStorageBroker.RemoveAllAccessPoliciesAsync(container);
        });

        public ValueTask RemoveAccessPolicyByNameAsync(string container, string policyName) =>
        TryCatch(async () =>
        {
            ValidateStorageArgumentsOnRemoveAccessPolicyByName(container, policyName);
            await this.blobStorageBroker.RemoveAccessPolicyByNameAsync(container, policyName);
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

        public ValueTask<string> CreateSasTokenAsync(
            string container,
            string path,
            string accessPolicyIdentifier,
            DateTimeOffset expiresOn) =>
        TryCatch(async () =>
        {
            ValidateOnCreateDirectorySasToken(container, path, accessPolicyIdentifier, expiresOn);

            return await this.blobStorageBroker
                .CreateSasTokenAsync(container, path, accessPolicyIdentifier, expiresOn);
        });

        private static AccessPolicy ConvertToAccessPolicy(Policy policy) =>
            new AccessPolicy
            {
                PolicyName = policy.PolicyName,
                Permissions = policy.Permissions,
                StartTime = policy.StartTime,
                ExpiryTime = policy.ExpiryTime,
            };

        private static List<AccessPolicy> ConvertToAccessPolicyList(List<Policy> policies)
        {
            List<AccessPolicy> accessPolicyList = new List<AccessPolicy>();

            foreach (Policy policy in policies)
            {
                AccessPolicy accessPolicy = ConvertToAccessPolicy(policy);
                accessPolicyList.Add(accessPolicy);
            }

            return accessPolicyList;
        }

        private static Policy ConvertToPolicy(AccessPolicy accessPolicy) =>
            new Policy
            {
                PolicyName = accessPolicy.PolicyName,
                Permissions = accessPolicy.Permissions,
                StartTime = accessPolicy.StartTime,
                ExpiryTime = accessPolicy.ExpiryTime,
            };

        private static List<Policy> ConvertToPolicyList(List<AccessPolicy> accessPolicies)
        {
            List<Policy> policyList = new List<Policy>();

            foreach (AccessPolicy accessPolicy in accessPolicies)
            {
                Policy policy = ConvertToPolicy(accessPolicy);
                policyList.Add(policy);
            }

            return policyList;
        }
    };
}