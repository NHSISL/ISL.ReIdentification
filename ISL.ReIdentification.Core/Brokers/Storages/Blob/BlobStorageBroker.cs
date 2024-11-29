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

        /// <summary>
        /// Asynchronously deletes a file from the specified storage container.
        /// </summary>
        /// <param name="fileName">The name of the file to delete.</param>
        /// <param name="container">The name of the storage container where the file is located.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        public async ValueTask DeleteFileAsync(string fileName, string container) =>
           await this.storageAbstractionProvider.DeleteFileAsync(fileName, container);

        /// <summary>
        /// Asynchronously generates a download link for a file in the specified storage container.
        /// </summary>
        /// <param name="fileName">The name of the file to generate a download link for.</param>
        /// <param name="container">The name of the storage container where the file is located.</param>
        /// <param name="expiresOn">The <see cref="DateTimeOffset"/> indicating when the download link will expire.</param>
        /// <returns>A <see cref="ValueTask{String}"/> containing the download link.</returns>
        public async ValueTask<string> GetDownloadLinkAsync(
            string fileName,
            string container,
            DateTimeOffset expiresOn) =>
            await this.storageAbstractionProvider.GetDownloadLinkAsync(fileName, container, expiresOn);

        /// <summary>
        /// Creates a container in the storage account.
        /// </summary>
        /// <param name="container">The name of the created storage container.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        public async ValueTask CreateContainerAsync(string container) =>
            await this.storageAbstractionProvider.CreateContainerAsync(container);

        /// <summary>
        /// Asynchronously lists all files in the specified storage container.
        /// </summary>
        /// <param name="container">The name of the storage container to list files from.</param>
        /// <returns>A <see cref="ValueTask{List{String}}"/> containing the list of file names.</returns>
        public async ValueTask<List<string>> ListFilesInContainerAsync(string container) =>
            await this.storageAbstractionProvider.ListFilesInContainerAsync(container);
    }
}