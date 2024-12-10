// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace ISL.ReIdentification.Core.Services.Foundations.UserAgreements
{
    public partial class UserAgreementService
    {
        private delegate ValueTask<UserAgreement> ReturningUserAgreementFunction();
        private delegate ValueTask<IQueryable<UserAgreement>> ReturningUserAgreementsFunction();

        private async ValueTask<UserAgreement> TryCatch(ReturningUserAgreementFunction returningUserAgreementFunction)
        {
            try
            {
                return await returningUserAgreementFunction();
            }
            catch (NullUserAgreementException nullUserAgreementException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullUserAgreementException);
            }
            catch (InvalidUserAgreementException invalidUserAgreementException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidUserAgreementException);
            }
            catch (SqlException sqlException)
            {
                var failedUserAgreementStorageException =
                    new FailedUserAgreementStorageException(
                        message: "Failed userAgreement storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedUserAgreementStorageException);
            }
            catch (NotFoundUserAgreementException notFoundUserAgreementException)
            {
                throw await CreateAndLogValidationExceptionAsync(notFoundUserAgreementException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsUserAgreementException =
                    new AlreadyExistsUserAgreementException(
                        message: "UserAgreement with the same Id already exists.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(alreadyExistsUserAgreementException);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidUserAgreementReferenceException =
                    new InvalidUserAgreementReferenceException(
                        message: "Invalid userAgreement reference error occurred.",
                        innerException: foreignKeyConstraintConflictException);

                throw await CreateAndLogDependencyValidationExceptionAsync(invalidUserAgreementReferenceException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedUserAgreementException =
                    new LockedUserAgreementException(
                        message: "Locked userAgreement record exception, please try again later",
                        innerException: dbUpdateConcurrencyException);

                throw await CreateAndLogDependencyValidationExceptionAsync(lockedUserAgreementException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedUserAgreementStorageException =
                    new FailedUserAgreementStorageException(
                        message: "Failed userAgreement storage error occurred, contact support.",
                        innerException: databaseUpdateException);

                throw await CreateAndLogDependencyExceptionAsync(failedUserAgreementStorageException);
            }
            catch (Exception exception)
            {
                var failedUserAgreementServiceException =
                    new FailedUserAgreementServiceException(
                        message: "Failed userAgreement service occurred, please contact support",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedUserAgreementServiceException);
            }
        }

        private async ValueTask<IQueryable<UserAgreement>> TryCatch(
            ReturningUserAgreementsFunction returningUserAgreementsFunction)
        {
            try
            {
                return await returningUserAgreementsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedUserAgreementStorageException =
                    new FailedUserAgreementStorageException(
                        message: "Failed userAgreement storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedUserAgreementStorageException);
            }
            catch (Exception exception)
            {
                var failedUserAgreementServiceException =
                    new FailedUserAgreementServiceException(
                        message: "Failed userAgreement service occurred, please contact support",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedUserAgreementServiceException);
            }
        }

        private async ValueTask<UserAgreementValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var userAgreementValidationException =
                new UserAgreementValidationException(
                    message: "UserAgreement validation errors occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(userAgreementValidationException);

            return userAgreementValidationException;
        }

        private async ValueTask<UserAgreementDependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var userAgreementDependencyException =
                new UserAgreementDependencyException(
                    message: "UserAgreement dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(userAgreementDependencyException);

            return userAgreementDependencyException;
        }

        private async ValueTask<UserAgreementDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var userAgreementDependencyValidationException =
                new UserAgreementDependencyValidationException(
                    message: "UserAgreement dependency validation occurred, please try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(userAgreementDependencyValidationException);

            return userAgreementDependencyValidationException;
        }

        private async ValueTask<UserAgreementDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var userAgreementDependencyException =
                new UserAgreementDependencyException(
                    message: "UserAgreement dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(userAgreementDependencyException);

            return userAgreementDependencyException;
        }

        private async ValueTask<UserAgreementServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var userAgreementServiceException =
                new UserAgreementServiceException(
                    message: "UserAgreement service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(userAgreementServiceException);

            return userAgreementServiceException;
        }
    }
}