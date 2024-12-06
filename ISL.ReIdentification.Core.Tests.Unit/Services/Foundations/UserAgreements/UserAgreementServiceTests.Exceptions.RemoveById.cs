using System.Threading.Tasks;
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
    }
}