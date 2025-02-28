using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions
{
    public class FailedUserAgreementServiceException : Xeption
    {
        public FailedUserAgreementServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}