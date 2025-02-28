using Xeptions;

namespace ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions
{
    public class UserAgreementDependencyException : Xeption
    {
        public UserAgreementDependencyException(string message, Xeption innerException) 
            : base(message, innerException)
        { }
    }
}