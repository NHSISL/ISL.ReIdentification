// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.UserAccesses.Exceptions
{
    public class InvalidUserAccessException : Xeption
    {
        public InvalidUserAccessException(string message)
            : base(message)
        { }
    }
}
