// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions
{
    public class AlreadyExistsUserAgreementException : Xeption
    {
        public AlreadyExistsUserAgreementException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}