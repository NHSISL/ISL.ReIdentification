// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.Lookups.Exceptions
{
    public class AlreadyExistsLookupException : Xeption
    {
        public AlreadyExistsLookupException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}