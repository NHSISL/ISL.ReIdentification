// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.Reidentification.Core.Models.Foundations.UserAccesses.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAccesses
{
    public partial class UserAccessesTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid randomUserAccessId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedUserAccessStorageException =
                new FailedStorageUserAccessException(
                    message: "Failed user access storage error occurred, contact support.",
                        innerException: sqlException);

            var expectedUserAccessDependencyException =
                new UserAccessDependencyException(
                    message: "UserAccess dependency error occurred, contact support.",
                        innerException: failedUserAccessStorageException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectUserAccessByIdAsync(randomUserAccessId))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<UserAccess> removeByIdUserAccessTask =
                this.userAccessService.RemoveUserAccessByIdAsync(randomUserAccessId);

            UserAccessDependencyException actualUserAccessDependencyException =
                await Assert.ThrowsAsync<UserAccessDependencyException>(
                    testCode: removeByIdUserAccessTask.AsTask);

            // then
            actualUserAccessDependencyException.Should().BeEquivalentTo(
                expectedUserAccessDependencyException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectUserAccessByIdAsync(randomUserAccessId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedUserAccessDependencyException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.DeleteUserAccessAsync(It.IsAny<UserAccess>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowDependencyValidationOnRemoveUserAccessByIdIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid randomUserAccessId = Guid.NewGuid();

            var databaseUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedUserAccessException =
                new LockedUserAccessException(
                    message: "Locked user access record error occurred, please try again.",
                    innerException: databaseUpdateConcurrencyException);

            var expectedUserAccessDependencyValidationException =
                new UserAccessDependencyValidationException(
                    message: "UserAccess dependency validation error occurred, fix errors and try again.",
                    innerException: lockedUserAccessException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectUserAccessByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<UserAccess> removeUserAccessByIdTask =
                this.userAccessService.RemoveUserAccessByIdAsync(randomUserAccessId);

            UserAccessDependencyValidationException actualUserAccessDependencyValidationException =
                await Assert.ThrowsAsync<UserAccessDependencyValidationException>(
                    testCode: removeUserAccessByIdTask.AsTask);

            // then
            actualUserAccessDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserAccessDependencyValidationException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectUserAccessByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserAccessDependencyValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.DeleteUserAccessAsync(It.IsAny<UserAccess>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveUserAccessByIdWhenServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid randomUserAccessId = Guid.NewGuid();
            Exception serviceError = new Exception();

            var failedServiceUserAccessException = new FailedServiceUserAccessException(
                message: "Failed service user access error occurred, contact support.",
                innerException: serviceError);

            var expectedUserAccessServiceException = new UserAccessServiceException(
                message: "Service error occurred, contact support.",
                innerException: failedServiceUserAccessException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectUserAccessByIdAsync(randomUserAccessId))
                    .ThrowsAsync(serviceError);

            // when
            ValueTask<UserAccess> removeByIdUserAccessTask =
                this.userAccessService.RemoveUserAccessByIdAsync(randomUserAccessId);

            UserAccessServiceException actualUserAccessServiceExcpetion =
                await Assert.ThrowsAsync<UserAccessServiceException>(
                    testCode: removeByIdUserAccessTask.AsTask);

            // then
            actualUserAccessServiceExcpetion.Should().BeEquivalentTo(expectedUserAccessServiceException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectUserAccessByIdAsync(randomUserAccessId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserAccessServiceException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.DeleteUserAccessAsync(It.IsAny<UserAccess>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
