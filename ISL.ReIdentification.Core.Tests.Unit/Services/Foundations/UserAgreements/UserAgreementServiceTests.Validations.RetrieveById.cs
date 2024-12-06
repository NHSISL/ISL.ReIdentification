using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions;
using Xunit;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAgreements
{
    public partial class UserAgreementServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            var invalidUserAgreementId = Guid.Empty;

            var invalidUserAgreementException = 
                new InvalidUserAgreementException(
                    message: "Invalid userAgreement. Please correct the errors and try again.");

            invalidUserAgreementException.AddData(
                key: nameof(UserAgreement.Id),
                values: "Id is required");

            var expectedUserAgreementValidationException =
                new UserAgreementValidationException(
                    message: "UserAgreement validation errors occurred, please try again.",
                    innerException: invalidUserAgreementException);

            // when
            ValueTask<UserAgreement> retrieveUserAgreementByIdTask =
                this.userAgreementService.RetrieveUserAgreementByIdAsync(invalidUserAgreementId);

            UserAgreementValidationException actualUserAgreementValidationException =
                await Assert.ThrowsAsync<UserAgreementValidationException>(
                    retrieveUserAgreementByIdTask.AsTask);

            // then
            actualUserAgreementValidationException.Should()
                .BeEquivalentTo(expectedUserAgreementValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserAgreementValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserAgreementByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfUserAgreementIsNotFoundAndLogItAsync()
        {
            //given
            Guid someUserAgreementId = Guid.NewGuid();
            UserAgreement noUserAgreement = null;

            var notFoundUserAgreementException =
                new NotFoundUserAgreementException(someUserAgreementId);

            var expectedUserAgreementValidationException =
                new UserAgreementValidationException(
                    message: "UserAgreement validation errors occurred, please try again.",
                    innerException: notFoundUserAgreementException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserAgreementByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noUserAgreement);

            //when
            ValueTask<UserAgreement> retrieveUserAgreementByIdTask =
                this.userAgreementService.RetrieveUserAgreementByIdAsync(someUserAgreementId);

            UserAgreementValidationException actualUserAgreementValidationException =
                await Assert.ThrowsAsync<UserAgreementValidationException>(
                    retrieveUserAgreementByIdTask.AsTask);

            //then
            actualUserAgreementValidationException.Should().BeEquivalentTo(expectedUserAgreementValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserAgreementByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserAgreementValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}