// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.Documents.Exceptions
{
    public class InvalidArgumentDocumentException : Xeption
    {
        public InvalidArgumentDocumentException(string message)
            : base(message)
        { }
    }
}
