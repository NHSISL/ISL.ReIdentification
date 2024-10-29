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
        public async Task ShouldThrowDependencyValidationExceptionOnBulkAddRemoveIfErrorOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            BulkUserAccess someBulkUserAccess = CreateRandomBulkUserAccess();

            var expectedUserAccessProcessingDependencyValidationException =
                new UserAccessProcessingDependencyValidationException(
                    message: "User access processing dependency validation error occurred, please try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.userAccessServiceMock.Setup(service =>
                service.RetrieveAllUserAccessesAsync())
                    .Throws(dependencyValidationException);

            // when
            ValueTask bulkUserAccessAddTask =
                this.userAccessProcessingService.BulkAddRemoveUserAccessAsync(someBulkUserAccess);

            UserAccessProcessingDependencyValidationException actualException =
                await Assert.ThrowsAsync<UserAccessProcessingDependencyValidationException>(
                    bulkUserAccessAddTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedUserAccessProcessingDependencyValidationException);

            this.userAccessServiceMock.Verify(service =>
                service.RetrieveAllUserAccessesAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                 broker.LogErrorAsync(It.Is(SameExceptionAs(
                     expectedUserAccessProcessingDependencyValidationException))),
                         Times.Once);

            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnBulkAddRemoveIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            BulkUserAccess someBulkUserAccess = CreateRandomBulkUserAccess();

            var expectedUserAccessProcessingDependencyException = new UserAccessProcessingDependencyException(
                message: "User access processing dependency error occurred, please try again.",
                innerException: dependencyException.InnerException as Xeption);

            this.userAccessServiceMock.Setup(service =>
                service.RetrieveAllUserAccessesAsync())
                    .Throws(dependencyException);

            // when
            ValueTask bulkUserAccessAddTask =
                this.userAccessProcessingService.BulkAddRemoveUserAccessAsync(someBulkUserAccess);

            UserAccessProcessingDependencyException actualException =
                await Assert.ThrowsAsync<UserAccessProcessingDependencyException>(bulkUserAccessAddTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedUserAccessProcessingDependencyException);

            this.userAccessServiceMock.Verify(service =>
                service.RetrieveAllUserAccessesAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                 broker.LogErrorAsync(It.Is(SameExceptionAs(
                     expectedUserAccessProcessingDependencyException))),
                         Times.Once);

            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnBulkAddRemoveIfServiceErrorOccursAsync()
        {
            // given
            BulkUserAccess someBulkUserAccess = CreateRandomBulkUserAccess();

            var serviceException = new Exception();

            var failedUserAccessProcessingServiceException = new FailedUserAccessProcessingServiceException(
                message: "Failed user access processing service error occurred, please contact support.",
                innerException: serviceException);

            var expectedUserAccessProcessingServiveException = new UserAccessProcessingServiceException(
                message: "User access processing service error occurred, please contact support.",
                innerException: failedUserAccessProcessingServiceException);

            this.userAccessServiceMock.Setup(service =>
                service.RetrieveAllUserAccessesAsync())
                    .Throws(serviceException);

            // when
            ValueTask bulkUserAccessAddTask =
                this.userAccessProcessingService.BulkAddRemoveUserAccessAsync(someBulkUserAccess);

            UserAccessProcessingServiceException actualException =
                await Assert.ThrowsAsync<UserAccessProcessingServiceException>(bulkUserAccessAddTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedUserAccessProcessingServiveException);

            this.userAccessServiceMock.Verify(service =>
                service.RetrieveAllUserAccessesAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                 broker.LogErrorAsync(It.Is(SameExceptionAs(
                     expectedUserAccessProcessingServiveException))),
                         Times.Once);

            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
