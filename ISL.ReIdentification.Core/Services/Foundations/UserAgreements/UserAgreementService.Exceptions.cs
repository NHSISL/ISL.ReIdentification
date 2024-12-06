using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsUserAgreementException =
                    new AlreadyExistsUserAgreementException(
                        message: "UserAgreement with the same Id already exists.",
                        innerException: duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsUserAgreementException);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidUserAgreementReferenceException =
                    new InvalidUserAgreementReferenceException(
                        message: "Invalid userAgreement reference error occurred.", 
                        innerException: foreignKeyConstraintConflictException);

                throw CreateAndLogDependencyValidationException(invalidUserAgreementReferenceException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedUserAgreementStorageException =
                    new FailedUserAgreementStorageException(
                    message: "Failed userAgreement storage error occurred, contact support.",
                    innerException: databaseUpdateException);

                throw CreateAndLogDependencyException(failedUserAgreementStorageException);
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

        private UserAgreementDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var userAgreementDependencyValidationException =
                new UserAgreementDependencyValidationException(
                    message: "UserAgreement dependency validation occurred, please try again.",
                    innerException: exception);

            this.loggingBroker.LogError(userAgreementDependencyValidationException);

            return userAgreementDependencyValidationException;
        }

        private UserAgreementDependencyException CreateAndLogDependencyException(
            Xeption exception)
        {
            var userAgreementDependencyException = 
                new UserAgreementDependencyException(
                    message: "UserAgreement dependency error occurred, contact support.",
                    innerException: exception); 

            this.loggingBroker.LogError(userAgreementDependencyException);

            return userAgreementDependencyException;
        }
    }
}