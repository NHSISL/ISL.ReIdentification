﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.DelegatedAccesses;
using ISL.ReIdentification.Core.Models.Foundations.DelegatedAccesses.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.DelegatedAccesses
{
    public partial class DelegatedAccessesTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddDelegatedAccessAsync()
        {
            //given
            DelegatedAccess nullDelegatedAccess = null;
            var nullDelegatedAccessException = new NullDelegatedAccessException(message: "Delegated access is null.");

            var expectedDelegatedAccessValidationException =
                new DelegatedAccessValidationException(
                    message: "DelegatedAccess validation error occurred, please fix errors and try again.",
                    innerException: nullDelegatedAccessException);

            //when
            ValueTask<DelegatedAccess> addDelegatedAccessTask =
                this.delegatedAccessService.AddDelegatedAccessAsync(nullDelegatedAccess);

            DelegatedAccessValidationException actualDelegatedAccessValidationException =
                await Assert.ThrowsAsync<DelegatedAccessValidationException>(addDelegatedAccessTask.AsTask);

            //then
            actualDelegatedAccessValidationException.Should()
                .BeEquivalentTo(expectedDelegatedAccessValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedDelegatedAccessValidationException))), Times.Once());

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertDelegatedAccessAsync(It.IsAny<DelegatedAccess>()),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfDelegatedAccessIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            var invalidDelegatedAccess = new DelegatedAccess
            {
                RequesterFirstName = invalidText,
                RequesterLastName = invalidText,
                RequesterEmail = invalidText,
                RecipientFirstName = invalidText,
                RecipientLastName = invalidText,
                RecipientEmail = invalidText,
                IdentifierColumn = invalidText
            };

            var invalidDelegatedAccessException =
                new InvalidDelegatedAccessException(
                    message: "Invalid delegated access. Please correct the errors and try again.");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.Id),
                values: "Id is invalid");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.RequesterFirstName),
                values: "Text is invalid");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.RequesterLastName),
                values: "Text is invalid");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.RequesterEmail),
                values: "Text is invalid");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.RecipientFirstName),
                values: "Text is invalid");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.RecipientLastName),
                values: "Text is invalid");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.RecipientEmail),
                values: "Text is invalid");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.IdentifierColumn),
                values: "Text is invalid");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.CreatedDate),
                values: "Date is invalid");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.CreatedBy),
                values: "Text is invalid");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.UpdatedDate),
                values: "Date is invalid");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.UpdatedBy),
                values: "Text is invalid");

            var expectedDelegatedAccessValidationException =
                new DelegatedAccessValidationException(
                    message: "DelegatedAccess validation error occurred, please fix errors and try again.",
                    innerException: invalidDelegatedAccessException);

            // when
            ValueTask<DelegatedAccess> addDelegatedAccessTask =
                this.delegatedAccessService.AddDelegatedAccessAsync(invalidDelegatedAccess);

            DelegatedAccessValidationException actualDelegatedAccessValidationException =
                await Assert.ThrowsAsync<DelegatedAccessValidationException>(addDelegatedAccessTask.AsTask);

            // then
            actualDelegatedAccessValidationException.Should()
                .BeEquivalentTo(expectedDelegatedAccessValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDelegatedAccessValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertDelegatedAccessAsync(It.IsAny<DelegatedAccess>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfDelegatedAccessHasInvalidLengthPropertiesAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            var invalidDelegatedAccess = CreateRandomDelegatedAccess(dateTimeOffset: randomDateTimeOffset);
            var username = GetRandomStringWithLengthOf(256);
            invalidDelegatedAccess.RequesterFirstName = GetRandomStringWithLengthOf(256);
            invalidDelegatedAccess.RequesterLastName = GetRandomStringWithLengthOf(256);
            invalidDelegatedAccess.RequesterEmail = GetRandomStringWithLengthOf(321);
            invalidDelegatedAccess.RecipientFirstName = GetRandomStringWithLengthOf(256);
            invalidDelegatedAccess.RecipientLastName = GetRandomStringWithLengthOf(256);
            invalidDelegatedAccess.RecipientEmail = GetRandomStringWithLengthOf(321);
            invalidDelegatedAccess.IdentifierColumn = GetRandomStringWithLengthOf(11);
            invalidDelegatedAccess.CreatedBy = username;
            invalidDelegatedAccess.UpdatedBy = username;

            var invalidDelegatedAccessException =
                new InvalidDelegatedAccessException(
                    message: "Invalid delegated access. Please correct the errors and try again.");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.RequesterFirstName),
                values: $"Text exceed max length of {invalidDelegatedAccess.RequesterFirstName.Length - 1} characters");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.RequesterLastName),
                values: $"Text exceed max length of {invalidDelegatedAccess.RequesterLastName.Length - 1} characters");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.RequesterEmail),
                values: $"Text exceed max length of {invalidDelegatedAccess.RequesterEmail.Length - 1} characters");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.RecipientFirstName),
                values: $"Text exceed max length of {invalidDelegatedAccess.RecipientFirstName.Length - 1} characters");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.RecipientLastName),
                values: $"Text exceed max length of {invalidDelegatedAccess.RecipientLastName.Length - 1} characters");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.RecipientEmail),
                values: $"Text exceed max length of {invalidDelegatedAccess.RecipientEmail.Length - 1} characters");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.IdentifierColumn),
                values: $"Text exceed max length of {invalidDelegatedAccess.IdentifierColumn.Length - 1} characters");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.CreatedBy),
                values: $"Text exceed max length of {invalidDelegatedAccess.CreatedBy.Length - 1} characters");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.UpdatedBy),
                values: $"Text exceed max length of {invalidDelegatedAccess.UpdatedBy.Length - 1} characters");

            var expectedDelegatedAccessValidationException =
                new DelegatedAccessValidationException(
                    message: "DelegatedAccess validation error occurred, please fix errors and try again.",
                    innerException: invalidDelegatedAccessException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<DelegatedAccess> addDelegatedAccessTask =
                this.delegatedAccessService.AddDelegatedAccessAsync(invalidDelegatedAccess);

            DelegatedAccessValidationException actualDelegatedAccessValidationException =
                await Assert.ThrowsAsync<DelegatedAccessValidationException>(
                    addDelegatedAccessTask.AsTask);

            // then
            actualDelegatedAccessValidationException.Should()
                .BeEquivalentTo(expectedDelegatedAccessValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDelegatedAccessValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertDelegatedAccessAsync(It.IsAny<DelegatedAccess>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfAuditPropertiesIsNotTheSameAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTime;
            DelegatedAccess randomDelegatedAccess = CreateRandomDelegatedAccess(now);
            DelegatedAccess invalidDelegatedAccess = randomDelegatedAccess;
            invalidDelegatedAccess.CreatedBy = GetRandomString();
            invalidDelegatedAccess.UpdatedBy = GetRandomString();
            invalidDelegatedAccess.CreatedDate = now;
            invalidDelegatedAccess.UpdatedDate = GetRandomDateTimeOffset();

            var invalidDelegatedAccessException = new InvalidDelegatedAccessException(
                message: "Invalid delegated access. Please correct the errors and try again.");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.UpdatedBy),
                values: $"Text is not the same as {nameof(DelegatedAccess.CreatedBy)}");

            invalidDelegatedAccessException.AddData(
                key: nameof(DelegatedAccess.UpdatedDate),
                values: $"Date is not the same as {nameof(DelegatedAccess.CreatedDate)}");

            var expectedDelegatedAccessValidationException =
                new DelegatedAccessValidationException(
                    message: "DelegatedAccess validation error occurred, please fix errors and try again.",
                    innerException: invalidDelegatedAccessException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            // when
            ValueTask<DelegatedAccess> addDelegatedAccessTask =
                this.delegatedAccessService.AddDelegatedAccessAsync(invalidDelegatedAccess);

            DelegatedAccessValidationException actualDelegatedAccessValidationException =
                await Assert.ThrowsAsync<DelegatedAccessValidationException>(
                    addDelegatedAccessTask.AsTask);

            // then
            actualDelegatedAccessValidationException.Should().BeEquivalentTo(
                expectedDelegatedAccessValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedDelegatedAccessValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertDelegatedAccessAsync(It.IsAny<DelegatedAccess>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-61)]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
            int invalidSeconds)
        {
            // given
            DateTimeOffset randomDateTime =
                GetRandomDateTimeOffset();

            DateTimeOffset now = randomDateTime;
            DateTimeOffset startDate = now.AddSeconds(-60);
            DateTimeOffset endDate = now.AddSeconds(0);
            DelegatedAccess randomDelegatedAccess = CreateRandomDelegatedAccess();
            DelegatedAccess invalidDelegatedAccess = randomDelegatedAccess;

            DateTimeOffset invalidDate =
                now.AddSeconds(invalidSeconds);

            invalidDelegatedAccess.CreatedDate = invalidDate;
            invalidDelegatedAccess.UpdatedDate = invalidDate;

            var invalidDelegatedAccessException = new InvalidDelegatedAccessException(
                message: "Invalid delegated access. Please correct the errors and try again.");

            invalidDelegatedAccessException.AddData(
            key: nameof(DelegatedAccess.CreatedDate),
                values:
                    $"Date is not recent. Expected a value between " +
                    $"{startDate} and {endDate} but found {invalidDate}");

            var expectedDelegatedAccessValidationException =
                new DelegatedAccessValidationException(
                    message: "DelegatedAccess validation error occurred, please fix errors and try again.",
                    innerException: invalidDelegatedAccessException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            // when
            ValueTask<DelegatedAccess> addDelegatedAccessTask =
                this.delegatedAccessService.AddDelegatedAccessAsync(invalidDelegatedAccess);

            DelegatedAccessValidationException actualDelegatedAccessValidationException =
                await Assert.ThrowsAsync<DelegatedAccessValidationException>(
                    addDelegatedAccessTask.AsTask);

            // then
            actualDelegatedAccessValidationException.Should().BeEquivalentTo(
                expectedDelegatedAccessValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedDelegatedAccessValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertDelegatedAccessAsync(It.IsAny<DelegatedAccess>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }
    }
}