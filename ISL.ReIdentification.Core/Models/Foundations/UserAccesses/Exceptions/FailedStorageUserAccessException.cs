// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.UserAccesses.Exceptions
{
    public class FailedStorageUserAccessException : Xeption
    {
        public FailedStorageUserAccessException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
