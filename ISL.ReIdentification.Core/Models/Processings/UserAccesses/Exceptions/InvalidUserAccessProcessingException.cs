// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Processings.UserAccesses.Exceptions
{
    public class InvalidUserAccessProcessingException : Xeption
    {
        public InvalidUserAccessProcessingException(string message)
            : base(message)
        { }
    }
}
