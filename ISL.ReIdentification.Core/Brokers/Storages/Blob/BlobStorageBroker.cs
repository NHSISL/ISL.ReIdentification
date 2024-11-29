// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using ISL.Providers.Storages.Abstractions;

namespace ISL.ReIdentification.Core.Brokers.Storages.Blob
{
    public class BlobStorageBroker : IBlobStorageBroker
    {
        private readonly IStorageAbstractionProvider storageAbstractionProvider;

        public BlobStorageBroker(
            IStorageAbstractionProvider storageAbstractionProvider)
        {
            this.storageAbstractionProvider = storageAbstractionProvider;
        }

        public async ValueTask InsertFileAsync(Stream input, string fileName, string container) =>
            throw new NotImplementedException();

        public async ValueTask SelectByFileNameAsync(Stream output, string fileName, string container) =>
            throw new NotImplementedException();

        public async ValueTask DeleteFileAsync(string fileName, string container) =>
            throw new NotImplementedException();

        public async ValueTask<string> GetDownloadLinkAsync(
                string fileName,
                string container,
                DateTimeOffset expiresOn) =>
            throw new NotImplementedException();
    }
}