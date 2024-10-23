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
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfErrorOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            UserAccess someUserAccess = CreateRandomUserAccess();
            UserAccess inputUserAccess = someUserAccess;

            var expectedUserAccessProcessingDependencyValidationException =
                new UserAccessProcessingDependencyValidationException(
                    message: "User access processing dependency validation error occurred, please try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.userAccessServiceMock.Setup(service =>
                service.AddUserAccessAsync(inputUserAccess))
                    .Throws(dependencyValidationException);

            // when
            ValueTask<UserAccess> userAccessAddTask =
                this.userAccessProcessingService.AddUserAccessAsync(inputUserAccess);

            UserAccessProcessingDependencyValidationException actualException =
                await Assert.ThrowsAsync<UserAccessProcessingDependencyValidationException>(
                    userAccessAddTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedUserAccessProcessingDependencyValidationException);

            this.userAccessServiceMock.Verify(service =>
                service.AddUserAccessAsync(inputUserAccess),
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
        public async Task ShouldThrowDependencyExceptionOnAddIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            UserAccess someUserAccess = CreateRandomUserAccess();
            UserAccess inputUserAccess = someUserAccess;

            var expectedUserAccessProcessingDependencyException =
                new UserAccessProcessingDependencyException(
                    message: "User access processing dependency error occurred, please try again.",
                    innerException: dependencyException.InnerException as Xeption);

            this.userAccessServiceMock.Setup(service =>
                service.AddUserAccessAsync(inputUserAccess))
                    .Throws(dependencyException);

            // when
            ValueTask<UserAccess> userAccessAddTask =
                this.userAccessProcessingService.AddUserAccessAsync(inputUserAccess);

            UserAccessProcessingDependencyException actualException =
                await Assert.ThrowsAsync<UserAccessProcessingDependencyException>(userAccessAddTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedUserAccessProcessingDependencyException);

            this.userAccessServiceMock.Verify(service =>
                service.AddUserAccessAsync(inputUserAccess),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                 broker.LogErrorAsync(It.Is(SameExceptionAs(
                     expectedUserAccessProcessingDependencyException))),
                         Times.Once);

            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAsync()
        {
            // given
            UserAccess someUserAccess = CreateRandomUserAccess();
            UserAccess inputUserAccess = someUserAccess;

            var serviceException = new Exception();

            var failedUserAccessProcessingServiceException =
                new FailedUserAccessProcessingServiceException(
                    message: "Failed user access processing service error occurred, please contact support.",
                    innerException: serviceException);

            var expectedUserAccessProcessingServiveException =
                new UserAccessProcessingServiceException(
                    message: "User access processing service error occurred, please contact support.",
                    innerException: failedUserAccessProcessingServiceException);

            this.userAccessServiceMock.Setup(service =>
                service.AddUserAccessAsync(inputUserAccess))
                    .Throws(serviceException);

            // when
            ValueTask<UserAccess> addUserAccessTask =
                this.userAccessProcessingService.AddUserAccessAsync(inputUserAccess);

            UserAccessProcessingServiceException actualException =
                await Assert.ThrowsAsync<UserAccessProcessingServiceException>(addUserAccessTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedUserAccessProcessingServiveException);

            this.userAccessServiceMock.Verify(service =>
                service.AddUserAccessAsync(inputUserAccess),
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
