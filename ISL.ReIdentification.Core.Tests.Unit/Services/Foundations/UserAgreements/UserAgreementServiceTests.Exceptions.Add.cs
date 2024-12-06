using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions;
using Xunit;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAgreements
{
    public partial class UserAgreementServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            UserAgreement someUserAgreement = CreateRandomUserAgreement();
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
            ValueTask<UserAgreement> addUserAgreementTask =
                this.userAgreementService.AddUserAgreementAsync(someUserAgreement);

            UserAgreementDependencyException actualUserAgreementDependencyException =
                await Assert.ThrowsAsync<UserAgreementDependencyException>(
                    addUserAgreementTask.AsTask);

            // then
            actualUserAgreementDependencyException.Should()
                .BeEquivalentTo(expectedUserAgreementDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserAgreementAsync(It.IsAny<UserAgreement>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedUserAgreementDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfUserAgreementAlreadyExsitsAndLogItAsync()
        {
            // given
            UserAgreement randomUserAgreement = CreateRandomUserAgreement();
            UserAgreement alreadyExistsUserAgreement = randomUserAgreement;
            string randomMessage = GetRandomString();

            var duplicateKeyException =
                new DuplicateKeyException(randomMessage);

            var alreadyExistsUserAgreementException =
                new AlreadyExistsUserAgreementException(
                    message: "UserAgreement with the same Id already exists.",
                    innerException: duplicateKeyException);

            var expectedUserAgreementDependencyValidationException =
                new UserAgreementDependencyValidationException(
                    message: "UserAgreement dependency validation occurred, please try again.",
                    innerException: alreadyExistsUserAgreementException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(duplicateKeyException);

            // when
            ValueTask<UserAgreement> addUserAgreementTask =
                this.userAgreementService.AddUserAgreementAsync(alreadyExistsUserAgreement);

            // then
            UserAgreementDependencyValidationException actualUserAgreementDependencyValidationException =
                await Assert.ThrowsAsync<UserAgreementDependencyValidationException>(
                    addUserAgreementTask.AsTask);

            actualUserAgreementDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserAgreementDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserAgreementAsync(It.IsAny<UserAgreement>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserAgreementDependencyValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnAddIfReferenceErrorOccursAndLogItAsync()
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

            var expectedUserAgreementValidationException =
                new UserAgreementDependencyValidationException(
                    message: "UserAgreement dependency validation occurred, please try again.",
                    innerException: invalidUserAgreementReferenceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(foreignKeyConstraintConflictException);

            // when
            ValueTask<UserAgreement> addUserAgreementTask =
                this.userAgreementService.AddUserAgreementAsync(someUserAgreement);

            // then
            UserAgreementDependencyValidationException actualUserAgreementDependencyValidationException =
                await Assert.ThrowsAsync<UserAgreementDependencyValidationException>(
                    addUserAgreementTask.AsTask);

            actualUserAgreementDependencyValidationException.Should().BeEquivalentTo(expectedUserAgreementValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserAgreementValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserAgreementAsync(someUserAgreement),
                    Times.Never());

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}