// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.Audits;
using ISL.ReIdentification.Core.Models.Foundations.Audits.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace ISL.ReIdentification.Core.Services.Foundations.Audits
{
    public partial class AuditService
    {
        private delegate ValueTask<Audit> ReturningAuditFunction();
        private delegate ValueTask<IQueryable<Audit>> ReturningAuditsFunction();

        private async ValueTask<IQueryable<Audit>> TryCatch(
            ReturningAuditsFunction returningAuditsFunction)
        {
            try
            {
                return await returningAuditsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedStorageAuditException = new FailedStorageAuditException(
                    message: "Failed audit storage error occurred, contact support.",
                    innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageAuditException);
            }
            catch (Exception exception)
            {
                var failedServiceAuditException =
                    new FailedServiceAuditException(
                        message: "Failed service audit error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceAuditException);
            }
        }
        private async ValueTask<Audit> TryCatch(ReturningAuditFunction returningAuditFunction)
        {
            try
            {
                return await returningAuditFunction();
            }
            catch (NullAuditException nullAuditException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullAuditException);
            }
            catch (InvalidAuditException invalidAuditException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidAuditException);
            }
            catch (NotFoundAuditException notFoundAuditException)
            {
                throw await CreateAndLogValidationExceptionAsync(notFoundAuditException);
            }
            catch (SqlException sqlException)
            {
                var failedStorageAuditException = new FailedStorageAuditException(
                    message: "Failed audit storage error occurred, contact support.",
                    innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageAuditException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsAuditException =
                    new AlreadyExistsAuditException(
                        message: "Audit already exists error occurred.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(alreadyExistsAuditException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedAuditException =
                    new LockedAuditException(
                        message: "Locked audit record error occurred, please try again.",
                        innerException: dbUpdateConcurrencyException);

                throw await CreateAndLogDependencyValidationExceptionAsync(lockedAuditException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedOperationAuditException =
                    new FailedOperationAuditException(
                        message: "Failed operation audit error occurred, contact support.",
                        innerException: dbUpdateException);

                throw await CreateAndLogDependencyExceptionAsync(failedOperationAuditException);
            }
            catch (Exception exception)
            {
                var failedServiceAuditException =
                    new FailedServiceAuditException(
                        message: "Failed service audit error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceAuditException);
            }
        }

        private async ValueTask<AuditValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var accessAuditValidationException = new AuditValidationException(
                message: "Audit validation error occurred, please fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(accessAuditValidationException);

            return accessAuditValidationException;
        }

        private async ValueTask<AuditDependencyException> CreateAndLogCriticalDependencyExceptionAsync(
           Xeption exception)
        {
            var accessAuditDependencyException = new AuditDependencyException(
                message: "Audit dependency error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogCriticalAsync(accessAuditDependencyException);

            return accessAuditDependencyException;
        }

        private async ValueTask<AuditDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var accessAuditDependencyValidationException = new AuditDependencyValidationException(
                message: "Audit dependency validation error occurred, fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(accessAuditDependencyValidationException);

            return accessAuditDependencyValidationException;
        }

        private async ValueTask<AuditDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var accessAuditDependencyException = new AuditDependencyException(
                message: "Audit dependency error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(accessAuditDependencyException);

            return accessAuditDependencyException;
        }

        private async ValueTask<AuditServiceException> CreateAndLogServiceExceptionAsync(
           Xeption exception)
        {
            var accessAuditServiceException = new AuditServiceException(
                message: "Service error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(accessAuditServiceException);

            return accessAuditServiceException;
        }
    }
}
