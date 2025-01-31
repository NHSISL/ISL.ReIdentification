// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            UserAccess randomUserAccess = CreateRandomUserAccess();
            SqlException sqlException = CreateSqlException();

            var failedUserAccessStorageException =
                new FailedStorageUserAccessException(
                    message: "Failed user access storage error occurred, contact support.",
                        innerException: sqlException);

            var expectedUserAccessDependencyException =
                new UserAccessDependencyException(
                    message: "UserAccess dependency error occurred, contact support.",
                        innerException: failedUserAccessStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<UserAccess> modifyUserAccessTask =
                this.userAccessService.ModifyUserAccessAsync(randomUserAccess);

            UserAccessDependencyException actualUserAccessDependencyException =
                await Assert.ThrowsAsync<UserAccessDependencyException>(
                    testCode: modifyUserAccessTask.AsTask);

            // then
            actualUserAccessDependencyException.Should().BeEquivalentTo(
                expectedUserAccessDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectUserAccessByIdAsync(randomUserAccess.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedUserAccessDependencyException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateUserAccessAsync(randomUserAccess),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDependencyErrorOccurredAndLogItAsync()
        {
            // given
            UserAccess someUserAccess = CreateRandomUserAccess();
            var dbUpdateException = new DbUpdateException();

            var failedOperationUserAccessException =
                new FailedOperationUserAccessException(
                    message: "Failed operation user access error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedUserAccessDependencyException =
                new UserAccessDependencyException(
                    message: "UserAccess dependency error occurred, contact support.",
                    innerException: failedOperationUserAccessException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<UserAccess> addUserAccessTask =
                this.userAccessService.AddUserAccessAsync(
                    someUserAccess);

            UserAccessDependencyException actualUserAccessDependencyException =
                await Assert.ThrowsAsync<UserAccessDependencyException>(
                    testCode: addUserAccessTask.AsTask);

            // then
            actualUserAccessDependencyException.Should().BeEquivalentTo(
                expectedUserAccessDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserAccessDependencyException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertUserAccessAsync(It.IsAny<UserAccess>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDbUpdateConcurrencyOccursAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            UserAccess randomUserAccess = CreateRandomUserAccess(randomDateTimeOffset);
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedUserAccessException = new LockedUserAccessException(
                message: "Locked user access record error occurred, please try again.",
                innerException: dbUpdateConcurrencyException);

            var expectedUserAccessDependencyValidationException = new UserAccessDependencyValidationException(
                message: "UserAccess dependency validation error occurred, fix errors and try again.",
                innerException: lockedUserAccessException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<UserAccess> modifyUserAccessTask =
                this.userAccessService.ModifyUserAccessAsync(randomUserAccess);

            UserAccessDependencyValidationException actualUserAccessDependencyValidationException =
                await Assert.ThrowsAsync<UserAccessDependencyValidationException>(
                    testCode: modifyUserAccessTask.AsTask);

            // then
            actualUserAccessDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserAccessDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserAccessDependencyValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectUserAccessByIdAsync(randomUserAccess.Id),
                    Times.Never());

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            UserAccess someUserAccess = CreateRandomModifyUserAccess(randomDateTimeOffset);
            var serviceException = new Exception();

            var failedServiceUserAccessException =
                new FailedServiceUserAccessException(
                    message: "Failed service user access error occurred, contact support.",
                    innerException: serviceException);

            var expectedUserAccessServiceException =
                new UserAccessServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceUserAccessException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<UserAccess> addUserAccessTask =
                this.userAccessService.AddUserAccessAsync(someUserAccess);

            UserAccessServiceException actualUserAccessServiceException =
                await Assert.ThrowsAsync<UserAccessServiceException>(
                    testCode: addUserAccessTask.AsTask);

            // then
            actualUserAccessServiceException.Should().BeEquivalentTo(
                expectedUserAccessServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserAccessServiceException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertUserAccessAsync(It.IsAny<UserAccess>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }
    }
}
