// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.UserAccesses.Exceptions
{
    public class FailedOperationUserAccessException : Xeption
    {
        public FailedOperationUserAccessException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
