// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.UserAccesses.Exceptions
{
    public class NullUserAccessException : Xeption
    {
        public NullUserAccessException(string message)
            : base(message)
        { }
    }
}
