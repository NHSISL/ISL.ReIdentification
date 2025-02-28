// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.Documents.Exceptions
{
    public class InvalidPermissionDocumentException : Xeption
    {
        public InvalidPermissionDocumentException(string message)
            : base(message)
        { }
    }
}
