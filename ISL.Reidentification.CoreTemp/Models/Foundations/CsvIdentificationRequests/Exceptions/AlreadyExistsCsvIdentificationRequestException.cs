// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions
{
    public class AlreadyExistsCsvIdentificationRequestException : Xeption
    {
        public AlreadyExistsCsvIdentificationRequestException(
            string message,
            Exception innerException,
            IDictionary data)
                : base(message, innerException, data)
        { }
    }
}
