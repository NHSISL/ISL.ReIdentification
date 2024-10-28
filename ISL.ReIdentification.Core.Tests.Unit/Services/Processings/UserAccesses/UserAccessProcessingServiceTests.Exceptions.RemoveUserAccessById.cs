// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Processings.UserAccesses.Exceptions;
using Moq;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Processings.UserAccesses
{
    public partial class UserAccessProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRemoveByIdIfErrorOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            Guid someId = Guid.NewGuid();

            var expectedUserAccessProcessingDependencyValidationException =
                new UserAccessProcessingDependencyValidationException(
                    message: "User access processing dependency validation error occurred, please try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.userAccessServiceMock.Setup(service =>
                service.RemoveUserAccessByIdAsync(It.IsAny<Guid>()))
                    .Throws(dependencyValidationException);

            // when
            ValueTask<UserAccess> userAccessAddTask =
                this.userAccessProcessingService.RemoveUserAccessByIdAsync(someId);

            UserAccessProcessingDependencyValidationException actualException =
                await Assert.ThrowsAsync<UserAccessProcessingDependencyValidationException>(
                    userAccessAddTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedUserAccessProcessingDependencyValidationException);

            this.userAccessServiceMock.Verify(service =>
                service.RemoveUserAccessByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                 broker.LogErrorAsync(It.Is(SameExceptionAs(
                     expectedUserAccessProcessingDependencyValidationException))),
                         Times.Once);

            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            Guid someId = Guid.NewGuid();

            var expectedUserAccessProcessingDependencyException = new UserAccessProcessingDependencyException(
                message: "User access processing dependency error occurred, please try again.",
                innerException: dependencyException.InnerException as Xeption);

            this.userAccessServiceMock.Setup(service =>
                service.RemoveUserAccessByIdAsync(It.IsAny<Guid>()))
                    .Throws(dependencyException);

            // when
            ValueTask<UserAccess> userAccessAddTask =
                this.userAccessProcessingService.RemoveUserAccessByIdAsync(someId);

            UserAccessProcessingDependencyException actualException =
                await Assert.ThrowsAsync<UserAccessProcessingDependencyException>(userAccessAddTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedUserAccessProcessingDependencyException);

            this.userAccessServiceMock.Verify(service =>
                service.RemoveUserAccessByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                 broker.LogErrorAsync(It.Is(SameExceptionAs(
                     expectedUserAccessProcessingDependencyException))),
                         Times.Once);

            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfServiceErrorOccursAsync()
        {
            // given
            Guid someId = Guid.NewGuid();

            var serviceException = new Exception();

            var failedUserAccessProcessingServiceException = new FailedUserAccessProcessingServiceException(
                message: "Failed user access processing service error occurred, please contact support.",
                innerException: serviceException);

            var expectedUserAccessProcessingServiveException = new UserAccessProcessingServiceException(
                message: "User access processing service error occurred, please contact support.",
                innerException: failedUserAccessProcessingServiceException);

            this.userAccessServiceMock.Setup(service =>
                service.RemoveUserAccessByIdAsync(It.IsAny<Guid>()))
                    .Throws(serviceException);

            // when
            ValueTask<UserAccess> addUserAccessTask =
                this.userAccessProcessingService.RemoveUserAccessByIdAsync(someId);

            UserAccessProcessingServiceException actualException =
                await Assert.ThrowsAsync<UserAccessProcessingServiceException>(addUserAccessTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedUserAccessProcessingServiveException);

            this.userAccessServiceMock.Verify(service =>
                service.RemoveUserAccessByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                 broker.LogErrorAsync(It.Is(SameExceptionAs(
                     expectedUserAccessProcessingServiveException))),
                         Times.Once);

            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
