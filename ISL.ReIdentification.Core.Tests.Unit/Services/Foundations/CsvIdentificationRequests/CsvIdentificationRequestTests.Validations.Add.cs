// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.CsvIdentificationRequests
{
    public partial class CsvIdentificationRequestsTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddCsvIdentificationRequestAsync()
        {
            //given
            CsvIdentificationRequest nullCsvIdentificationRequest = null;

            var nullCsvIdentificationRequestException =
                new NullCsvIdentificationRequestException(message: "CSV identification request is null.");

            var expectedCsvIdentificationRequestValidationException =
                new CsvIdentificationRequestValidationException(
                    message: "CSV identification request validation error occurred, please fix errors and try again.",
                    innerException: nullCsvIdentificationRequestException);

            //when
            ValueTask<CsvIdentificationRequest> addCsvIdentificationRequestTask =
                this.csvIdentificationRequestService.AddCsvIdentificationRequestAsync(nullCsvIdentificationRequest);

            CsvIdentificationRequestValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<CsvIdentificationRequestValidationException>(
                    testCode: addCsvIdentificationRequestTask.AsTask);

            //then
            actualCsvIdentificationRequestValidationException.Should()
                .BeEquivalentTo(expectedCsvIdentificationRequestValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedCsvIdentificationRequestValidationException))), Times.Once());

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertCsvIdentificationRequestAsync(It.IsAny<CsvIdentificationRequest>()),
                    Times.Never);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfCsvIdentificationRequestIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            var invalidCsvIdentificationRequest = new CsvIdentificationRequest
            {
                RequesterEmail = invalidText,
                RecipientEmail = invalidText,
                IdentifierColumn = invalidText
            };

            var invalidCsvIdentificationRequestException =
                new InvalidCsvIdentificationRequestException(
                    message: "Invalid CSV identification request. Please correct the errors and try again.");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.Id),
                values: "Id is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.RequesterEntraUserId),
                values: "Id is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.RequesterEmail),
                values: "Text is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.RecipientEntraUserId),
                values: "Id is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.RecipientEmail),
                values: "Text is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.Filepath),
                values: "Text is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.IdentifierColumn),
                values: "Text is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.CreatedDate),
                values: "Date is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.CreatedBy),
                values: "Text is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.UpdatedDate),
                values: "Date is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.UpdatedBy),
                values: "Text is invalid");

            var expectedCsvIdentificationRequestValidationException =
                new CsvIdentificationRequestValidationException(
                    message: "CSV identification request validation error occurred, please fix errors and try again.",
                    innerException: invalidCsvIdentificationRequestException);

            // when
            ValueTask<CsvIdentificationRequest> addCsvIdentificationRequestTask =
                this.csvIdentificationRequestService.AddCsvIdentificationRequestAsync(invalidCsvIdentificationRequest);

            CsvIdentificationRequestValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<CsvIdentificationRequestValidationException>(
                    testCode: addCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should()
                .BeEquivalentTo(expectedCsvIdentificationRequestValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedCsvIdentificationRequestValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertCsvIdentificationRequestAsync(It.IsAny<CsvIdentificationRequest>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCsvIdentificationRequestHasInvalidLengthPropertiesAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            var invalidCsvIdentificationRequest = CreateRandomCsvIdentificationRequest(
                dateTimeOffset: randomDateTimeOffset);

            var username = GetRandomStringWithLengthOf(256);
            invalidCsvIdentificationRequest.RequesterEmail = GetRandomStringWithLengthOf(321);
            invalidCsvIdentificationRequest.RecipientEmail = GetRandomStringWithLengthOf(321);
            invalidCsvIdentificationRequest.IdentifierColumn = GetRandomStringWithLengthOf(11);
            invalidCsvIdentificationRequest.CreatedBy = username;
            invalidCsvIdentificationRequest.UpdatedBy = username;

            var invalidCsvIdentificationRequestException =
                new InvalidCsvIdentificationRequestException(
                    message: "Invalid CSV identification request. Please correct the errors and try again.");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.RequesterEmail),
                values: $"Text exceed max length of " +
                    $"{invalidCsvIdentificationRequest.RequesterEmail.Length - 1} characters");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.RecipientEmail),
                values: $"Text exceed max length of " +
                    $"{invalidCsvIdentificationRequest.RecipientEmail.Length - 1} characters");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.IdentifierColumn),
                values: $"Text exceed max length of " +
                    $"{invalidCsvIdentificationRequest.IdentifierColumn.Length - 1} characters");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.CreatedBy),
                values: $"Text exceed max length of " +
                    $"{invalidCsvIdentificationRequest.CreatedBy.Length - 1} characters");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.UpdatedBy),
                values: $"Text exceed max length of " +
                    $"{invalidCsvIdentificationRequest.UpdatedBy.Length - 1} characters");

            var expectedCsvIdentificationRequestValidationException =
                new CsvIdentificationRequestValidationException(
                    message: "CSV identification request validation error occurred, please fix errors and try again.",
                    innerException: invalidCsvIdentificationRequestException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<CsvIdentificationRequest> addCsvIdentificationRequestTask =
                this.csvIdentificationRequestService.AddCsvIdentificationRequestAsync(invalidCsvIdentificationRequest);

            CsvIdentificationRequestValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<CsvIdentificationRequestValidationException>(
                    testCode: addCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should()
                .BeEquivalentTo(expectedCsvIdentificationRequestValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedCsvIdentificationRequestValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertCsvIdentificationRequestAsync(It.IsAny<CsvIdentificationRequest>()),
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
            CsvIdentificationRequest randomCsvIdentificationRequest = CreateRandomCsvIdentificationRequest(now);
            CsvIdentificationRequest invalidCsvIdentificationRequest = randomCsvIdentificationRequest;
            invalidCsvIdentificationRequest.CreatedBy = GetRandomString();
            invalidCsvIdentificationRequest.UpdatedBy = GetRandomString();
            invalidCsvIdentificationRequest.CreatedDate = now;
            invalidCsvIdentificationRequest.UpdatedDate = GetRandomDateTimeOffset();

            var invalidCsvIdentificationRequestException = new InvalidCsvIdentificationRequestException(
                message: "Invalid CSV identification request. Please correct the errors and try again.");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.UpdatedBy),
                values: $"Text is not the same as {nameof(CsvIdentificationRequest.CreatedBy)}");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.UpdatedDate),
                values: $"Date is not the same as {nameof(CsvIdentificationRequest.CreatedDate)}");

            var expectedCsvIdentificationRequestValidationException =
                new CsvIdentificationRequestValidationException(
                    message: "CSV identification request validation error occurred, please fix errors and try again.",
                    innerException: invalidCsvIdentificationRequestException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            // when
            ValueTask<CsvIdentificationRequest> addCsvIdentificationRequestTask =
                this.csvIdentificationRequestService.AddCsvIdentificationRequestAsync(invalidCsvIdentificationRequest);

            CsvIdentificationRequestValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<CsvIdentificationRequestValidationException>(
                    testCode: addCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should().BeEquivalentTo(
                expectedCsvIdentificationRequestValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedCsvIdentificationRequestValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertCsvIdentificationRequestAsync(It.IsAny<CsvIdentificationRequest>()),
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
            CsvIdentificationRequest randomCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            CsvIdentificationRequest invalidCsvIdentificationRequest = randomCsvIdentificationRequest;

            DateTimeOffset invalidDate =
                now.AddSeconds(invalidSeconds);

            invalidCsvIdentificationRequest.CreatedDate = invalidDate;
            invalidCsvIdentificationRequest.UpdatedDate = invalidDate;

            var invalidCsvIdentificationRequestException = new InvalidCsvIdentificationRequestException(
                message: "Invalid CSV identification request. Please correct the errors and try again.");

            invalidCsvIdentificationRequestException.AddData(
            key: nameof(CsvIdentificationRequest.CreatedDate),
                values:
                    $"Date is not recent. Expected a value between " +
                    $"{startDate} and {endDate} but found {invalidDate}");

            var expectedCsvIdentificationRequestValidationException =
                new CsvIdentificationRequestValidationException(
                    message: "CSV identification request validation error occurred, please fix errors and try again.",
                    innerException: invalidCsvIdentificationRequestException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            // when
            ValueTask<CsvIdentificationRequest> addCsvIdentificationRequestTask =
                this.csvIdentificationRequestService.AddCsvIdentificationRequestAsync(invalidCsvIdentificationRequest);

            CsvIdentificationRequestValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<CsvIdentificationRequestValidationException>(
                    testCode: addCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should().BeEquivalentTo(
                expectedCsvIdentificationRequestValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedCsvIdentificationRequestValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertCsvIdentificationRequestAsync(It.IsAny<CsvIdentificationRequest>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }
    }
}
