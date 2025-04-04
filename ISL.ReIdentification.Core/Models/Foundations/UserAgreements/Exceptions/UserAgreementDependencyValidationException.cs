// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions
{
    public class UserAgreementDependencyValidationException : Xeption
    {
        public UserAgreementDependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}