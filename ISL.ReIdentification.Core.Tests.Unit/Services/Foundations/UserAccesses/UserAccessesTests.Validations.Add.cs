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
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);
            EntraUser randomEntraUser = CreateRandomEntraUser();

            UserAccess randomUserAccess =
                CreateRandomModifyUserAccess(randomDateTimeOffset, randomEntraUser.EntraUserId.ToString());

            randomUserAccess.CreatedBy = GetRandomString();
            randomUserAccess.UpdatedBy = GetRandomString();
            randomUserAccess.CreatedDate = GetRandomDateTimeOffset();
            randomUserAccess.UpdatedDate = GetRandomDateTimeOffset();

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
                    $"Expected value to be '{randomEntraUser.EntraUserId.ToString()}' " +
                    $"but found '{randomUserAccess.CreatedBy}'.");

            invalidUserAccessException.AddData(
                key: nameof(UserAccess.UpdatedBy),
                values: $"Text is not the same as {nameof(UserAccess.CreatedBy)}");

            invalidUserAccessException.AddData(
                key: nameof(UserAccess.UpdatedDate),
                values: $"Date is not the same as {nameof(UserAccess.CreatedDate)}");

            invalidUserAccessException.AddData(
                key: nameof(UserAccess.CreatedDate),
                values:
                    $"Date is not recent." +
                    $" Expected a value between {startDate} and {endDate} but found {invalidUserAccess.CreatedDate}");

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
            actualUserAccessValidationException.Should().BeEquivalentTo(
                expectedUserAccessValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

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


        [Theory]
        [InlineData(1)]
        [InlineData(-91)]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
    int invalidSeconds)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);

            UserAccess randomUserAccess =
                CreateRandomUserAccess(randomDateTimeOffset, randomEntraUser.EntraUserId.ToString());

            UserAccess invalidUserAccess = randomUserAccess;

            DateTimeOffset invalidDate =
                randomDateTimeOffset.AddSeconds(invalidSeconds);

            invalidUserAccess.CreatedDate = invalidDate;
            invalidUserAccess.UpdatedDate = invalidDate;

            var invalidUserAccessException = new InvalidUserAccessException(
                message: "Invalid user access. Please correct the errors and try again.");

            invalidUserAccessException.AddData(
            key: nameof(UserAccess.CreatedDate),
                values:
                    $"Date is not recent. Expected a value between " +
                    $"{startDate} and {endDate} but found {invalidDate}");

            var expectedUserAccessValidationException =
                new UserAccessValidationException(
                    message: "UserAccess validation error occurred, please fix errors and try again.",
                    innerException: invalidUserAccessException);

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
            actualUserAccessValidationException.Should().BeEquivalentTo(
                expectedUserAccessValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedUserAccessValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertUserAccessAsync(It.IsAny<UserAccess>()),
                    Times.Never);

            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }
    }
}
