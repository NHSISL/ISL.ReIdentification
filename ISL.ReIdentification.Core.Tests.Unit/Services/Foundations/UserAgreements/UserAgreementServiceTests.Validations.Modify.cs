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
        public async Task ShouldThrowValidationExceptionOnModifyIfUserAgreementIsNullAndLogItAsync()
        {
            // given
            UserAgreement nullUserAgreement = null;
            var nullUserAgreementException = new NullUserAgreementException(message: "UserAgreement is null.");

            var expectedUserAgreementValidationException =
                new UserAgreementValidationException(
                    message: "UserAgreement validation errors occurred, please try again.",
                    innerException: nullUserAgreementException);

            // when
            ValueTask<UserAgreement> modifyUserAgreementTask =
                this.userAgreementService.ModifyUserAgreementAsync(nullUserAgreement);

            UserAgreementValidationException actualUserAgreementValidationException =
                await Assert.ThrowsAsync<UserAgreementValidationException>(
                    modifyUserAgreementTask.AsTask);

            // then
            actualUserAgreementValidationException.Should()
                .BeEquivalentTo(expectedUserAgreementValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserAgreementValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAgreementAsync(It.IsAny<UserAgreement>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}