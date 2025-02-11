// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using ISL.ReIdentification.Core.Models.Foundations.Lookups.Exceptions;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Foundations.Lookups;
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

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfLookupIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            EntraUser randomEntraUser = CreateRandomEntraUser();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);

            var invalidLookup = new Lookup
            {
                GroupName = invalidText,
                Name = invalidText
            };

            var lookupServiceMock = new Mock<LookupService>(
                this.reIdentificationStorageBroker.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            lookupServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidLookup))
                    .ReturnsAsync(invalidLookup);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidLookupException =
                new InvalidLookupException(
                    message: "Invalid lookup. Please correct the errors and try again.");

            invalidLookupException.AddData(
                key: nameof(Lookup.Id),
                values: "Id is invalid");

            invalidLookupException.AddData(
                key: nameof(Lookup.GroupName),
                values: "Text is invalid");

            invalidLookupException.AddData(
                key: nameof(Lookup.Name),
                values: "Text is invalid");

            invalidLookupException.AddData(
                key: nameof(Lookup.CreatedDate),
                values:
                    [
                        "Date is invalid",
                        $"Date is not recent. Expected a value between " +
                            $"{startDate} and {endDate} but found {invalidLookup.CreatedDate}"
                    ]);

            invalidLookupException.AddData(
                key: nameof(Lookup.CreatedBy),
                values:
                    [
                        "Text is invalid",
                        $"Expected value to be '{randomEntraUser.EntraUserId}' but found " +
                            $"'{invalidLookup.CreatedBy}'."
                    ]);

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
                lookupServiceMock.Object.AddLookupAsync(invalidLookup);

            LookupValidationException actualLookupValidationException =
                await Assert.ThrowsAsync<LookupValidationException>(
                    testCode: addLookupTask.AsTask);

            // then
            actualLookupValidationException.Should()
                .BeEquivalentTo(expectedLookupValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedLookupValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertLookupAsync(It.IsAny<Lookup>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfLookupHasInvalidLengthProperty()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser(entraUserId: GetRandomStringWithLengthOf(256));
            string randomString = randomEntraUser.EntraUserId;

            var invalidLookup = CreateRandomLookup(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            invalidLookup.GroupName = GetRandomStringWithLengthOf(221);
            invalidLookup.Name = GetRandomStringWithLengthOf(221);
            invalidLookup.CreatedBy = randomString;
            invalidLookup.UpdatedBy = randomString;

            var lookupServiceMock = new Mock<LookupService>(
                this.reIdentificationStorageBroker.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            lookupServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidLookup))
                    .ReturnsAsync(invalidLookup);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidLookupException = new InvalidLookupException(
                message: "Invalid lookup. Please correct the errors and try again.");

            invalidLookupException.AddData(
                key: nameof(Lookup.GroupName),
                values: $"Text exceed max length of {invalidLookup.GroupName.Length - 1} characters");

            invalidLookupException.AddData(
                key: nameof(Lookup.Name),
                values: $"Text exceed max length of {invalidLookup.Name.Length - 1} characters");

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

            // when
            ValueTask<Lookup> addLookupTask =
                lookupServiceMock.Object.AddLookupAsync(invalidLookup);

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

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

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

            Lookup randomLookup = CreateRandomLookup(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            Lookup invalidLookup = randomLookup;
            invalidLookup.CreatedBy = GetRandomString();
            invalidLookup.UpdatedBy = GetRandomString();
            invalidLookup.CreatedDate = GetRandomDateTimeOffset();
            invalidLookup.UpdatedDate = GetRandomDateTimeOffset();

            var lookupServiceMock = new Mock<LookupService>(
                this.reIdentificationStorageBroker.Object,
                this.dateTimeBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            lookupServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidLookup))
                    .ReturnsAsync(invalidLookup);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidLookupException = new InvalidLookupException(
                message: "Invalid lookup. Please correct the errors and try again.");

            invalidLookupException.AddData(
                key: nameof(Lookup.CreatedBy),
                values:
                    $"Expected value to be '{randomEntraUser.EntraUserId}' " +
                    $"but found '{invalidLookup.CreatedBy}'.");

            invalidLookupException.AddData(
                key: nameof(Lookup.UpdatedBy),
                values: $"Text is not the same as {nameof(Lookup.CreatedBy)}");

            invalidLookupException.AddData(
                key: nameof(Lookup.UpdatedDate),
                values: $"Date is not the same as {nameof(Lookup.CreatedDate)}");

            invalidLookupException.AddData(
                key: nameof(Lookup.CreatedDate),
                values:
                    $"Date is not recent." +
                    $" Expected a value between {startDate} and {endDate} but found {invalidLookup.CreatedDate}");

            var expectedLookupValidationException =
                new LookupValidationException(
                    message: "Lookup validation error occurred, please fix errors and try again.",
                    innerException: invalidLookupException);

            // when
            ValueTask<Lookup> addLookupTask =
                lookupServiceMock.Object.AddLookupAsync(invalidLookup);

            LookupValidationException actualLookupValidationException =
                await Assert.ThrowsAsync<LookupValidationException>(
                    testCode: addLookupTask.AsTask);

            // then
            actualLookupValidationException.Should().BeEquivalentTo(
                expectedLookupValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedLookupValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertLookupAsync(It.IsAny<Lookup>()),
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
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }
    }
}