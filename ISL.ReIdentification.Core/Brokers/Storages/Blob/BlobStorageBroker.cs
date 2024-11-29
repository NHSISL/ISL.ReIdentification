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

        /// <summary>
        /// Creates a file in the storage container.
        /// </summary>
        /// <param name="input">The <see cref="Stream"/> containing the file data to be uploaded.</param>
        /// <param name="fileName">The name of the file to create in the container.</param>
        /// <param name="container">The name of the storage container where the file will be stored.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        public async ValueTask InsertFileAsync(Stream input, string fileName, string container) =>
            await this.storageAbstractionProvider.CreateFileAsync(input, fileName, container);

        /// <summary>
        /// Retrieves a file from the storage container.
        /// </summary>
        /// <param name="output">The <see cref="Stream"/> containing the file data to be downloaded.</param>
        /// <param name="fileName">The name of the file to retrieve in the container.</param>
        /// <param name="container">The name of the storage container where the file is located.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        public async ValueTask SelectByFileNameAsync(Stream output, string fileName, string container) =>
            await this.storageAbstractionProvider.RetrieveFileAsync(output, fileName, container);

        public async ValueTask DeleteFileAsync(string fileName, string container) =>
            throw new NotImplementedException();

        public async ValueTask<string> GetDownloadLinkAsync(
                string fileName,
                string container,
                DateTimeOffset expiresOn) =>
            throw new NotImplementedException();
    }
}