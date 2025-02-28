// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using ISL.ReIdentification.Core.Models.Foundations.Lookups.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace ISL.ReIdentification.Core.Services.Foundations.Lookups
{
    public partial class LookupService
    {
        private delegate ValueTask<Lookup> ReturningLookupFunction();
        private delegate ValueTask<IQueryable<Lookup>> ReturningLookupsFunction();

        private async ValueTask<Lookup> TryCatch(ReturningLookupFunction returningLookupFunction)
        {
            try
            {
                return await returningLookupFunction();
            }
            catch (NullLookupException nullLookupException)
            {
                throw CreateAndLogValidationException(nullLookupException);
            }
            catch (InvalidLookupException invalidLookupException)
            {
                throw CreateAndLogValidationException(invalidLookupException);
            }
            catch (SqlException sqlException)
            {
                var failedStorageLookupException =
                    new FailedStorageLookupException(
                        message: "Failed lookup storage error occurred, contact support.",
                        innerException: sqlException);

                throw CreateAndLogCriticalDependencyException(failedStorageLookupException);
            }
            catch (NotFoundLookupException notFoundLookupException)
            {
                throw CreateAndLogValidationException(notFoundLookupException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsLookupException =
                    new AlreadyExistsLookupException(
                        message: "Lookup with the same Id already exists.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw CreateAndLogDependencyValidationException(alreadyExistsLookupException);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidLookupReferenceException =
                    new InvalidLookupReferenceException(
                        message: "Invalid lookup reference error occurred.",
                        innerException: foreignKeyConstraintConflictException);

                throw CreateAndLogDependencyValidationException(invalidLookupReferenceException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedLookupException =
                    new LockedLookupException(
                        message: "Locked lookup record exception, please try again later",
                        innerException: dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedLookupException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedOperationLookupException =
                    new FailedOperationLookupException(
                        message: "Failed lookup operation error occurred, contact support.",
                        innerException: databaseUpdateException);

                throw CreateAndLogDependencyException(failedOperationLookupException);
            }
            catch (Exception exception)
            {
                var failedLookupServiceException =
                    new FailedLookupServiceException(
                        message: "Failed lookup service occurred, please contact support",
                        innerException: exception);

                throw CreateAndLogServiceException(failedLookupServiceException);
            }
        }

        private async ValueTask<IQueryable<Lookup>> TryCatch(ReturningLookupsFunction returningLookupsFunction)
        {
            try
            {
                return await returningLookupsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedStorageLookupException =
                    new FailedStorageLookupException(
                        message: "Failed lookup storage error occurred, contact support.",
                        innerException: sqlException);

                throw CreateAndLogCriticalDependencyException(failedStorageLookupException);
            }
            catch (Exception exception)
            {
                var failedLookupServiceException =
                    new FailedLookupServiceException(
                        message: "Failed lookup service occurred, please contact support",
                        innerException: exception);

                throw CreateAndLogServiceException(failedLookupServiceException);
            }
        }

        private LookupValidationException CreateAndLogValidationException(Xeption exception)
        {
            var lookupValidationException =
                new LookupValidationException(
                    message: "Lookup validation error occurred, please fix errors and try again.",
                    innerException: exception);

            this.loggingBroker.LogErrorAsync(lookupValidationException);

            return lookupValidationException;
        }

        private LookupDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var lookupDependencyException =
                new LookupDependencyException(
                    message: "Lookup dependency error occurred, contact support.",
                    innerException: exception);

            this.loggingBroker.LogCriticalAsync(lookupDependencyException);

            return lookupDependencyException;
        }

        private LookupDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var lookupDependencyValidationException =
                new LookupDependencyValidationException(
                    message: "Lookup dependency validation occurred, please try again.",
                    innerException: exception);

            this.loggingBroker.LogErrorAsync(lookupDependencyValidationException);

            return lookupDependencyValidationException;
        }

        private LookupDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var lookupDependencyException =
                new LookupDependencyException(
                    message: "Lookup dependency error occurred, contact support.",
                    innerException: exception);

            this.loggingBroker.LogErrorAsync(lookupDependencyException);

            return lookupDependencyException;
        }

        private LookupServiceException CreateAndLogServiceException(Xeption exception)
        {
            var lookupServiceException =
                new LookupServiceException(
                    message: "Lookup service error occurred, contact support.",
                    innerException: exception);

            this.loggingBroker.LogErrorAsync(lookupServiceException);

            return lookupServiceException;
        }
    }
}