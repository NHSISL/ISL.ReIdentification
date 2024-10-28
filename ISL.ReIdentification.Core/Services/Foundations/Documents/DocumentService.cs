// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Storages.Blob;

namespace ISL.ReIdentification.Core.Services.Foundations.Documents
{
    public class DocumentService : IDocumentService
    {
        private readonly IBlobStorageBroker blobStorageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public DocumentService(
            IBlobStorageBroker blobStorageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.blobStorageBroker = blobStorageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask AddDocumentAsync(Stream input, string fileName, string container) =>
            throw new NotImplementedException();

        public ValueTask RetrieveDocumentByFileNameAsync(Stream output, string fileName, string container) =>
             throw new NotImplementedException();

        public ValueTask RemoveDocumentByFileNameAsync(string fileName, string container) =>
           throw new NotImplementedException();

        public ValueTask<string> GetDownloadLinkAsync(string fileName, string container) =>
           throw new NotImplementedException();
    }
}
