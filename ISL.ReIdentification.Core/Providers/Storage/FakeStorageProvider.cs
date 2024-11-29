// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ISL.Providers.Storages.Abstractions;

namespace ISL.ReIdentification.Core.Providers.Storage
{
    public class FakeStorageProvider : IStorageProvider
    {
        public ValueTask CreateAndAssignAccessPoliciesToContainerAsync(string container, List<string> policyNames)
        {
            throw new NotImplementedException();
        }

        public ValueTask CreateContainerAsync(string container)
        {
            throw new NotImplementedException();
        }

        public ValueTask<string> CreateDirectorySasTokenAsync(string container, string directoryPath, string accessPolicyIdentifier, DateTimeOffset expiresOn)
        {
            throw new NotImplementedException();
        }

        public ValueTask CreateFileAsync(Stream input, string fileName, string container)
        {
            throw new NotImplementedException();
        }

        public ValueTask CreateFolderInContainerAsync(string container, string folder)
        {
            throw new NotImplementedException();
        }

        public ValueTask DeleteFileAsync(string fileName, string container)
        {
            throw new NotImplementedException();
        }

        public ValueTask<string> GetAccessTokenAsync(string path, string container, string accessLevel, DateTimeOffset expiresOn)
        {
            throw new NotImplementedException();
        }

        public ValueTask<string> GetDownloadLinkAsync(string fileName, string container, DateTimeOffset expiresOn)
        {
            throw new NotImplementedException();
        }

        public ValueTask<List<string>> ListFilesInContainerAsync(string container)
        {
            throw new NotImplementedException();
        }

        public ValueTask RemoveAccessPoliciesFromContainerAsync(string container)
        {
            throw new NotImplementedException();
        }

        public ValueTask<List<string>> RetrieveAllAccessPoliciesFromContainerAsync(string container)
        {
            throw new NotImplementedException();
        }

        public ValueTask RetrieveFileAsync(Stream output, string fileName, string container)
        {
            throw new NotImplementedException();
        }
    }
}
