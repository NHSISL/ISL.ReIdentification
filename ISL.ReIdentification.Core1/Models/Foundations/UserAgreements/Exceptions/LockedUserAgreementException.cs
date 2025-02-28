using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions
{
    public class LockedUserAgreementException : Xeption
    {
        public LockedUserAgreementException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}