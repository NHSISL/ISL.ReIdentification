using System;
using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions
{
    public class AlreadyExistsUserAgreementException : Xeption
    {
        public AlreadyExistsUserAgreementException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}