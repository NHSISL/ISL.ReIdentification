using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions;
using Xunit;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAgreements
{
    public partial class UserAgreementServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveIfSqlErrorOccursAndLogItAsync()
        {
            // given
            UserAgreement randomUserAgreement = CreateRandomUserAgreement();
            SqlException sqlException = GetSqlException();

            var failedUserAgreementStorageException =
                new FailedUserAgreementStorageException(
                    message: "Failed userAgreement storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedUserAgreementDependencyException =
                new UserAgreementDependencyException(
                    message: "UserAgreement dependency error occurred, contact support.",
                    innerException: failedUserAgreementStorageException); 

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserAgreementByIdAsync(randomUserAgreement.Id))
                    .Throws(sqlException);

            // when
            ValueTask<UserAgreement> addUserAgreementTask =
                this.userAgreementService.RemoveUserAgreementByIdAsync(randomUserAgreement.Id);

            UserAgreementDependencyException actualUserAgreementDependencyException =
                await Assert.ThrowsAsync<UserAgreementDependencyException>(
                    addUserAgreementTask.AsTask);

            // then
            actualUserAgreementDependencyException.Should()
                .BeEquivalentTo(expectedUserAgreementDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserAgreementByIdAsync(randomUserAgreement.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedUserAgreementDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteUserAgreementAsync(It.IsAny<UserAgreement>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someUserAgreementId = Guid.NewGuid();

            var databaseUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedUserAgreementException =
                new LockedUserAgreementException(
                    message: "Locked userAgreement record exception, please try again later",
                    innerException: databaseUpdateConcurrencyException);

            var expectedUserAgreementDependencyValidationException =
                new UserAgreementDependencyValidationException(
                    message: "UserAgreement dependency validation occurred, please try again.",
                    innerException: lockedUserAgreementException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserAgreementByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<UserAgreement> removeUserAgreementByIdTask =
                this.userAgreementService.RemoveUserAgreementByIdAsync(someUserAgreementId);

            UserAgreementDependencyValidationException actualUserAgreementDependencyValidationException =
                await Assert.ThrowsAsync<UserAgreementDependencyValidationException>(
                    removeUserAgreementByIdTask.AsTask);

            // then
            actualUserAgreementDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserAgreementDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserAgreementByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserAgreementDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteUserAgreementAsync(It.IsAny<UserAgreement>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            Guid someUserAgreementId = Guid.NewGuid();
            SqlException sqlException = GetSqlException();

            var failedUserAgreementStorageException =
                new FailedUserAgreementStorageException(
                    message: "Failed userAgreement storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedUserAgreementDependencyException =
                new UserAgreementDependencyException(
                    message: "UserAgreement dependency error occurred, contact support.",
                    innerException: failedUserAgreementStorageException); 

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserAgreementByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<UserAgreement> deleteUserAgreementTask =
                this.userAgreementService.RemoveUserAgreementByIdAsync(someUserAgreementId);

            UserAgreementDependencyException actualUserAgreementDependencyException =
                await Assert.ThrowsAsync<UserAgreementDependencyException>(
                    deleteUserAgreementTask.AsTask);

            // then
            actualUserAgreementDependencyException.Should()
                .BeEquivalentTo(expectedUserAgreementDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserAgreementByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedUserAgreementDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someUserAgreementId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedUserAgreementServiceException =
                new FailedUserAgreementServiceException(
                    message: "Failed userAgreement service occurred, please contact support", 
                    innerException: serviceException);

            var expectedUserAgreementServiceException =
                new UserAgreementServiceException(
                    message: "UserAgreement service error occurred, contact support.",
                    innerException: failedUserAgreementServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserAgreementByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<UserAgreement> removeUserAgreementByIdTask =
                this.userAgreementService.RemoveUserAgreementByIdAsync(someUserAgreementId);

            UserAgreementServiceException actualUserAgreementServiceException =
                await Assert.ThrowsAsync<UserAgreementServiceException>(
                    removeUserAgreementByIdTask.AsTask);

            // then
            actualUserAgreementServiceException.Should()
                .BeEquivalentTo(expectedUserAgreementServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserAgreementByIdAsync(It.IsAny<Guid>()),
                        Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserAgreementServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}