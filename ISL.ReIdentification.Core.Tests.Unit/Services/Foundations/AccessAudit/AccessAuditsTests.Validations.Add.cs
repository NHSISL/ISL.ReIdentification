// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions;
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

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfAccessAuditIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            var invalidAccessAudit = new AccessAudit
            {
                PseudoIdentifier = invalidText,
                Email = invalidText,
            };

            var invalidAccessAuditException =
                new InvalidAccessAuditException(
                    message: "Invalid access audit. Please correct the errors and try again.");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.Id),
                values: "Id is invalid");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.EntraUserId),
                values: "Id is invalid");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.Email),
                values: "Text is invalid");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.PseudoIdentifier),
                values: "Text is invalid");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.CreatedDate),
                values: "Date is invalid");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.CreatedBy),
                values: "Text is invalid");

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
                this.accessAuditService.AddAccessAuditAsync(invalidAccessAudit);

            AccessAuditValidationException actualAccessAuditValidationException =
                await Assert.ThrowsAsync<AccessAuditValidationException>(testCode: addAccessAuditTask.AsTask);

            // then
            actualAccessAuditValidationException.Should()
                .BeEquivalentTo(expectedAccessAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAccessAuditValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfAccessAuditHasInvalidLengthProperty()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            AccessAudit invalidAccessAudit = CreateRandomAccessAudit(dateTimeOffset: randomDateTimeOffset);
            var inputCreatedByUpdatedByString = GetRandomStringWithLengthOf(256);
            invalidAccessAudit.Email = GetRandomStringWithLengthOf(321);
            invalidAccessAudit.PseudoIdentifier = GetRandomStringWithLengthOf(11);
            invalidAccessAudit.CreatedBy = inputCreatedByUpdatedByString;
            invalidAccessAudit.UpdatedBy = inputCreatedByUpdatedByString;

            var invalidAccessAuditException = new InvalidAccessAuditException(
                message: "Invalid access audit. Please correct the errors and try again.");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.PseudoIdentifier),
                values: $"Text exceed max length of {invalidAccessAudit.PseudoIdentifier.Length - 1} characters");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.Email),
                values: $"Text exceed max length of {invalidAccessAudit.Email.Length - 1} characters");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.CreatedBy),
                values: $"Text exceed max length of {invalidAccessAudit.CreatedBy.Length - 1} characters");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.UpdatedBy),
                values: $"Text exceed max length of {invalidAccessAudit.UpdatedBy.Length - 1} characters");

            var expectedAccessAuditValidationException =
                new AccessAuditValidationException(
                    message: "Access audit validation error occurred, please fix errors and try again.",
                    innerException: invalidAccessAuditException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<AccessAudit> addAccessAuditTask =
                this.accessAuditService.AddAccessAuditAsync(invalidAccessAudit);

            AccessAuditValidationException actualAccessAuditValidationException =
                await Assert.ThrowsAsync<AccessAuditValidationException>(
                    testCode: addAccessAuditTask.AsTask);

            // then
            actualAccessAuditValidationException.Should()
                .BeEquivalentTo(expectedAccessAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAccessAuditValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfAuditPropertiesIsNotTheSameAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTime;
            AccessAudit randomAccessAudit = CreateRandomAccessAudit(now);
            AccessAudit invalidAccessAudit = randomAccessAudit;
            invalidAccessAudit.CreatedBy = GetRandomString();
            invalidAccessAudit.UpdatedBy = GetRandomString();
            invalidAccessAudit.CreatedDate = now;
            invalidAccessAudit.UpdatedDate = GetRandomDateTimeOffset();

            var invalidAccessAuditException = new InvalidAccessAuditException(
                message: "Invalid access audit. Please correct the errors and try again.");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.UpdatedBy),
                values: $"Text is not the same as {nameof(AccessAudit.CreatedBy)}");

            invalidAccessAuditException.AddData(
                key: nameof(AccessAudit.UpdatedDate),
                values: $"Date is not the same as {nameof(AccessAudit.CreatedDate)}");

            var expectedAccessAuditValidationException =
                new AccessAuditValidationException(
                    message: "Access audit validation error occurred, please fix errors and try again.",
                    innerException: invalidAccessAuditException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            // when
            ValueTask<AccessAudit> addAccessAuditTask =
                this.accessAuditService.AddAccessAuditAsync(invalidAccessAudit);

            AccessAuditValidationException actualAccessAuditValidationException =
                await Assert.ThrowsAsync<AccessAuditValidationException>(
                    testCode: addAccessAuditTask.AsTask);

            // then
            actualAccessAuditValidationException.Should().BeEquivalentTo(
                expectedAccessAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedAccessAuditValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
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
            DateTimeOffset randomDateTime =
                GetRandomDateTimeOffset();

            DateTimeOffset now = randomDateTime;
            DateTimeOffset startDate = now.AddSeconds(-60);
            DateTimeOffset endDate = now.AddSeconds(0);
            AccessAudit randomAccessAudit = CreateRandomAccessAudit();
            AccessAudit invalidAccessAudit = randomAccessAudit;

            DateTimeOffset invalidDate =
                now.AddSeconds(invalidSeconds);

            invalidAccessAudit.CreatedDate = invalidDate;
            invalidAccessAudit.UpdatedDate = invalidDate;

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
                    .ReturnsAsync(now);

            // when
            ValueTask<AccessAudit> addAccessAuditTask =
                this.accessAuditService.AddAccessAuditAsync(invalidAccessAudit);

            AccessAuditValidationException actualAccessAuditValidationException =
                await Assert.ThrowsAsync<AccessAuditValidationException>(
                    testCode: addAccessAuditTask.AsTask);

            // then
            actualAccessAuditValidationException.Should().BeEquivalentTo(
                expectedAccessAuditValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedAccessAuditValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertAccessAuditAsync(It.IsAny<AccessAudit>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }
    }
}
