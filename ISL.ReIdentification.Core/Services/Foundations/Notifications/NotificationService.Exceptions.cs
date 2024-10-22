// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions;
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
    }
}
