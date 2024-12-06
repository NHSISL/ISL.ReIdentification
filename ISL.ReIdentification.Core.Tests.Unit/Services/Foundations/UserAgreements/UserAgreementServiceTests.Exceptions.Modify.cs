using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<UserAgreement> modifyUserAgreementTask =
                this.userAgreementService.ModifyUserAgreementAsync(randomUserAgreement);

            UserAgreementDependencyException actualUserAgreementDependencyException =
                await Assert.ThrowsAsync<UserAgreementDependencyException>(
                    modifyUserAgreementTask.AsTask);

            // then
            actualUserAgreementDependencyException.Should()
                .BeEquivalentTo(expectedUserAgreementDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserAgreementByIdAsync(randomUserAgreement.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedUserAgreementDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAgreementAsync(randomUserAgreement),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnModifyIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            UserAgreement someUserAgreement = CreateRandomUserAgreement();
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(exceptionMessage);

            var invalidUserAgreementReferenceException =
                new InvalidUserAgreementReferenceException(
                    message: "Invalid userAgreement reference error occurred.", 
                    innerException: foreignKeyConstraintConflictException);

            UserAgreementDependencyValidationException expectedUserAgreementDependencyValidationException =
                new UserAgreementDependencyValidationException(
                    message: "UserAgreement dependency validation occurred, please try again.",
                    innerException: invalidUserAgreementReferenceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(foreignKeyConstraintConflictException);

            // when
            ValueTask<UserAgreement> modifyUserAgreementTask =
                this.userAgreementService.ModifyUserAgreementAsync(someUserAgreement);

            UserAgreementDependencyValidationException actualUserAgreementDependencyValidationException =
                await Assert.ThrowsAsync<UserAgreementDependencyValidationException>(
                    modifyUserAgreementTask.AsTask);

            // then
            actualUserAgreementDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserAgreementDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserAgreementByIdAsync(someUserAgreement.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedUserAgreementDependencyValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAgreementAsync(someUserAgreement),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            UserAgreement randomUserAgreement = CreateRandomUserAgreement();
            var databaseUpdateException = new DbUpdateException();

            var failedUserAgreementStorageException =
                new FailedUserAgreementStorageException(
                    message: "Failed userAgreement storage error occurred, contact support.",
                    innerException: databaseUpdateException);

            var expectedUserAgreementDependencyException =
                new UserAgreementDependencyException(
                    message: "UserAgreement dependency error occurred, contact support.",
                    innerException: failedUserAgreementStorageException); 

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<UserAgreement> modifyUserAgreementTask =
                this.userAgreementService.ModifyUserAgreementAsync(randomUserAgreement);

            UserAgreementDependencyException actualUserAgreementDependencyException =
                await Assert.ThrowsAsync<UserAgreementDependencyException>(
                    modifyUserAgreementTask.AsTask);

            // then
            actualUserAgreementDependencyException.Should()
                .BeEquivalentTo(expectedUserAgreementDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserAgreementByIdAsync(randomUserAgreement.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserAgreementDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAgreementAsync(randomUserAgreement),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDbUpdateConcurrencyErrorOccursAndLogAsync()
        {
            // given
            UserAgreement randomUserAgreement = CreateRandomUserAgreement();
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedUserAgreementException =
                new LockedUserAgreementException(
                    message: "Locked userAgreement record exception, please try again later",
                    innerException: databaseUpdateConcurrencyException);

            var expectedUserAgreementDependencyValidationException =
                new UserAgreementDependencyValidationException(
                    message: "UserAgreement dependency validation occurred, please try again.",
                    innerException: lockedUserAgreementException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateConcurrencyException);

            // when
            ValueTask<UserAgreement> modifyUserAgreementTask =
                this.userAgreementService.ModifyUserAgreementAsync(randomUserAgreement);

            UserAgreementDependencyValidationException actualUserAgreementDependencyValidationException =
                await Assert.ThrowsAsync<UserAgreementDependencyValidationException>(
                    modifyUserAgreementTask.AsTask);

            // then
            actualUserAgreementDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserAgreementDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserAgreementByIdAsync(randomUserAgreement.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserAgreementDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAgreementAsync(randomUserAgreement),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfServiceErrorOccursAndLogItAsync()
        {
            // given
            UserAgreement randomUserAgreement = CreateRandomUserAgreement();
            var serviceException = new Exception();

            var failedUserAgreementServiceException =
                new FailedUserAgreementServiceException(
                    message: "Failed userAgreement service occurred, please contact support", 
                    innerException: serviceException);

            var expectedUserAgreementServiceException =
                new UserAgreementServiceException(
                    message: "UserAgreement service error occurred, contact support.",
                    innerException: failedUserAgreementServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // when
            ValueTask<UserAgreement> modifyUserAgreementTask =
                this.userAgreementService.ModifyUserAgreementAsync(randomUserAgreement);

            UserAgreementServiceException actualUserAgreementServiceException =
                await Assert.ThrowsAsync<UserAgreementServiceException>(
                    modifyUserAgreementTask.AsTask);

            // then
            actualUserAgreementServiceException.Should()
                .BeEquivalentTo(expectedUserAgreementServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserAgreementByIdAsync(randomUserAgreement.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserAgreementServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAgreementAsync(randomUserAgreement),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}