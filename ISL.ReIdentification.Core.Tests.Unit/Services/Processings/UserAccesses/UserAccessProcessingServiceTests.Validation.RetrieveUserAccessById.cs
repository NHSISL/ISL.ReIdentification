// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldThrowValidationExceptionOnRetrieveUserAccessByIdAsync()
        {
            // given
            Guid invalidUserAccessId = Guid.Empty;

            var invalidUserAccessProcessingException = new InvalidUserAccessProcessingException(
                message: "Invalid user access. Please correct the errors and try again.");

            invalidUserAccessProcessingException.AddData(
                key: nameof(UserAccess.Id),
                values: "Id is invalid");

            var expectedUserAccessProcessingValidationException =
                new UserAccessProcessingValidationException(
                    message: "User access processing validation error occurred, please fix errors and try again.",
                    innerException: invalidUserAccessProcessingException);

            // when
            ValueTask<UserAccess> retrieveByIdUserAccessTask =
                this.userAccessProcessingService.RetrieveUserAccessByIdAsync(invalidUserAccessId);

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
                broker.RetrieveUserAccessByIdAsync(invalidUserAccessId),
                    Times.Never);

            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
