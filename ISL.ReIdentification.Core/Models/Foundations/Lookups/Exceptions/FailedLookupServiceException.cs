// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.Lookups.Exceptions
{
    public class FailedLookupServiceException : Xeption
    {
        public FailedLookupServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}