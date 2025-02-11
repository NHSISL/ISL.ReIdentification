// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Foundations.AccessAudits;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.AccessAudits
{
    public partial class AccessAuditTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddAccessAuditAsync()
        {
            // given
            AccessAudit nullAccessAudit = null;
            var nullAccessAuditException = new NullAccessAuditException(message: "Access audit is null.");

            var expectedAccessAuditValidationException =
                new AccessAuditValidationException(
                    message: "Access audit validation error occurred, please fix errors and try again.",
                    innerException: nullAccessAuditException);

            // when
            ValueTask<AccessAudit> addAccessAuditTask = this.accessAuditService.AddAccessAuditAsync(nullAccessAudit);

            AccessAuditValidationException actualAccessAuditValidationException =
                await Assert.ThrowsAsync<AccessAuditValidationException>(testCode: addAccessAuditTask.AsTask);

            // then
            actualAccessAuditValidationException.Should().BeEquivalentTo(expectedAccessAuditValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedAccessAuditValidationException))), Times.Once());

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfAccessAuditIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            EntraUser randomEntraUser = CreateRandomEntraUser();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);

            var invalidAccessAudit = new AccessAudit
            {
                EntraUserId = invalidText,
                PseudoIdentifier = invalidText,
                Email = invalidText,
                CreatedBy = invalidText,
                UpdatedBy = invalidText,
            };

            var accessAuditServiceMock = new Mock<AccessAuditService>(
                reIdentificationStorageBroker.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            accessAuditServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidAccessAudit))
                    .ReturnsAsync(invalidAccessAudit);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidAccessAuditException =
                new InvalidAccessAuditException(
                    message: "Invalid access audit. Please correct the errors and try again.");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.Id),
                values: "Id is invalid");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.RequestId),
                values: "Id is invalid");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.EntraUserId),
                values: "Text is invalid");

            invalidAccessAuditException.AddData(
              key: nameof(AccessAudit.AuditType),
              values: "Text is invalid");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.Email),
                values: "Text is invalid");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.PseudoIdentifier),
                values: "Text is invalid");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.CreatedDate),
                values:
                [
                    "Date is invalid",
                    $"Date is not recent. Expected a value between " +
                    $"{startDate} and {endDate} but found {invalidAccessAudit.CreatedDate}"
                ]);

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.CreatedBy),
                values:
                [
                    "Text is invalid",
                    $"Expected value to be '{randomEntraUser.EntraUserId}' but found '{invalidAccessAudit.CreatedBy}'."
                ]);

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.UpdatedDate),
                values: "Date is invalid");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.UpdatedBy),
                values: "Text is invalid");

            var expectedAccessAuditValidationException =
                new AccessAuditValidationException(
                    message: "Access audit validation error occurred, please fix errors and try again.",
                    innerException: invalidAccessAuditException);

            // when
            ValueTask<AccessAudit> addAccessAuditTask =
                accessAuditServiceMock.Object.AddAccessAuditAsync(invalidAccessAudit);

            AccessAuditValidationException actualAccessAuditValidationException =
                await Assert.ThrowsAsync<AccessAuditValidationException>(testCode: addAccessAuditTask.AsTask);

            // then
            actualAccessAuditValidationException.Should()
                .BeEquivalentTo(expectedAccessAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAccessAuditValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfAccessAuditHasInvalidLengthProperty()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            AccessAudit invalidAccessAudit = CreateRandomAccessAudit(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            var inputCreatedByUpdatedByString = GetRandomStringWithLengthOf(256);
            invalidAccessAudit.EntraUserId = GetRandomStringWithLengthOf(256);
            invalidAccessAudit.Email = GetRandomStringWithLengthOf(321);
            invalidAccessAudit.PseudoIdentifier = GetRandomStringWithLengthOf(11);
            invalidAccessAudit.AuditType = GetRandomStringWithLengthOf(256);
            invalidAccessAudit.CreatedBy = inputCreatedByUpdatedByString;
            invalidAccessAudit.UpdatedBy = inputCreatedByUpdatedByString;

            var accessAuditServiceMock = new Mock<AccessAuditService>(
                reIdentificationStorageBroker.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            accessAuditServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidAccessAudit))
                    .ReturnsAsync(invalidAccessAudit);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidAccessAuditException = new InvalidAccessAuditException(
                message: "Invalid access audit. Please correct the errors and try again.");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.PseudoIdentifier),
                values: $"Text exceed max length of {invalidAccessAudit.PseudoIdentifier.Length - 1} characters");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.EntraUserId),
                values: $"Text exceed max length of {invalidAccessAudit.EntraUserId.Length - 1} characters");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.Email),
                values: $"Text exceed max length of {invalidAccessAudit.Email.Length - 1} characters");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.AuditType),
                values: $"Text exceed max length of {invalidAccessAudit.AuditType.Length - 1} characters");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.CreatedBy),
                values:
                [
                    $"Text exceed max length of {invalidAccessAudit.CreatedBy.Length - 1} characters",
                    $"Expected value to be '{randomEntraUser.EntraUserId}' but found '{invalidAccessAudit.CreatedBy}'."
                ]);

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.UpdatedBy),
                values: $"Text exceed max length of {invalidAccessAudit.UpdatedBy.Length - 1} characters");

            var expectedAccessAuditValidationException =
                new AccessAuditValidationException(
                    message: "Access audit validation error occurred, please fix errors and try again.",
                    innerException: invalidAccessAuditException);

            // when
            ValueTask<AccessAudit> addAccessAuditTask =
                accessAuditServiceMock.Object.AddAccessAuditAsync(invalidAccessAudit);

            AccessAuditValidationException actualAccessAuditValidationException =
                await Assert.ThrowsAsync<AccessAuditValidationException>(
                    testCode: addAccessAuditTask.AsTask);

            // then
            actualAccessAuditValidationException.Should()
                .BeEquivalentTo(expectedAccessAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAccessAuditValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfAuditPropertiesIsNotTheSameAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);
            EntraUser randomEntraUser = CreateRandomEntraUser();

            AccessAudit randomAccessAudit = CreateRandomAccessAudit(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            AccessAudit invalidAccessAudit = randomAccessAudit;
            invalidAccessAudit.CreatedBy = GetRandomString();
            invalidAccessAudit.UpdatedBy = GetRandomString();
            invalidAccessAudit.CreatedDate = GetRandomDateTimeOffset();
            invalidAccessAudit.UpdatedDate = GetRandomDateTimeOffset();

            var accessAuditServiceMock = new Mock<AccessAuditService>(
                reIdentificationStorageBroker.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            accessAuditServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidAccessAudit))
                    .ReturnsAsync(invalidAccessAudit);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidAccessAuditException = new InvalidAccessAuditException(
                message: "Invalid access audit. Please correct the errors and try again.");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.CreatedBy),
                values:
                    $"Expected value to be '{randomEntraUser.EntraUserId}' " +
                    $"but found '{invalidAccessAudit.CreatedBy}'.");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.UpdatedBy),
                values: $"Text is not the same as {nameof(AccessAudit.CreatedBy)}");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.UpdatedDate),
                values: $"Date is not the same as {nameof(AccessAudit.CreatedDate)}");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.CreatedDate),
                values:
                    $"Date is not recent." +
                    $" Expected a value between {startDate} and {endDate} but found {invalidAccessAudit.CreatedDate}");

            var expectedAccessAuditValidationException =
                new AccessAuditValidationException(
                    message: "Access audit validation error occurred, please fix errors and try again.",
                    innerException: invalidAccessAuditException);

            // when
            ValueTask<AccessAudit> addAccessAuditTask =
                accessAuditServiceMock.Object.AddAccessAuditAsync(invalidAccessAudit);

            AccessAuditValidationException actualAccessAuditValidationException =
                await Assert.ThrowsAsync<AccessAuditValidationException>(
                    testCode: addAccessAuditTask.AsTask);

            // then
            actualAccessAuditValidationException.Should().BeEquivalentTo(
                expectedAccessAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedAccessAuditValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-91)]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
            int invalidSeconds)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);
            EntraUser randomEntraUser = CreateRandomEntraUser();

            AccessAudit randomAccessAudit = CreateRandomAccessAudit(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            AccessAudit invalidAccessAudit = randomAccessAudit;

            DateTimeOffset invalidDate =
                randomDateTimeOffset.AddSeconds(invalidSeconds);

            invalidAccessAudit.CreatedDate = invalidDate;
            invalidAccessAudit.UpdatedDate = invalidDate;

            var accessAuditServiceMock = new Mock<AccessAuditService>(
                reIdentificationStorageBroker.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            accessAuditServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidAccessAudit))
                    .ReturnsAsync(invalidAccessAudit);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidAccessAuditException = new InvalidAccessAuditException(
                message: "Invalid access audit. Please correct the errors and try again.");

            invalidAccessAuditException.AddData(
            key: nameof(AccessAudit.CreatedDate),
                values:
                    $"Date is not recent. Expected a value between " +
                    $"{startDate} and {endDate} but found {invalidDate}");

            var expectedAccessAuditValidationException =
                new AccessAuditValidationException(
                    message: "Access audit validation error occurred, please fix errors and try again.",
                    innerException: invalidAccessAuditException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<AccessAudit> addAccessAuditTask =
                accessAuditServiceMock.Object.AddAccessAuditAsync(invalidAccessAudit);

            AccessAuditValidationException actualAccessAuditValidationException =
                await Assert.ThrowsAsync<AccessAuditValidationException>(
                    testCode: addAccessAuditTask.AsTask);

            // then
            actualAccessAuditValidationException.Should().BeEquivalentTo(
                expectedAccessAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedAccessAuditValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }
    }
}
