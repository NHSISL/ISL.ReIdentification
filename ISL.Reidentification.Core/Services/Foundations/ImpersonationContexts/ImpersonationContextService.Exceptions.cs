// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace ISL.ReIdentification.Core.Services.Foundations.ImpersonationContexts
{
    public partial class ImpersonationContextService
    {
        private delegate ValueTask<ImpersonationContext> ReturningImpersonationContextFunction();
        private delegate ValueTask<IQueryable<ImpersonationContext>> ReturningImpersonationContextsFunction();

        private async ValueTask<ImpersonationContext> TryCatch(
            ReturningImpersonationContextFunction returningImpersonationContextFunction)
        {
            try
            {
                return await returningImpersonationContextFunction();
            }
            catch (NullImpersonationContextException nullImpersonationContextException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullImpersonationContextException);
            }
            catch (InvalidImpersonationContextException invalidImpersonationContextException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidImpersonationContextException);
            }
            catch (NotFoundImpersonationContextException notFoundImpersonationContextException)
            {
                throw await CreateAndLogValidationExceptionAsync(notFoundImpersonationContextException);
            }
            catch (SqlException sqlException)
            {
                var failedStorageImpersonationContextException = new FailedStorageImpersonationContextException(
                    message: "Failed impersonation context storage error occurred, contact support.",
                    innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageImpersonationContextException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsImpersonationContextException =
                    new AlreadyExistsImpersonationContextException(
                        message: "Impersonation context already exists error occurred.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(alreadyExistsImpersonationContextException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var concurrencyGemException =
                    new LockedImpersonationContextException(
                        message: "Locked impersonation context record error occurred, please try again.",
                        innerException: dbUpdateConcurrencyException);

                throw await CreateAndLogDependencyValidationExceptionAsync(concurrencyGemException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedOperationImpersonationContextException =
                    new FailedOperationImpersonationContextException(
                        message: "Failed operation impersonation context error occurred, contact support.",
                        innerException: dbUpdateException);

                throw await CreateAndLogDependencyExceptionAsync(failedOperationImpersonationContextException);
            }
            catch (Exception exception)
            {
                var failedServiceImpersonationContextException =
                    new FailedServiceImpersonationContextException(
                        message: "Failed service impersonation context error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceImpersonationContextException);
            }
        }

        private async ValueTask<IQueryable<ImpersonationContext>> TryCatch(
           ReturningImpersonationContextsFunction returningImpersonationContextsFunction)
        {
            try
            {
                return await returningImpersonationContextsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedStorageImpersonationContextException = new FailedStorageImpersonationContextException(
                   message: "Failed impersonation context storage error occurred, contact support.",
                   innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageImpersonationContextException);
            }
            catch (Exception exception)
            {
                var failedServiceImpersonationContextException =
                    new FailedServiceImpersonationContextException(
                       message: "Failed service impersonation context error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceImpersonationContextException);
            }
        }

        private async ValueTask<ImpersonationContextValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var impersonationContextValidationException = new ImpersonationContextValidationException(
                message: "Impersonation context validation error occurred, please fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(impersonationContextValidationException);

            return impersonationContextValidationException;
        }

        private async ValueTask<ImpersonationContextDependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var impersonationContextDependencyException = new ImpersonationContextDependencyException(
                message: "Impersonation context dependency error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogCriticalAsync(impersonationContextDependencyException);

            return impersonationContextDependencyException;
        }

        private async ValueTask<ImpersonationContextDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var impersonationContextDependencyValidationException = new ImpersonationContextDependencyValidationException(
                message: "Impersonation context dependency validation error occurred, fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(impersonationContextDependencyValidationException);

            return impersonationContextDependencyValidationException;
        }

        private async ValueTask<ImpersonationContextDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var impersonationContextDependencyException = new ImpersonationContextDependencyException(
                message: "Impersonation context dependency error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(impersonationContextDependencyException);

            return impersonationContextDependencyException;
        }

        private async ValueTask<ImpersonationContextServiceException> CreateAndLogServiceExceptionAsync(
           Xeption exception)
        {
            var impersonationContextServiceException = new ImpersonationContextServiceException(
                message: "Impersonation context service error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(impersonationContextServiceException);

            return impersonationContextServiceException;
        }
    }
}
