// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace ISL.ReIdentification.Core.Services.Foundations.CsvIdentificationRequests
{
    public partial class CsvIdentificationRequestService
    {
        private delegate ValueTask<CsvIdentificationRequest> ReturningCsvIdentificationRequestFunction();
        private delegate ValueTask<IQueryable<CsvIdentificationRequest>> ReturningCsvIdentificationRequestsFunction();

        private async ValueTask<CsvIdentificationRequest> TryCatch(
            ReturningCsvIdentificationRequestFunction returningCsvIdentificationRequestFunction)
        {
            try
            {
                return await returningCsvIdentificationRequestFunction();
            }
            catch (NullCsvIdentificationRequestException nullCsvIdentificationRequestException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullCsvIdentificationRequestException);
            }
            catch (InvalidCsvIdentificationRequestException invalidCsvIdentificationRequestException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidCsvIdentificationRequestException);
            }
            catch (NotFoundCsvIdentificationRequestException notFoundCsvIdentificationRequestException)
            {
                throw await CreateAndLogValidationExceptionAsync(notFoundCsvIdentificationRequestException);
            }
            catch (SqlException sqlException)
            {
                var failedStorageCsvIdentificationRequestException = new FailedStorageCsvIdentificationRequestException(
                    message: "Failed csv identification request storage error occurred, contact support.",
                    innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageCsvIdentificationRequestException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsCsvIdentificationRequestException =
                    new AlreadyExistsCsvIdentificationRequestException(
                        message: "CsvIdentificationRequest already exists error occurred.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(alreadyExistsCsvIdentificationRequestException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var concurrencyGemException =
                    new LockedCsvIdentificationRequestException(
                        message: "Locked csv identification request record error occurred, please try again.",
                        innerException: dbUpdateConcurrencyException);

                throw await CreateAndLogDependencyValidationExceptionAsync(concurrencyGemException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedOperationCsvIdentificationRequestException =
                    new FailedOperationCsvIdentificationRequestException(
                        message: "Failed operation csv identification request error occurred, contact support.",
                        innerException: dbUpdateException);

                throw await CreateAndLogDependencyExceptionAsync(failedOperationCsvIdentificationRequestException);
            }
            catch (Exception exception)
            {
                var failedServiceCsvIdentificationRequestException =
                    new FailedServiceCsvIdentificationRequestException(
                        message: "Failed service csv identification request error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceCsvIdentificationRequestException);
            }
        }

        private async ValueTask<IQueryable<CsvIdentificationRequest>> TryCatch(
           ReturningCsvIdentificationRequestsFunction returningCsvIdentificationRequestsFunction)
        {
            try
            {
                return await returningCsvIdentificationRequestsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedStorageCsvIdentificationRequestException = new FailedStorageCsvIdentificationRequestException(
                   message: "Failed csv identification request storage error occurred, contact support.",
                   innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageCsvIdentificationRequestException);
            }
            catch (Exception exception)
            {
                var failedServiceCsvIdentificationRequestException =
                    new FailedServiceCsvIdentificationRequestException(
                       message: "Failed service csv identification request error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceCsvIdentificationRequestException);
            }
        }

        private async ValueTask<CsvIdentificationRequestValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var impersonationContextValidationException = new CsvIdentificationRequestValidationException(
                message: "CsvIdentificationRequest validation error occurred, please fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(impersonationContextValidationException);

            return impersonationContextValidationException;
        }

        private async ValueTask<CsvIdentificationRequestDependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var impersonationContextDependencyException = new CsvIdentificationRequestDependencyException(
                message: "CsvIdentificationRequest dependency error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogCriticalAsync(impersonationContextDependencyException);

            return impersonationContextDependencyException;
        }

        private async ValueTask<CsvIdentificationRequestDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var impersonationContextDependencyValidationException = new CsvIdentificationRequestDependencyValidationException(
                message: "CsvIdentificationRequest dependency validation error occurred, fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(impersonationContextDependencyValidationException);

            return impersonationContextDependencyValidationException;
        }

        private async ValueTask<CsvIdentificationRequestDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var impersonationContextDependencyException = new CsvIdentificationRequestDependencyException(
                message: "CsvIdentificationRequest dependency error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(impersonationContextDependencyException);

            return impersonationContextDependencyException;
        }

        private async ValueTask<CsvIdentificationRequestServiceException> CreateAndLogServiceExceptionAsync(
           Xeption exception)
        {
            var impersonationContextServiceException = new CsvIdentificationRequestServiceException(
                message: "Service error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(impersonationContextServiceException);

            return impersonationContextServiceException;
        }
    }
}
