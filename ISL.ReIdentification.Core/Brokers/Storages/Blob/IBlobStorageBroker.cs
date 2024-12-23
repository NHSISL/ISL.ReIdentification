// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ISL.Providers.Storages.Abstractions.Models;

namespace ISL.ReIdentification.Core.Brokers.Storages.Blob
{
    public interface IBlobStorageBroker
    {
        ValueTask InsertFileAsync(Stream input, string fileName, string container);
        ValueTask SelectByFileNameAsync(Stream output, string fileName, string container);
        ValueTask DeleteFileAsync(string fileName, string container);
        ValueTask<string> GetDownloadLinkAsync(string fileName, string container, DateTimeOffset expiresOn);
        ValueTask CreateContainerAsync(string container);
        ValueTask<List<string>> ListFilesInContainerAsync(string container);

        ValueTask<string> CreateSasTokenAsync(
            string container,
            string path,
            string accessPolicyIdentifier,
            DateTimeOffset expiresOn);

        ValueTask<List<string>> RetrieveListOfAllAccessPoliciesAsync(string container);
        ValueTask<List<Policy>> RetrieveAllAccessPoliciesAsync(string container);
        ValueTask<Policy> RetrieveAccessPolicyByNameAsync(string container, string policyName);
        ValueTask CreateAndAssignAccessPoliciesAsync(string container, List<Policy> policies);
        ValueTask RemoveAllAccessPoliciesAsync(string container);
        ValueTask RemoveAccessPolicyByNameAsync(string container, string policyName);
        ValueTask CreateFolderInContainerAsync(string container, string folder);
    }
}