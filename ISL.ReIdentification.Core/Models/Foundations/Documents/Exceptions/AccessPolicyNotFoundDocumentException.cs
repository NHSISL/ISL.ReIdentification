// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.Documents.Exceptions
{
    public class AccessPolicyNotFoundDocumentException : Xeption
    {
        public AccessPolicyNotFoundDocumentException(string message)
            : base(message)
        { }
    }

}
