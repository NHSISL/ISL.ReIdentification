// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Storages.Blob;

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

        public ValueTask<string> GetDownloadLinkAsync(string fileName, string container) =>
           throw new NotImplementedException();

        public ValueTask<List<string>> RetrieveAllAccessPoliciesFromContainerAsync(string container) =>
        throw new NotImplementedException();
    }
}
