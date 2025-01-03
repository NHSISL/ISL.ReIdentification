// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Brokers.Notifications;
using ISL.ReIdentification.Core.Models.Foundations.Notifications.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Foundations.Notifications;
using Moq;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Notifications
{
    public partial class NotificationTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task
            ShouldThrowDependencyValidationExceptionOnSendImpersonationTokensGeneratedNotificationAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            AccessRequest invalidAccessRequest = CreateImpersonationContextAccessRequest();
            NotificationConfigurations invalidNotificationConfigurations = this.notificationConfigurations;

            ClientNotificationException clientNotificationException = new ClientNotificationException(
                message: "Client notification error occurred, contact support.",
                innerException: dependencyValidationException,
                data: dependencyValidationException.Data);

            var expectedNotificationDependencyValidationException = new NotificationDependencyValidationException(
                message: "Notification dependency validation error occurred, fix errors and try again.",
                innerException: clientNotificationException);

            this.notificationBrokerMock.Setup(broker =>
                broker.SendEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, dynamic>>()))
                        .ThrowsAsync(dependencyValidationException);

            NotificationService notificationService = new NotificationService(
                notificationConfigurations: invalidNotificationConfigurations,
                notificationBroker: this.notificationBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);

            // when
            ValueTask sendImpersonationTokensGeneratedNotificationTask =
                notificationService.SendImpersonationTokensGeneratedNotificationAsync(invalidAccessRequest);

            NotificationDependencyValidationException actualNotificationDependencyValidationException =
                await Assert.ThrowsAsync<NotificationDependencyValidationException>(
                    testCode: sendImpersonationTokensGeneratedNotificationTask.AsTask);

            // then
            actualNotificationDependencyValidationException.Should()
                .BeEquivalentTo(expectedNotificationDependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNotificationDependencyValidationException))),
                        Times.Once);

            this.notificationBrokerMock.Verify(broker =>
                broker.SendEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, dynamic>>()),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.notificationBrokerMock.VerifyNoOtherCalls();
        }


    }
}
