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
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfErrorOccursAndLogItAsync(
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
                service.ModifyUserAccessAsync(inputUserAccess))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<UserAccess> userAccessAddTask =
                this.userAccessProcessingService.ModifyUserAccessAsync(inputUserAccess);

            UserAccessProcessingDependencyValidationException actualException =
                await Assert.ThrowsAsync<UserAccessProcessingDependencyValidationException>(
                    userAccessAddTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedUserAccessProcessingDependencyValidationException);

            this.userAccessServiceMock.Verify(service =>
                service.ModifyUserAccessAsync(inputUserAccess),
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
        public async Task ShouldThrowDependencyExceptionOnModifyIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            UserAccess someUserAccess = CreateRandomUserAccess();
            UserAccess inputUserAccess = someUserAccess;

            var expectedUserAccessProcessingDependencyException = new UserAccessProcessingDependencyException(
                message: "User access processing dependency error occurred, please try again.",
                innerException: dependencyException.InnerException as Xeption);

            this.userAccessServiceMock.Setup(service =>
                service.ModifyUserAccessAsync(inputUserAccess))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<UserAccess> userAccessAddTask =
                this.userAccessProcessingService.ModifyUserAccessAsync(inputUserAccess);

            UserAccessProcessingDependencyException actualException =
                await Assert.ThrowsAsync<UserAccessProcessingDependencyException>(userAccessAddTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedUserAccessProcessingDependencyException);

            this.userAccessServiceMock.Verify(service =>
                service.ModifyUserAccessAsync(inputUserAccess),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                 broker.LogErrorAsync(It.Is(SameExceptionAs(
                     expectedUserAccessProcessingDependencyException))),
                         Times.Once);

            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfServiceErrorOccursAsync()
        {
            // given
            UserAccess someUserAccess = CreateRandomUserAccess();
            UserAccess inputUserAccess = someUserAccess;

            var serviceException = new Exception();

            var failedUserAccessProcessingServiceException = new FailedUserAccessProcessingServiceException(
                message: "Failed user access processing service error occurred, please contact support.",
                innerException: serviceException);

            var expectedUserAccessProcessingServiveException = new UserAccessProcessingServiceException(
                message: "User access processing service error occurred, please contact support.",
                innerException: failedUserAccessProcessingServiceException);

            this.userAccessServiceMock.Setup(service =>
                service.ModifyUserAccessAsync(inputUserAccess))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<UserAccess> addUserAccessTask =
                this.userAccessProcessingService.ModifyUserAccessAsync(inputUserAccess);

            UserAccessProcessingServiceException actualException =
                await Assert.ThrowsAsync<UserAccessProcessingServiceException>(addUserAccessTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedUserAccessProcessingServiveException);

            this.userAccessServiceMock.Verify(service =>
                service.ModifyUserAccessAsync(inputUserAccess),
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
