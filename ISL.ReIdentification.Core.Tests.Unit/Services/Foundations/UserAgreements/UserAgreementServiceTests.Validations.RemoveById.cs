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
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidUserAgreementId = Guid.Empty;

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
            ValueTask<UserAgreement> removeUserAgreementByIdTask =
                this.userAgreementService.RemoveUserAgreementByIdAsync(invalidUserAgreementId);

            UserAgreementValidationException actualUserAgreementValidationException =
                await Assert.ThrowsAsync<UserAgreementValidationException>(
                    removeUserAgreementByIdTask.AsTask);

            // then
            actualUserAgreementValidationException.Should()
                .BeEquivalentTo(expectedUserAgreementValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserAgreementValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteUserAgreementAsync(It.IsAny<UserAgreement>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}