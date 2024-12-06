using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions;
using Xeptions;

namespace ISL.ReIdentification.Core.Services.Foundations.UserAgreements
{
    public partial class UserAgreementService
    {
        private delegate ValueTask<UserAgreement> ReturningUserAgreementFunction();

        private async ValueTask<UserAgreement> TryCatch(ReturningUserAgreementFunction returningUserAgreementFunction)
        {
            try
            {
                return await returningUserAgreementFunction();
            }
            catch (NullUserAgreementException nullUserAgreementException)
            {
                throw CreateAndLogValidationException(nullUserAgreementException);
            }
            catch (InvalidUserAgreementException invalidUserAgreementException)
            {
                throw CreateAndLogValidationException(invalidUserAgreementException);
            }
        }

        private UserAgreementValidationException CreateAndLogValidationException(Xeption exception)
        {
            var userAgreementValidationException =
                new UserAgreementValidationException(
                    message: "UserAgreement validation errors occurred, please try again.",
                    innerException: exception);

            this.loggingBroker.LogError(userAgreementValidationException);

            return userAgreementValidationException;
        }
    }
}