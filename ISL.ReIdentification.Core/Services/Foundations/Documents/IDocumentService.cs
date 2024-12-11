// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ISL.ReIdentification.Core.Services.Foundations.Documents
{
    public interface IDocumentService
    {
        ValueTask AddDocumentAsync(Stream input, string fileName, string container);
        ValueTask RetrieveDocumentByFileNameAsync(Stream output, string fileName, string container);
        ValueTask RemoveDocumentByFileNameAsync(string filename, string container);
        ValueTask<List<string>> RetrieveAllAccessPoliciesFromContainerAsync(string container);
        ValueTask RemoveAllAccessPoliciesFromContainerAsync(string container);
        ValueTask<List<string>> ListFilesInContainerAsync(string container);
        ValueTask AddContainerAsync (string container);
        ValueTask AddFolderAsync(string container, string folder);
        ValueTask<string> GetDownloadLinkAsync(string fileName, string container, DateTimeOffset expiresOn);

        ValueTask<string> CreateDirectorySasTokenAsync(
            string container, string directoryPath, string accessPolicyIdentifier, DateTimeOffset expiresOn);
    }
}
