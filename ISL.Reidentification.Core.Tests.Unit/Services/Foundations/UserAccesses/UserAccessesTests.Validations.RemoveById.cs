// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAccesses
{
    public partial class UserAccessesTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdUserAccessWithInvalidIdAndLogItAsync()
        {
            // given
            Guid invalidUserAccessId = Guid.Empty;

            var invalidUserAccessException = new InvalidUserAccessException(
                message: "Invalid user access. Please correct the errors and try again.");

            invalidUserAccessException.AddData(
                key: nameof(UserAccess.Id),
                values: "Id is invalid");

            var expectedUserAccessValidationException =
                new UserAccessValidationException(
                    message: "UserAccess validation error occurred, please fix errors and try again.",
                    innerException: invalidUserAccessException);

            // when
            ValueTask<UserAccess> removeByIdUserAccessTask = this.userAccessService
                .RemoveUserAccessByIdAsync(invalidUserAccessId);

            UserAccessValidationException actualUserAccessValidationException =
                await Assert.ThrowsAsync<UserAccessValidationException>(
                    testCode: removeByIdUserAccessTask.AsTask);

            // then
            actualUserAccessValidationException.Should().BeEquivalentTo(expectedUserAccessValidationException);
            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedUserAccessValidationException))),
                    Times.Once());

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectUserAccessByIdAsync(invalidUserAccessId),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfStorageUserAccessDoesNotExistAndLogItAsync()
        {
            // given
            UserAccess randomUserAccess = CreateRandomUserAccess();
            UserAccess nonExistingUserAccess = randomUserAccess;
            UserAccess nullUserAccess = null;

            var notFoundUserAccessException = new NotFoundUserAccessException(
                message: $"User access not found with Id: {nonExistingUserAccess.Id}");

            var expectedUserAccessValidationException = new UserAccessValidationException(
                message: "UserAccess validation error occurred, please fix errors and try again.",
                innerException: notFoundUserAccessException);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectUserAccessByIdAsync(nonExistingUserAccess.Id))
                    .ReturnsAsync(nullUserAccess);

            // when
            ValueTask<UserAccess> removeByIdUserAccessTask =
                this.userAccessService.RemoveUserAccessByIdAsync(nonExistingUserAccess.Id);

            UserAccessValidationException actualUserAccessVaildationException =
                await Assert.ThrowsAsync<UserAccessValidationException>(
                    testCode: removeByIdUserAccessTask.AsTask);

            // then
            actualUserAccessVaildationException.Should().BeEquivalentTo(expectedUserAccessValidationException);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectUserAccessByIdAsync(nonExistingUserAccess.Id),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(
                   SameExceptionAs(expectedUserAccessValidationException))),
                       Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }
    }
}
