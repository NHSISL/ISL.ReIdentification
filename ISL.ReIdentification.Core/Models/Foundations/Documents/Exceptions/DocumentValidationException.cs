// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.Documents.Exceptions
{
    public class DocumentValidationException : Xeption
    {
        public DocumentValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
