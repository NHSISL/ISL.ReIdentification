// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Processings.UserAccesses.Exceptions
{
    public class UserAccessProcessingServiceException : Xeption
    {
        public UserAccessProcessingServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
