// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.PdsDatas.Exceptions
{
    public class FailedOperationPdsDataException : Xeption
    {
        public FailedOperationPdsDataException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
