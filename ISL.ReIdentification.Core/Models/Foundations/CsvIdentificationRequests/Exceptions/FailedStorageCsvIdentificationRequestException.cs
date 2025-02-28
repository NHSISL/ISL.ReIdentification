// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions
{
    public class FailedStorageCsvIdentificationRequestException : Xeption
    {
        public FailedStorageCsvIdentificationRequestException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
