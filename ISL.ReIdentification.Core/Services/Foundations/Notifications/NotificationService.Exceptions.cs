// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.Providers.Notifications.Abstractions.Models.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.Notifications.Exceptions;
using Xeptions;

namespace ISL.ReIdentification.Core.Services.Foundations.Notifications
{
    public partial class NotificationService
    {
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (InvalidArgumentsNotificationException invalidArgumentsNotificationException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidArgumentsNotificationException);
            }
            catch (NotificationProviderValidationException notificationProviderValidationException)
            {
                ClientNotificationException clientNotificationException = new ClientNotificationException(
                    message: "Client notification error occurred, contact support.",
                    innerException: notificationProviderValidationException,
                    data: notificationProviderValidationException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(clientNotificationException);
            }
            catch (NotificationProviderDependencyException notificationProviderDependencyException)
            {
                ServerNotificationException serverNotificationException = new ServerNotificationException(
                    message: "Server notification error occurred, contact support.",
                    innerException: notificationProviderDependencyException,
                    data: notificationProviderDependencyException.Data);

                throw await CreateAndLogDependencyExceptionAsync(notificationProviderDependencyException);
            }
            catch (NotificationProviderServiceException notificationProviderServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(notificationProviderServiceException);
            }
            catch (Exception exception)
            {
                var failedServiceNotificationException =
                    new FailedServiceNotificationException(
                        message: "Failed service notification error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedServiceNotificationException);
            }
        }

        private async ValueTask<NotificationValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var notificationValidationException = new NotificationValidationException(
                message: "Notification validation error occurred, please fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(notificationValidationException);

            return notificationValidationException;
        }

        private async ValueTask<NotificationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var notificationDependencyValidationException = new NotificationDependencyValidationException(
                message: "Notification dependency validation error occurred, fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(notificationDependencyValidationException);

            return notificationDependencyValidationException;
        }

        private async ValueTask<NotificationDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var notificationDependencyException = new NotificationDependencyException(
                message: "Notification dependency error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(notificationDependencyException);

            return notificationDependencyException;
        }

        private async ValueTask<NotificationServiceException> CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var notificationServiceException = new NotificationServiceException(
                message: "Notification service error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(notificationServiceException);

            return notificationServiceException;
        }
    }
}
