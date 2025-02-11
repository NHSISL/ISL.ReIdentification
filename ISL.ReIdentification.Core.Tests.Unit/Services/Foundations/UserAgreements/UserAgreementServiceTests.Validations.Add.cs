// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using ISL.ReIdentification.Core.Models.Foundations.UserAgreements.Exceptions;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Foundations.UserAgreements;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAgreements
{
    public partial class UserAgreementServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfUserAgreementIsNullAndLogItAsync()
        {
            // given
            UserAgreement nullUserAgreement = null;

            var nullUserAgreementException =
                new NullUserAgreementException(message: "UserAgreement is null.");

            var expectedUserAgreementValidationException =
                new UserAgreementValidationException(
                    message: "UserAgreement validation errors occurred, please try again.",
                    innerException: nullUserAgreementException);

            // when
            ValueTask<UserAgreement> addUserAgreementTask =
                this.userAgreementService.AddUserAgreementAsync(nullUserAgreement);

            UserAgreementValidationException actualUserAgreementValidationException =
                await Assert.ThrowsAsync<UserAgreementValidationException>(() =>
                    addUserAgreementTask.AsTask());

            // then
            actualUserAgreementValidationException.Should()
                .BeEquivalentTo(expectedUserAgreementValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserAgreementValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfUserAgreementIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            EntraUser randomEntraUser = CreateRandomEntraUser();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);

            var invalidUserAgreement = new UserAgreement
            {
                EntraUserId = invalidText,
                AgreementType = invalidText,
                AgreementVersion = invalidText,
                CreatedBy = invalidText,
                UpdatedBy = invalidText,
            };

            var userAgreementServiceMock = new Mock<UserAgreementService>(
                this.reIdentificationStorageBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            userAgreementServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidUserAgreement))
                    .ReturnsAsync(invalidUserAgreement);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidUserAgreementException =
                new InvalidUserAgreementException(
                    message: "Invalid userAgreement. Please correct the errors and try again.");

            invalidUserAgreementException.AddData(
                key: nameof(UserAgreement.Id),
                values: "Id is required");

            invalidUserAgreementException.AddData(
                key: nameof(UserAgreement.EntraUserId),
                values: "Text is required");

            invalidUserAgreementException.AddData(
                key: nameof(UserAgreement.AgreementType),
                values: "Text is required");

            invalidUserAgreementException.AddData(
                key: nameof(UserAgreement.AgreementVersion),
                values: "Text is required");

            invalidUserAgreementException.AddData(
                key: nameof(UserAgreement.AgreementDate),
                values: "Date is required");

            invalidUserAgreementException.AddData(
                key: nameof(UserAgreement.CreatedDate),
                values:
                [
                    "Date is required",
                    $"Date is not recent. Expected a value between " +
                        $"{startDate} and {endDate} but found {invalidUserAgreement.CreatedDate}"
                ]);

            invalidUserAgreementException.AddData(
                key: nameof(UserAgreement.CreatedBy),
                values:
                [
                    "Text is required",
                    $"Expected value to be '{randomEntraUser.EntraUserId}' but found " +
                        $"'{invalidUserAgreement.CreatedBy}'."
                ]);

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
            ValueTask<UserAgreement> addUserAgreementTask =
                userAgreementServiceMock.Object.AddUserAgreementAsync(invalidUserAgreement);

            UserAgreementValidationException actualUserAgreementValidationException =
                await Assert.ThrowsAsync<UserAgreementValidationException>(() =>
                    addUserAgreementTask.AsTask());

            // then
            actualUserAgreementValidationException.Should()
                .BeEquivalentTo(expectedUserAgreementValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserAgreementValidationException))),
                        Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.InsertUserAgreementAsync(It.IsAny<UserAgreement>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfUserAgreementHasInvalidLengthProperty()
        {
            // given
            EntraUser randomEntraUser = CreateRandomEntraUser(entraUserId: GetRandomStringWithLengthOf(256));
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);

            UserAgreement invalidUserAgreement = CreateRandomUserAgreement(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            invalidUserAgreement.EntraUserId = GetRandomStringWithLengthOf(256);
            invalidUserAgreement.AgreementType = GetRandomStringWithLengthOf(256);
            invalidUserAgreement.AgreementVersion = GetRandomStringWithLengthOf(51);
            invalidUserAgreement.CreatedBy = randomEntraUser.EntraUserId;
            invalidUserAgreement.UpdatedBy = randomEntraUser.EntraUserId;

            var userAgreementServiceMock = new Mock<UserAgreementService>(
                this.reIdentificationStorageBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            userAgreementServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidUserAgreement))
                    .ReturnsAsync(invalidUserAgreement);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidUserAgreementException =
                new InvalidUserAgreementException(
                    message: "Invalid userAgreement. Please correct the errors and try again.");

            invalidUserAgreementException.AddData(
                key: nameof(UserAgreement.EntraUserId),
                values: $"Text exceed max length of {invalidUserAgreement.EntraUserId.Length - 1} characters");

            invalidUserAgreementException.AddData(
                key: nameof(UserAgreement.AgreementType),
                values: $"Text exceed max length of {invalidUserAgreement.AgreementType.Length - 1} characters");

            invalidUserAgreementException.AddData(
                key: nameof(UserAgreement.AgreementVersion),
                values: $"Text exceed max length of {invalidUserAgreement.AgreementVersion.Length - 1} characters");

            invalidUserAgreementException.AddData(
                key: nameof(UserAgreement.CreatedBy),
                values: $"Text exceed max length of {invalidUserAgreement.CreatedBy.Length - 1} characters");

            invalidUserAgreementException.AddData(
                key: nameof(UserAgreement.UpdatedBy),
                values: $"Text exceed max length of {invalidUserAgreement.UpdatedBy.Length - 1} characters");

            var expectedUserAgreementValidationException =
                new UserAgreementValidationException(
                    message: "UserAgreement validation errors occurred, please try again.",
                    innerException: invalidUserAgreementException);

            // when
            ValueTask<UserAgreement> addUserAgreementTask =
                userAgreementServiceMock.Object.AddUserAgreementAsync(invalidUserAgreement);

            UserAgreementValidationException actualUserAgreementValidationException =
                await Assert.ThrowsAsync<UserAgreementValidationException>(() =>
                    addUserAgreementTask.AsTask());

            // then
            actualUserAgreementValidationException.Should()
                .BeEquivalentTo(expectedUserAgreementValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserAgreementValidationException))),
                        Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.InsertUserAgreementAsync(It.IsAny<UserAgreement>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateDatesIsNotSameAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            UserAgreement randomUserAgreement = CreateRandomUserAgreement(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            UserAgreement invalidUserAgreement = randomUserAgreement;

            invalidUserAgreement.UpdatedDate =
                invalidUserAgreement.CreatedDate.AddDays(randomNumber);

            var userAgreementServiceMock = new Mock<UserAgreementService>(
                this.reIdentificationStorageBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            userAgreementServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidUserAgreement))
                    .ReturnsAsync(invalidUserAgreement);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidUserAgreementException =
                new InvalidUserAgreementException(
                    message: "Invalid userAgreement. Please correct the errors and try again.");

            invalidUserAgreementException.AddData(
                key: nameof(UserAgreement.UpdatedDate),
                values: $"Date is not the same as {nameof(UserAgreement.CreatedDate)}");

            var expectedUserAgreementValidationException =
                new UserAgreementValidationException(
                    message: "UserAgreement validation errors occurred, please try again.",
                    innerException: invalidUserAgreementException);

            // when
            ValueTask<UserAgreement> addUserAgreementTask =
                userAgreementServiceMock.Object.AddUserAgreementAsync(invalidUserAgreement);

            UserAgreementValidationException actualUserAgreementValidationException =
                await Assert.ThrowsAsync<UserAgreementValidationException>(() =>
                    addUserAgreementTask.AsTask());

            // then
            actualUserAgreementValidationException.Should()
                .BeEquivalentTo(expectedUserAgreementValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserAgreementValidationException))),
                        Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.InsertUserAgreementAsync(It.IsAny<UserAgreement>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateUsersIsNotSameAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            UserAgreement randomUserAgreement = CreateRandomUserAgreement(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            UserAgreement invalidUserAgreement = randomUserAgreement;
            invalidUserAgreement.UpdatedBy = Guid.NewGuid().ToString();

            var userAgreementServiceMock = new Mock<UserAgreementService>(
                this.reIdentificationStorageBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            userAgreementServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidUserAgreement))
                    .ReturnsAsync(invalidUserAgreement);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidUserAgreementException =
                new InvalidUserAgreementException(
                    message: "Invalid userAgreement. Please correct the errors and try again.");

            invalidUserAgreementException.AddData(
                key: nameof(UserAgreement.UpdatedBy),
                values: $"Text is not the same as {nameof(UserAgreement.CreatedBy)}");

            var expectedUserAgreementValidationException =
                new UserAgreementValidationException(
                    message: "UserAgreement validation errors occurred, please try again.",
                    innerException: invalidUserAgreementException);

            // when
            ValueTask<UserAgreement> addUserAgreementTask =
                userAgreementServiceMock.Object.AddUserAgreementAsync(invalidUserAgreement);

            UserAgreementValidationException actualUserAgreementValidationException =
                await Assert.ThrowsAsync<UserAgreementValidationException>(() =>
                    addUserAgreementTask.AsTask());

            // then
            actualUserAgreementValidationException.Should()
                .BeEquivalentTo(expectedUserAgreementValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedUserAgreementValidationException))),
                        Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.InsertUserAgreementAsync(It.IsAny<UserAgreement>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-91)]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
            int invalidSeconds)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTimeOffset;
            DateTimeOffset startDate = now.AddSeconds(-90);
            DateTimeOffset endDate = now.AddSeconds(0);
            EntraUser randomEntraUser = CreateRandomEntraUser();

            UserAgreement randomUserAgreement = CreateRandomUserAgreement(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            UserAgreement invalidUserAgreement = randomUserAgreement;

            DateTimeOffset invalidDate =
                now.AddSeconds(invalidSeconds);

            invalidUserAgreement.CreatedDate = invalidDate;
            invalidUserAgreement.UpdatedDate = invalidDate;

            var userAgreementServiceMock = new Mock<UserAgreementService>(
                this.reIdentificationStorageBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            userAgreementServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidUserAgreement))
                    .ReturnsAsync(invalidUserAgreement);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidUserAgreementException = new InvalidUserAgreementException(
                message: "Invalid userAgreement. Please correct the errors and try again.");

            invalidUserAgreementException.AddData(
            key: nameof(UserAgreement.CreatedDate),
                values:
                    $"Date is not recent. Expected a value between " +
                    $"{startDate} and {endDate} but found {invalidDate}");

            var expectedUserAgreementValidationException =
                new UserAgreementValidationException(
                    message: "UserAgreement validation errors occurred, please try again.",
                    innerException: invalidUserAgreementException);

            // when
            ValueTask<UserAgreement> addUserAgreementTask =
                userAgreementServiceMock.Object.AddUserAgreementAsync(invalidUserAgreement);

            UserAgreementValidationException actualUserAgreementValidationException =
                await Assert.ThrowsAsync<UserAgreementValidationException>(
                    testCode: addUserAgreementTask.AsTask);

            // then
            actualUserAgreementValidationException.Should().BeEquivalentTo(
                expectedUserAgreementValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedUserAgreementValidationException))),
                        Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.InsertUserAgreementAsync(It.IsAny<UserAgreement>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
        }
    }
}