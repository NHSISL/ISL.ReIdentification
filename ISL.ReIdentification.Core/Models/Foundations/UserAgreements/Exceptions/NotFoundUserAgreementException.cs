// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions
{
    public class NotFoundUserAgreementException : Xeption
    {
        public NotFoundUserAgreementException(Guid userAgreementId)
            : base(message: $"Couldn't find userAgreement with userAgreementId: {userAgreementId}.")
        { }
    }
}