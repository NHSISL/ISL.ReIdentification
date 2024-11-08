// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Brokers.Notifications;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.Notifications.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Foundations.Notifications;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Notifications
{
    public partial class NotificationTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnSendImpersonationApprovedNotificationIfArgInvalidAndLogItAsync()
        {
            // given
            AccessRequest invalidAccessRequest = null;
            NotificationConfigurations invalidNotificationConfigurations = null;

            var invalidArgumentsNotificationException =
                new InvalidArgumentsNotificationException(
                    message: "Invalid notification arguments. Please correct the errors and try again.");

            invalidArgumentsNotificationException.AddData(
                key: nameof(AccessRequest),
                values: $"{nameof(AccessRequest)} is invalid");

            invalidArgumentsNotificationException.AddData(
                key: nameof(NotificationConfigurations),
                values: $"{nameof(NotificationConfigurations)} is invalid");

            var expectedNotificationValidationException =
                new NotificationValidationException(
                    message: "Notification validation error occurred, please fix errors and try again.",
                    innerException: invalidArgumentsNotificationException);

            NotificationService notificationService =
                new NotificationService(
                    notificationConfigurations: invalidNotificationConfigurations,
                    notificationBroker: this.notificationBrokerMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);

            // when
            ValueTask sendImpersonationApprovedNotificationTask =
                notificationService.SendImpersonationApprovedNotificationAsync(invalidAccessRequest);

            NotificationValidationException actualNotificationValidationException =
                await Assert.ThrowsAsync<NotificationValidationException>(
                    testCode: sendImpersonationApprovedNotificationTask.AsTask);

            // then
            actualNotificationValidationException.Should()
                .BeEquivalentTo(expectedNotificationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNotificationValidationException))),
                        Times.Once);

            this.notificationBrokerMock.Verify(broker =>
                broker.SendEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, dynamic>>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.notificationBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnSendImpersonationApprovedNotificationIfCsvItemIsInvalidAndLogItAsync()
        {
            // given
            AccessRequest invalidAccessRequest = new AccessRequest();
            NotificationConfigurations invalidNotificationConfigurations = new NotificationConfigurations();

            var invalidArgumentsNotificationException =
                new InvalidArgumentsNotificationException(
                    message: "Invalid notification arguments. Please correct the errors and try again.");

            invalidArgumentsNotificationException.AddData(
                key: $"{nameof(AccessRequest)}." +
                    $"{nameof(AccessRequest.ImpersonationContext)}",
                values: $"{nameof(ImpersonationContext)} is invalid");

            invalidArgumentsNotificationException.AddData(
                key: $"{nameof(NotificationConfigurations)}." +
                    $"{nameof(NotificationConfigurations.ImpersonationApprovedRequestTemplateId)}",
                values: "Text is invalid");

            invalidArgumentsNotificationException.AddData(
                key: $"{nameof(NotificationConfigurations)}." +
                    $"{nameof(NotificationConfigurations.ConfigurationBaseUrl)}",
                values: "Text is invalid");

            invalidArgumentsNotificationException.AddData(
                key: $"{nameof(NotificationConfigurations)}." +
                    $"{nameof(NotificationConfigurations.PortalBaseUrl)}",
                values: "Text is invalid");

            var expectedNotificationValidationException =
                new NotificationValidationException(
                    message: "Notification validation error occurred, please fix errors and try again.",
                    innerException: invalidArgumentsNotificationException);

            NotificationService notificationService =
                new NotificationService(
                    notificationConfigurations: invalidNotificationConfigurations,
                    notificationBroker: this.notificationBrokerMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);

            // when
            ValueTask sendImpersonationApprovedNotificationTask =
                notificationService.SendImpersonationApprovedNotificationAsync(invalidAccessRequest);

            NotificationValidationException actualNotificationValidationException =
                await Assert.ThrowsAsync<NotificationValidationException>(
                    testCode: sendImpersonationApprovedNotificationTask.AsTask);

            // then
            actualNotificationValidationException.Should()
                .BeEquivalentTo(expectedNotificationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNotificationValidationException))),
                        Times.Once);

            this.notificationBrokerMock.Verify(broker =>
                broker.SendEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, dynamic>>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.notificationBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnSendImpersonationApprovedNotificationIfInputsIsInvalidAndLogItAsync()
        {
            // given
            AccessRequest invalidAccessRequest = new AccessRequest
            {
                ImpersonationContext = new ImpersonationContext()
            };

            NotificationConfigurations invalidNotificationConfigurations = this.notificationConfigurations;

            var invalidArgumentsNotificationException =
                new InvalidArgumentsNotificationException(
                    message: "Invalid notification arguments. Please correct the errors and try again.");

            invalidArgumentsNotificationException.AddData(
                key: $"toEmail",
                values: $"Text is invalid");

            var expectedNotificationValidationException =
                new NotificationValidationException(
                    message: "Notification validation error occurred, please fix errors and try again.",
                    innerException: invalidArgumentsNotificationException);

            NotificationService notificationService =
                new NotificationService(
                    notificationConfigurations: invalidNotificationConfigurations,
                    notificationBroker: this.notificationBrokerMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);

            // when
            ValueTask sendImpersonationApprovedNotificationTask =
                notificationService.SendImpersonationApprovedNotificationAsync(invalidAccessRequest);

            NotificationValidationException actualNotificationValidationException =
                await Assert.ThrowsAsync<NotificationValidationException>(
                    testCode: sendImpersonationApprovedNotificationTask.AsTask);

            // then
            actualNotificationValidationException.Should()
                .BeEquivalentTo(expectedNotificationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNotificationValidationException))),
                        Times.Once);

            this.notificationBrokerMock.Verify(broker =>
                broker.SendEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, dynamic>>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.notificationBrokerMock.VerifyNoOtherCalls();
        }
    }
}
