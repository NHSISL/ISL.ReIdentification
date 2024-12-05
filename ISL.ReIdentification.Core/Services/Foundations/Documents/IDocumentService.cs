// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

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
        ValueTask<string> GetDownloadLinkAsync(string fileName, string container);
        ValueTask<List<string>> RetrieveAllAccessPoliciesFromContainerAsync(string container);
        ValueTask RemoveAllAccessPoliciesFromContainerAsync(string container);
    }
}
