// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Processings.UserAccesses.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Processings.UserAccesses
{
    public partial class UserAccessProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnBulkAddRemoveUserAccessAsync()
        {
            // given
            BulkUserAccess nullBulkUserAccess = null;

            var nullUserAccessProcessingException =
                new NullUserAccessProcessingException(message: "Bulk user access is null.");

            var expectedUserAccessProcessingValidationException =
                new UserAccessProcessingValidationException(
                    message: "User access processing validation error occurred, please fix errors and try again.",
                    innerException: nullUserAccessProcessingException);

            // when
            ValueTask bulkAddRemoveUserAccessTask = this.userAccessProcessingService
                .BulkAddRemoveUserAccessAsync(nullBulkUserAccess);

            UserAccessProcessingValidationException actualUserAccessProcessingValidationException =
                await Assert.ThrowsAsync<UserAccessProcessingValidationException>(
                    testCode: bulkAddRemoveUserAccessTask.AsTask);

            // then
            actualUserAccessProcessingValidationException.Should()
                .BeEquivalentTo(expectedUserAccessProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedUserAccessProcessingValidationException))),
                    Times.Once());

            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnBulkAddRemoveIfUserAccessIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            var invalidUserAccess = new BulkUserAccess
            {
                EntraUserId = invalidText,
                GivenName = invalidText,
                Surname = invalidText,
                DisplayName = invalidText,
                JobTitle = invalidText,
                Email = invalidText,
                UserPrincipalName = invalidText,
                OrgCodes = null,
            };

            var invalidUserAccessProcessingException =
                new InvalidUserAccessProcessingException(
                    message: "Invalid user access. Please correct the errors and try again.");

            invalidUserAccessProcessingException.AddData(
                key: nameof(BulkUserAccess.EntraUserId),
                values: "Text is invalid");

            invalidUserAccessProcessingException.AddData(
                key: nameof(BulkUserAccess.Email),
                values: "Text is invalid");

            invalidUserAccessProcessingException.AddData(
                key: nameof(BulkUserAccess.OrgCodes),
                values: "List is invalid");

            var expectedUserAccessProcessingValidationException =
                new UserAccessProcessingValidationException(
                    message: "User access processing validation error occurred, please fix errors and try again.",
                    innerException: invalidUserAccessProcessingException);

            // when
            ValueTask bulkAddRemoveUserAccessTask =
                this.userAccessProcessingService.BulkAddRemoveUserAccessAsync(invalidUserAccess);

            UserAccessProcessingValidationException actualUserAccessProcessingValidationException =
                await Assert.ThrowsAsync<UserAccessProcessingValidationException>(
                    testCode: bulkAddRemoveUserAccessTask.AsTask);

            // then
            actualUserAccessProcessingValidationException.Should()
                .BeEquivalentTo(expectedUserAccessProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserAccessProcessingValidationException))),
                        Times.Once);

            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
