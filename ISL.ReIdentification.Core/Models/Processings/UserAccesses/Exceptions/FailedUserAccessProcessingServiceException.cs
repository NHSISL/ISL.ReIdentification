// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Processings.UserAccesses.Exceptions
{
    public class FailedUserAccessProcessingServiceException : Xeption
    {
        public FailedUserAccessProcessingServiceException(string message, Exception innerException)
          : base(message, innerException)
        { }
    }
}
