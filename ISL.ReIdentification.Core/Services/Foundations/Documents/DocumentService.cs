// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;

namespace ISL.ReIdentification.Core.Services.Foundations.Documents
{
    public class DocumentService : IDocumentService
    {
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
