// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Processings.UserAccesses.Exceptions
{
    public class UserAccessProcessingDependencyValidationException : Xeption
    {
        public UserAccessProcessingDependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
