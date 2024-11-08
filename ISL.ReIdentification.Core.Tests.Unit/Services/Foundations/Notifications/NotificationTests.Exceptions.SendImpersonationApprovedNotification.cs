// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldThrowDependencyValidationExceptionOnSendImpersonationApprovedNotificationAndLogItAsync(
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
            ValueTask sendImpersonationApprovedNotificationTask =
                notificationService.SendImpersonationApprovedNotificationAsync(invalidAccessRequest);

            NotificationDependencyValidationException actualNotificationDependencyValidationException =
                await Assert.ThrowsAsync<NotificationDependencyValidationException>(
                    testCode: sendImpersonationApprovedNotificationTask.AsTask);

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

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnSendImpersonationApprovedNotificationAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            AccessRequest invalidAccessRequest = CreateImpersonationContextAccessRequest();
            NotificationConfigurations invalidNotificationConfigurations = this.notificationConfigurations;

            ServerNotificationException serverNotificationException = new ServerNotificationException(
                message: "Server notification error occurred, contact support.",
                innerException: dependencyException,
                data: dependencyException.Data);

            var expectedNotificationDependencyException = new NotificationDependencyException(
                message: "Notification dependency error occurred, contact support.",
                innerException: serverNotificationException);

            this.notificationBrokerMock.Setup(broker =>
                broker.SendEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, dynamic>>()))
                        .ThrowsAsync(dependencyException);

            NotificationService notificationService =
                new NotificationService(
                    notificationConfigurations: invalidNotificationConfigurations,
                    notificationBroker: this.notificationBrokerMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);

            // when
            ValueTask sendImpersonationApprovedNotificationTask =
                notificationService.SendImpersonationApprovedNotificationAsync(invalidAccessRequest);

            NotificationDependencyException actualNotificationDependencyException =
                await Assert.ThrowsAsync<NotificationDependencyException>(
                    testCode: sendImpersonationApprovedNotificationTask.AsTask);

            // then
            actualNotificationDependencyException.Should()
                .BeEquivalentTo(expectedNotificationDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNotificationDependencyException))),
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

        [Fact]
        public async Task ShouldThrowServiceExceptionOnSendImpersonationApprovedNotificationAndLogItAsync()
        {
            // given
            AccessRequest invalidAccessRequest = CreateImpersonationContextAccessRequest();
            NotificationConfigurations invalidNotificationConfigurations = this.notificationConfigurations;
            Exception someException = new Exception();

            var failedServiceNotificationException =
                new FailedServiceNotificationException(
                    message: "Failed service notification error occurred, contact support.",
                    innerException: someException,
                    data: someException.Data);

            var expectedNotificationServiceException = new NotificationServiceException(
                message: "Notification service error occurred, contact support.",
                innerException: failedServiceNotificationException);

            this.notificationBrokerMock.Setup(broker =>
                broker.SendEmailAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, dynamic>>()))
                        .ThrowsAsync(someException);

            NotificationService notificationService =
                new NotificationService(
                    notificationConfigurations: invalidNotificationConfigurations,
                    notificationBroker: this.notificationBrokerMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);

            // when
            ValueTask sendImpersonationApprovedNotificationTask =
                notificationService.SendImpersonationApprovedNotificationAsync(invalidAccessRequest);

            NotificationServiceException actualNotificationServiceException =
                await Assert.ThrowsAsync<NotificationServiceException>(
                    testCode: sendImpersonationApprovedNotificationTask.AsTask);

            // then
            actualNotificationServiceException.Should()
                .BeEquivalentTo(expectedNotificationServiceException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedNotificationServiceException))),
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
