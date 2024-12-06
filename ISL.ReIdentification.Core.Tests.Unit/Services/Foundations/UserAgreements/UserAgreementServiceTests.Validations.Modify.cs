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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfUserAgreementIsInvalidAndLogItAsync(string invalidText)
        {
            // given 
            var invalidUserAgreement = new UserAgreement
            {
                // TODO:  Add default values for your properties i.e. Name = invalidText
            };

            var invalidUserAgreementException = 
                new InvalidUserAgreementException(
                        message: "Invalid userAgreement. Please correct the errors and try again.");

            invalidUserAgreementException.AddData(
                key: nameof(UserAgreement.Id),
                values: "Id is required");

            //invalidUserAgreementException.AddData(
            //    key: nameof(UserAgreement.Name),
            //    values: "Text is required");

            // TODO: Add or remove data here to suit the validation needs for the UserAgreement model

            invalidUserAgreementException.AddData(
                key: nameof(UserAgreement.CreatedDate),
                values: "Date is required");

            invalidUserAgreementException.AddData(
                key: nameof(UserAgreement.CreatedBy),
                values: "Text is required");

            invalidUserAgreementException.AddData(
                key: nameof(UserAgreement.UpdatedDate),
                values: "Date is required");

            invalidUserAgreementException.AddData(
                key: nameof(UserAgreement.UpdatedBy),
                values: "Text is required");

            var expectedUserAgreementValidationException =
                new UserAgreementValidationException(
                    message: "UserAgreement validation errors occurred, please try again.",
                    innerException: invalidUserAgreementException);

            // when
            ValueTask<UserAgreement> modifyUserAgreementTask =
                this.userAgreementService.ModifyUserAgreementAsync(invalidUserAgreement);

            UserAgreementValidationException actualUserAgreementValidationException =
                await Assert.ThrowsAsync<UserAgreementValidationException>(
                    modifyUserAgreementTask.AsTask);

            //then
            actualUserAgreementValidationException.Should().BeEquivalentTo(expectedUserAgreementValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserAgreementValidationException))),
                        Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAgreementAsync(It.IsAny<UserAgreement>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}