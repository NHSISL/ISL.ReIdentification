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
        public async Task ShouldThrowValidationExceptionOnModifyUserAccessAsync()
        {
            // given
            UserAccess nullUserAccess = null;

            var nullUserAccessProcessingException =
                new NullUserAccessProcessingException(message: "User access is null.");

            var expectedUserAccessProcessingValidationException =
                new UserAccessProcessingValidationException(
                    message: "User access processing validation error occurred, please fix errors and try again.",
                    innerException: nullUserAccessProcessingException);

            // when
            ValueTask<UserAccess> addUserAccessTask = this.userAccessProcessingService
                .ModifyUserAccessAsync(nullUserAccess);

            UserAccessProcessingValidationException actualUserAccessProcessingValidationException =
                await Assert.ThrowsAsync<UserAccessProcessingValidationException>(
                    testCode: addUserAccessTask.AsTask);

            // then
            actualUserAccessProcessingValidationException.Should()
                .BeEquivalentTo(expectedUserAccessProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedUserAccessProcessingValidationException))),
                    Times.Once());

            this.userAccessServiceMock.Verify(broker =>
                broker.ModifyUserAccessAsync(It.IsAny<UserAccess>()),
                    Times.Never);

            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
