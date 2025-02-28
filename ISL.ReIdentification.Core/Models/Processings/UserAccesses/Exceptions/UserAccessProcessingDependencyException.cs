// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Processings.UserAccesses.Exceptions
{
    public class UserAccessProcessingDependencyException : Xeption
    {
        public UserAccessProcessingDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
