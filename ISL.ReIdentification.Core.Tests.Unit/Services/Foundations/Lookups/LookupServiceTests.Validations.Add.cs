// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using ISL.ReIdentification.Core.Models.Foundations.Lookups.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Lookups
{
    public partial class LookupServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfLookupIsNullAndLogItAsync()
        {
            // given
            Lookup nullLookup = null;

            var nullLookupException =
                new NullLookupException(message: "Lookup is null.");

            var expectedLookupValidationException =
                new LookupValidationException(
                    message: "Lookup validation error occurred, please fix errors and try again.",
                    innerException: nullLookupException);

            // when
            ValueTask<Lookup> addLookupTask =
                this.lookupService.AddLookupAsync(nullLookup);

            LookupValidationException actualLookupValidationException =
                await Assert.ThrowsAsync<LookupValidationException>(
                    testCode: addLookupTask.AsTask);

            // then
            actualLookupValidationException.Should()
                .BeEquivalentTo(expectedLookupValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedLookupValidationException))),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfLookupIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            var invalidLookup = new Lookup
            {
                Name = invalidText
            };

            var invalidLookupException =
                new InvalidLookupException(
                    message: "Invalid lookup. Please correct the errors and try again.");

            invalidLookupException.AddData(
                key: nameof(Lookup.Id),
                values: "Id is invalid");

            invalidLookupException.AddData(
                key: nameof(Lookup.Name),
                values: "Text is invalid");

            invalidLookupException.AddData(
                key: nameof(Lookup.CreatedDate),
                values: "Date is invalid");

            invalidLookupException.AddData(
                key: nameof(Lookup.CreatedBy),
                values: "Text is invalid");

            invalidLookupException.AddData(
                key: nameof(Lookup.UpdatedDate),
                values: "Date is invalid");

            invalidLookupException.AddData(
                key: nameof(Lookup.UpdatedBy),
                values: "Text is invalid");

            var expectedLookupValidationException =
                new LookupValidationException(
                    message: "Lookup validation error occurred, please fix errors and try again.",
                    innerException: invalidLookupException);

            // when
            ValueTask<Lookup> addLookupTask =
                this.lookupService.AddLookupAsync(invalidLookup);

            LookupValidationException actualLookupValidationException =
                await Assert.ThrowsAsync<LookupValidationException>(
                    testCode: addLookupTask.AsTask);

            // then
            actualLookupValidationException.Should()
                .BeEquivalentTo(expectedLookupValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedLookupValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertLookupAsync(It.IsAny<Lookup>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfLookupHasInvalidLengthProperty()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            string randomString = GetRandomStringWithLengthOf(256);
            var invalidLookup = CreateRandomLookup(dateTimeOffset: randomDateTimeOffset);
            invalidLookup.CreatedBy = randomString;
            invalidLookup.UpdatedBy = randomString;

            var invalidLookupException = new InvalidLookupException(
                message: "Invalid lookup. Please correct the errors and try again.");

            invalidLookupException.AddData(
                key: nameof(Lookup.CreatedBy),
                values: $"Text exceed max length of {invalidLookup.CreatedBy.Length - 1} characters");

            invalidLookupException.AddData(
                key: nameof(Lookup.UpdatedBy),
                values: $"Text exceed max length of {invalidLookup.UpdatedBy.Length - 1} characters");

            var expectedLookupValidationException =
                new LookupValidationException(
                    message: "Lookup validation error occurred, please fix errors and try again.",
                    innerException: invalidLookupException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Lookup> addLookupTask =
                this.lookupService.AddLookupAsync(invalidLookup);

            LookupValidationException actualLookupValidationException =
                await Assert.ThrowsAsync<LookupValidationException>(
                    testCode: addLookupTask.AsTask);

            // then
            actualLookupValidationException.Should()
                .BeEquivalentTo(expectedLookupValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedLookupValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertLookupAsync(It.IsAny<Lookup>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfAuditPropertiesIsNotTheSameAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTime;
            Lookup randomLookup = CreateRandomLookup(now);
            Lookup invalidLookup = randomLookup;
            invalidLookup.CreatedBy = GetRandomString();
            invalidLookup.UpdatedBy = GetRandomString();
            invalidLookup.CreatedDate = now;
            invalidLookup.UpdatedDate = GetRandomDateTimeOffset();

            var invalidLookupException = new InvalidLookupException(
                message: "Invalid lookup. Please correct the errors and try again.");

            invalidLookupException.AddData(
                key: nameof(Lookup.UpdatedBy),
                values: $"Text is not the same as {nameof(Lookup.CreatedBy)}");

            invalidLookupException.AddData(
                key: nameof(Lookup.UpdatedDate),
                values: $"Date is not the same as {nameof(Lookup.CreatedDate)}");

            var expectedLookupValidationException =
                new LookupValidationException(
                    message: "Lookup validation error occurred, please fix errors and try again.",
                    innerException: invalidLookupException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            // when
            ValueTask<Lookup> addLookupTask =
                this.lookupService.AddLookupAsync(invalidLookup);

            LookupValidationException actualLookupValidationException =
                await Assert.ThrowsAsync<LookupValidationException>(
                    testCode: addLookupTask.AsTask);

            // then
            actualLookupValidationException.Should().BeEquivalentTo(
                expectedLookupValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedLookupValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertLookupAsync(It.IsAny<Lookup>()),
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
            DateTimeOffset startDate = now.AddSeconds(-90);
            DateTimeOffset endDate = now.AddSeconds(0);
            Lookup randomLookup = CreateRandomLookup();
            Lookup invalidLookup = randomLookup;

            DateTimeOffset invalidDate =
                now.AddSeconds(invalidSeconds);

            invalidLookup.CreatedDate = invalidDate;
            invalidLookup.UpdatedDate = invalidDate;

            var invalidLookupException = new InvalidLookupException(
                message: "Invalid lookup. Please correct the errors and try again.");

            invalidLookupException.AddData(
            key: nameof(Lookup.CreatedDate),
                values:
                    $"Date is not recent. Expected a value between " +
                    $"{startDate} and {endDate} but found {invalidDate}");

            var expectedLookupValidationException =
                new LookupValidationException(
                    message: "Lookup validation error occurred, please fix errors and try again.",
                    innerException: invalidLookupException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            // when
            ValueTask<Lookup> addLookupTask =
                this.lookupService.AddLookupAsync(invalidLookup);

            LookupValidationException actualLookupValidationException =
                await Assert.ThrowsAsync<LookupValidationException>(
                    testCode: addLookupTask.AsTask);

            // then
            actualLookupValidationException.Should().BeEquivalentTo(
                expectedLookupValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedLookupValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertLookupAsync(It.IsAny<Lookup>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }
    }
}