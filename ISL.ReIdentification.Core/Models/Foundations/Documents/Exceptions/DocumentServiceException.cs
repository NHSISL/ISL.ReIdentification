// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.Documents.Exceptions
{
    public class DocumentServiceException : Xeption
    {
        public DocumentServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
