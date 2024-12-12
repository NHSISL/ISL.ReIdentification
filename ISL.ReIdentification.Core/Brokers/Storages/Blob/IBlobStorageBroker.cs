// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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

        ValueTask<string> CreateDirectorySasTokenAsync(
            string container, string directoryPath, string accessPolicyIdentifier, DateTimeOffset expiresOn);

        ValueTask<string> GetAccessTokenAsync(
            string path, string container, string accessLevel, DateTimeOffset expiresOn);

        ValueTask<List<string>> RetrieveAllAccessPoliciesFromContainerAsync(string container);
        ValueTask CreateAndAssignAccessPoliciesToContainerAsync(string container, List<string> policyNames);
        ValueTask RemoveAccessPoliciesFromContainerAsync(string container);
        ValueTask RemoveAccessPolicyByNameAsync(string container, string policyName);
        ValueTask CreateFolderInContainerAsync(string container, string folder);
    }
}