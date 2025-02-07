// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses.Exceptions;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Foundations.UserAccesses;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAccesses
{
    public partial class UserAccessesTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddUserAccessAsync()
        {
            // given
            UserAccess nullUserAccess = null;
            var nullUserAccessException = new NullUserAccessException(message: "User access is null.");

            var userAccessServiceMock = new Mock<UserAccessService>(
                reIdentificationStorageBroker.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            userAccessServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(nullUserAccess))
                    .ReturnsAsync(nullUserAccess);

            var expectedUserAccessValidationException =
                new UserAccessValidationException(
                    message: "UserAccess validation error occurred, please fix errors and try again.",
                    innerException: nullUserAccessException);

            // when
            ValueTask<UserAccess> addUserAccessTask = userAccessServiceMock.Object.AddUserAccessAsync(nullUserAccess);

            UserAccessValidationException actualUserAccessValidationException =
                await Assert.ThrowsAsync<UserAccessValidationException>(
                    testCode: addUserAccessTask.AsTask);

            // then
            actualUserAccessValidationException.Should().BeEquivalentTo(expectedUserAccessValidationException);
            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedUserAccessValidationException))), Times.Once());

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertUserAccessAsync(It.IsAny<UserAccess>()),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfUserAccessIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            EntraUser randomEntraUser = CreateRandomEntraUser();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            var invalidUserAccess = new UserAccess
            {
                EntraUserId = invalidText,
                Email = invalidText,
                OrgCode = invalidText,
                CreatedBy = invalidText,
                UpdatedBy = invalidText,
            };

            var userAccessServiceMock = new Mock<UserAccessService>(
                reIdentificationStorageBroker.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            userAccessServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidUserAccess))
                    .ReturnsAsync(invalidUserAccess);

            this.dateTimeBrokerMock.SetupSequence(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.SetupSequence(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidUserAccessException =
                new InvalidUserAccessException(
                    message: "Invalid user access. Please correct the errors and try again.");

            invalidUserAccessException.AddData(
                key: nameof(UserAccess.Id),
                values: "Id is invalid");

            invalidUserAccessException.AddData(
                key: nameof(UserAccess.EntraUserId),
                values: "Text is invalid");

            invalidUserAccessException.AddData(
                key: nameof(UserAccess.Email),
                values: "Text is invalid");

            invalidUserAccessException.AddData(
                key: nameof(UserAccess.OrgCode),
                values: "Text is invalid");

            invalidUserAccessException.AddData(
                key: nameof(UserAccess.CreatedBy),
                values: [
                    "Text is invalid",
                    $"Expected value to be '{randomEntraUser.EntraUserId}' but found '{invalidText}'."
                ]);

            invalidUserAccessException.AddData(
                key: nameof(UserAccess.CreatedDate),
                values: [
                    "Date is invalid",
                    $"Expected value to be '{randomDateTimeOffset}' but found '{invalidUserAccess.CreatedDate}'."
                ]);

            invalidUserAccessException.AddData(
                key: nameof(UserAccess.UpdatedBy),
                values: "Text is invalid");

            invalidUserAccessException.AddData(
                key: nameof(UserAccess.UpdatedDate),
                values: "Date is invalid");




            var expectedUserAccessValidationException =
                new UserAccessValidationException(
                    message: "UserAccess validation error occurred, please fix errors and try again.",
                    innerException: invalidUserAccessException);

            // when
            ValueTask<UserAccess> addUserAccessTask =
                userAccessServiceMock.Object.AddUserAccessAsync(invalidUserAccess);

            UserAccessValidationException actualUserAccessValidationException =
                await Assert.ThrowsAsync<UserAccessValidationException>(
                    testCode: addUserAccessTask.AsTask);

            // then
            actualUserAccessValidationException.Should()
                .BeEquivalentTo(expectedUserAccessValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserAccessValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertUserAccessAsync(It.IsAny<UserAccess>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfUserAccessHasInvalidLengthProperty()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            var inputCreatedByUpdatedByString = GetRandomStringWithLength(256);
            EntraUser randomEntraUser = CreateRandomEntraUser();

            UserAccess invalidUserAccess =
                CreateRandomUserAccess(randomDateTimeOffset, randomEntraUser.EntraUserId.ToString());

            invalidUserAccess.GivenName = GetRandomStringWithLength(256);
            invalidUserAccess.Surname = GetRandomStringWithLength(256);
            invalidUserAccess.Email = GetRandomStringWithLength(321);
            invalidUserAccess.OrgCode = GetRandomStringWithLength(16);

            var userAccessServiceMock = new Mock<UserAccessService>(
                reIdentificationStorageBroker.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            userAccessServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidUserAccess))
                    .ReturnsAsync(invalidUserAccess);

            var invalidUserAccessException = new InvalidUserAccessException(
                message: "Invalid user access. Please correct the errors and try again.");

            invalidUserAccessException.AddData(
                key: nameof(UserAccess.Email),
                values: $"Text exceed max length of {invalidUserAccess.Email.Length - 1} characters");

            invalidUserAccessException.AddData(
                key: nameof(UserAccess.OrgCode),
                values: $"Text exceed max length of {invalidUserAccess.OrgCode.Length - 1} characters");

            var expectedUserAccessValidationException =
                new UserAccessValidationException(
                    message: "UserAccess validation error occurred, please fix errors and try again.",
                    innerException: invalidUserAccessException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            // when
            ValueTask<UserAccess> addUserAccessTask =
                userAccessServiceMock.Object.AddUserAccessAsync(invalidUserAccess);

            UserAccessValidationException actualUserAccessValidationException =
                await Assert.ThrowsAsync<UserAccessValidationException>(
                    testCode: addUserAccessTask.AsTask);

            // then
            actualUserAccessValidationException.Should()
                .BeEquivalentTo(expectedUserAccessValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserAccessValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertUserAccessAsync(It.IsAny<UserAccess>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfAuditPropertiesIsNotTheSameAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            DateTimeOffset randomAuditDateTime = randomDateTime.AddSeconds(+1);
            EntraUser randomEntraUser = CreateRandomEntraUser();
            EntraUser randomAuditEntraUser = CreateRandomEntraUser();

            UserAccess randomUserAccess =
                CreateRandomModifyUserAccess(randomDateTime, randomEntraUser.EntraUserId.ToString());

            UserAccess invalidUserAccess = randomUserAccess;

            var userAccessServiceMock = new Mock<UserAccessService>(
                reIdentificationStorageBroker.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            userAccessServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidUserAccess))
                    .ReturnsAsync(invalidUserAccess);

            var invalidUserAccessException = new InvalidUserAccessException(
                message: "Invalid user access. Please correct the errors and try again.");

            invalidUserAccessException.AddData(
                key: nameof(UserAccess.CreatedBy),
                values:
                    $"Expected value to be '{randomAuditEntraUser.EntraUserId.ToString()}' " +
                    $"but found '{randomEntraUser.EntraUserId.ToString()}'.");

            var expectedUserAccessValidationException =
                new UserAccessValidationException(
                    message: "UserAccess validation error occurred, please fix errors and try again.",
                    innerException: invalidUserAccessException);

            this.dateTimeBrokerMock.SetupSequence(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime)
                        .ReturnsAsync(randomAuditDateTime);

            this.securityBrokerMock.SetupSequence(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser)
                        .ReturnsAsync(randomAuditEntraUser);

            // when
            ValueTask<UserAccess> addUserAccessTask =
                userAccessServiceMock.Object.AddUserAccessAsync(invalidUserAccess);

            UserAccessValidationException actualUserAccessValidationException =
                await Assert.ThrowsAsync<UserAccessValidationException>(
                    testCode: addUserAccessTask.AsTask);

            // then
            actualUserAccessValidationException.Should().BeEquivalentTo(
                expectedUserAccessValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Exactly(2));

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedUserAccessValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertUserAccessAsync(It.IsAny<UserAccess>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}
