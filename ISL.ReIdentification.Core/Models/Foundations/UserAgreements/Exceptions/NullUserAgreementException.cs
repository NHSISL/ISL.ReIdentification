// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions
{
    public class NullUserAgreementException : Xeption
    {
        public NullUserAgreementException(string message)
            : base(message)
        { }
    }
}