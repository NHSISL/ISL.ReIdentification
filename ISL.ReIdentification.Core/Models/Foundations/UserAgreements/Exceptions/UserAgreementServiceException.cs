using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions
{
    public class UserAgreementServiceException : Xeption
    {
        public UserAgreementServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}