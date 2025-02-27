// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Processings.UserAccesses.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Processings.UserAccesses
{
    public partial class UserAccessProcessingServiceTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnRetrieveAllActiveOrganisationsUserHasAccessToAsync(
            string invalidText)
        {
            // given
            string invalidUserAccessId = invalidText;

            var invalidUserAccessProcessingException = new InvalidUserAccessProcessingException(
                message: "Invalid user access. Please correct the errors and try again.");

            invalidUserAccessProcessingException.AddData(
                key: nameof(UserAccess.EntraUserId),
                values: "Text is invalid");

            var expectedUserAccessProcessingValidationException =
                new UserAccessProcessingValidationException(
                    message: "User access processing validation error occurred, please fix errors and try again.",
                    innerException: invalidUserAccessProcessingException);

            // when
            ValueTask<List<string>> retrieveByIdUserAccessTask =
                this.userAccessProcessingService.RetrieveAllActiveOrganisationsUserHasAccessToAsync(invalidUserAccessId);

            UserAccessProcessingValidationException actualUserAccessProcessingValidationException =
                await Assert.ThrowsAsync<UserAccessProcessingValidationException>(
                    testCode: retrieveByIdUserAccessTask.AsTask);

            // then
            actualUserAccessProcessingValidationException.Should()
                .BeEquivalentTo(expectedUserAccessProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserAccessProcessingValidationException))), Times.Once());

            this.userAccessServiceMock.Verify(broker =>
                broker.RetrieveAllActiveOrganisationsUserHasAccessToAsync(invalidUserAccessId),
                    Times.Never);

            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
