using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
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
            catch (SqlException sqlException)
            {
                var failedUserAgreementStorageException =
                    new FailedUserAgreementStorageException(
                        message: "Failed userAgreement storage error occurred, contact support.",
                        innerException: sqlException);

                throw CreateAndLogCriticalDependencyException(failedUserAgreementStorageException);
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

        private UserAgreementDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var userAgreementDependencyException = 
                new UserAgreementDependencyException(
                    message: "UserAgreement dependency error occurred, contact support.",
                    innerException: exception);

            this.loggingBroker.LogCritical(userAgreementDependencyException);

            return userAgreementDependencyException;
        }
    }
}