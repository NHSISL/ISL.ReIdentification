// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.UserAccesses.Exceptions
{
    public class AlreadyExistsUserAccessException : Xeption
    {
        public AlreadyExistsUserAccessException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
