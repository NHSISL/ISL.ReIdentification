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