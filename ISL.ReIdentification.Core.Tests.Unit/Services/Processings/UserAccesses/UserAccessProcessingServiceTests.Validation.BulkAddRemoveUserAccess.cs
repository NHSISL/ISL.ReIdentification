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
            ValueTask addUserAccessTask = this.userAccessProcessingService
                .BulkAddRemoveUserAccessAsync(nullBulkUserAccess);

            UserAccessProcessingValidationException actualUserAccessProcessingValidationException =
                await Assert.ThrowsAsync<UserAccessProcessingValidationException>(
                    testCode: addUserAccessTask.AsTask);

            // then
            actualUserAccessProcessingValidationException.Should()
                .BeEquivalentTo(expectedUserAccessProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedUserAccessProcessingValidationException))),
                    Times.Once());

            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
