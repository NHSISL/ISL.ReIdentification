// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ISL.Providers.Storages.Abstractions.Models;

namespace ISL.ReIdentification.Core.Services.Foundations.Documents
{
    public interface IDocumentService
    {
        ValueTask AddDocumentAsync(Stream input, string fileName, string container);
        ValueTask RetrieveDocumentByFileNameAsync(Stream output, string fileName, string container);
        ValueTask RemoveDocumentByFileNameAsync(string filename, string container);
        ValueTask CreateAndAssignAccessPoliciesAsync(string container, List<Policy> policies);
        ValueTask<List<string>> RetrieveListOfAllAccessPoliciesAsync(string container);
        ValueTask<List<Policy>> RetrieveAllAccessPoliciesAsync(string container);
        ValueTask<Policy> RetrieveAccessPolicyByNameAsync(string container, string policyName);
        ValueTask RemoveAllAccessPoliciesAsync(string container);
        ValueTask RemoveAccessPolicyByNameAsync(string container, string policyName);
        ValueTask<List<string>> ListFilesInContainerAsync(string container);
        ValueTask AddContainerAsync(string container);
        ValueTask AddFolderAsync(string container, string folder);
        ValueTask<string> GetDownloadLinkAsync(string fileName, string container, DateTimeOffset expiresOn);

        ValueTask<string> CreateSasTokenAsync(
            string container,
            string path,
            string accessPolicyIdentifier,
            DateTimeOffset expiresOn);

        ValueTask<string> CreateSasTokenAsync(
            string container,
            string path,
            DateTimeOffset expiresOn,
            List<string> permissions);
    }
}