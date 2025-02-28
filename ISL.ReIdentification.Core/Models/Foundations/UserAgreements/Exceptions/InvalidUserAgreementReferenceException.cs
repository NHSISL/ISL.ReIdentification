// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions
{
    public class InvalidUserAgreementReferenceException : Xeption
    {
        public InvalidUserAgreementReferenceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}