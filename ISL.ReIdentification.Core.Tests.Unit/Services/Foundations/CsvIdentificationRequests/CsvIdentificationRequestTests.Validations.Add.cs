// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Foundations.CsvIdentificationRequests;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.CsvIdentificationRequests
{
    public partial class CsvIdentificationRequestsTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddCsvIdentificationRequestAsync()
        {
            // given
            CsvIdentificationRequest nullCsvIdentificationRequest = null;
            var nullCsvIdentificationRequestException = new NullCsvIdentificationRequestException(message: "CSV Identification request is null.");

            var expectedCsvIdentificationRequestValidationException =
                new CsvIdentificationRequestValidationException(
                    message: "CSV Identification request validation error occurred, please fix errors and try again.",
                    innerException: nullCsvIdentificationRequestException);

            // when
            ValueTask<CsvIdentificationRequest> addCsvIdentificationRequestTask = this.csvIdentificationRequestService.AddCsvIdentificationRequestAsync(nullCsvIdentificationRequest);

            CsvIdentificationRequestValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<CsvIdentificationRequestValidationException>(testCode: addCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should().BeEquivalentTo(expectedCsvIdentificationRequestValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedCsvIdentificationRequestValidationException))), Times.Once());

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertCsvIdentificationRequestAsync(It.IsAny<CsvIdentificationRequest>()),
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
        public async Task ShouldThrowValidationExceptionOnAddIfCsvIdentificationRequestIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            EntraUser randomEntraUser = CreateRandomEntraUser();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset startDate = randomDateTimeOffset.AddSeconds(-90);
            DateTimeOffset endDate = randomDateTimeOffset.AddSeconds(0);

            var invalidCsvIdentificationRequest = new CsvIdentificationRequest
            {
                RequesterEntraUserId = invalidText,
                RequesterEmail = invalidText,
                RecipientEntraUserId = invalidText,
                RecipientEmail = invalidText,
                CreatedBy = invalidText,
                UpdatedBy = invalidText,
            };

            var csvIdentificationRequestServiceMock = new Mock<CsvIdentificationRequestService>(
                reIdentificationStorageBroker.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            csvIdentificationRequestServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidCsvIdentificationRequest))
                    .ReturnsAsync(invalidCsvIdentificationRequest);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidCsvIdentificationRequestException =
                new InvalidCsvIdentificationRequestException(
                    message: "Invalid CSV identification request. Please correct the errors and try again.");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.Id),
                values: "Id is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.RequesterEntraUserId),
                values: "Text is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.RequesterEmail),
                values: "Text is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.RecipientEntraUserId),
              values: "Text is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.RecipientEmail),
                values: "Text is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.Filepath),
                values: "Text is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.CreatedDate),
                values:
                [
                    "Date is invalid",
                    $"Date is not recent. Expected a value between " +
                    $"{startDate} and {endDate} but found {invalidCsvIdentificationRequest.CreatedDate}"
                ]);

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.CreatedBy),
                values:
                [
                    "Text is invalid",
                    $"Expected value to be '{randomEntraUser.EntraUserId}' but found '{invalidCsvIdentificationRequest.CreatedBy}'."
                ]);

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.UpdatedDate),
                values: "Date is invalid");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.UpdatedBy),
                values: "Text is invalid");

            var expectedCsvIdentificationRequestValidationException =
                new CsvIdentificationRequestValidationException(
                    message: "CSV Identification request validation error occurred, please fix errors and try again.",
                    innerException: invalidCsvIdentificationRequestException);

            // when
            ValueTask<CsvIdentificationRequest> addCsvIdentificationRequestTask =
                csvIdentificationRequestServiceMock.Object.AddCsvIdentificationRequestAsync(invalidCsvIdentificationRequest);

            CsvIdentificationRequestValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<CsvIdentificationRequestValidationException>(testCode: addCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should()
                .BeEquivalentTo(expectedCsvIdentificationRequestValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedCsvIdentificationRequestValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertCsvIdentificationRequestAsync(It.IsAny<CsvIdentificationRequest>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCsvIdentificationRequestHasInvalidLengthProperty()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            CsvIdentificationRequest invalidCsvIdentificationRequest = CreateRandomCsvIdentificationRequest(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            var inputCreatedByUpdatedByString = GetRandomStringWithLengthOf(256);
            invalidCsvIdentificationRequest.RequesterEmail = GetRandomStringWithLengthOf(321);
            invalidCsvIdentificationRequest.RecipientEmail = GetRandomStringWithLengthOf(321);
            invalidCsvIdentificationRequest.CreatedBy = inputCreatedByUpdatedByString;
            invalidCsvIdentificationRequest.UpdatedBy = inputCreatedByUpdatedByString;

            var csvIdentificationRequestServiceMock = new Mock<CsvIdentificationRequestService>(
                reIdentificationStorageBroker.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            csvIdentificationRequestServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidCsvIdentificationRequest))
                    .ReturnsAsync(invalidCsvIdentificationRequest);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidCsvIdentificationRequestException = new InvalidCsvIdentificationRequestException(
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
                key: nameof(CsvIdentificationRequest.CreatedBy),
                values:
                [
                    $"Text exceed max length of {invalidCsvIdentificationRequest.CreatedBy.Length - 1} characters",
                    $"Expected value to be '{randomEntraUser.EntraUserId}' " +
                    $"but found '{invalidCsvIdentificationRequest.CreatedBy}'."
                ]);

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.UpdatedBy),
                values: $"Text exceed max length of {invalidCsvIdentificationRequest.UpdatedBy.Length - 1} characters");

            var expectedCsvIdentificationRequestValidationException =
                new CsvIdentificationRequestValidationException(
                    message: "CSV Identification request validation error occurred, please fix errors and try again.",
                    innerException: invalidCsvIdentificationRequestException);

            // when
            ValueTask<CsvIdentificationRequest> addCsvIdentificationRequestTask =
                csvIdentificationRequestServiceMock.Object.AddCsvIdentificationRequestAsync(
                    invalidCsvIdentificationRequest);

            CsvIdentificationRequestValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<CsvIdentificationRequestValidationException>(
                    testCode: addCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should()
                .BeEquivalentTo(expectedCsvIdentificationRequestValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedCsvIdentificationRequestValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertCsvIdentificationRequestAsync(It.IsAny<CsvIdentificationRequest>()),
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

            CsvIdentificationRequest randomCsvIdentificationRequest = CreateRandomCsvIdentificationRequest(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            CsvIdentificationRequest invalidCsvIdentificationRequest = randomCsvIdentificationRequest;
            invalidCsvIdentificationRequest.CreatedBy = GetRandomString();
            invalidCsvIdentificationRequest.UpdatedBy = GetRandomString();
            invalidCsvIdentificationRequest.CreatedDate = GetRandomDateTimeOffset();
            invalidCsvIdentificationRequest.UpdatedDate = GetRandomDateTimeOffset();

            var csvIdentificationRequestServiceMock = new Mock<CsvIdentificationRequestService>(
                reIdentificationStorageBroker.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            csvIdentificationRequestServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidCsvIdentificationRequest))
                    .ReturnsAsync(invalidCsvIdentificationRequest);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidCsvIdentificationRequestException = new InvalidCsvIdentificationRequestException(
                message: "Invalid CSV identification request. Please correct the errors and try again.");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.CreatedBy),
                values:
                    $"Expected value to be '{randomEntraUser.EntraUserId}' " +
                    $"but found '{invalidCsvIdentificationRequest.CreatedBy}'.");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.UpdatedBy),
                values: $"Text is not the same as {nameof(CsvIdentificationRequest.CreatedBy)}");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.UpdatedDate),
                values: $"Date is not the same as {nameof(CsvIdentificationRequest.CreatedDate)}");

            invalidCsvIdentificationRequestException.AddData(
                key: nameof(CsvIdentificationRequest.CreatedDate),
                values:
                    $"Date is not recent." +
                    $" Expected a value between {startDate} and {endDate} but found " +
                    $"{invalidCsvIdentificationRequest.CreatedDate}");

            var expectedCsvIdentificationRequestValidationException =
                new CsvIdentificationRequestValidationException(
                    message: "CSV Identification request validation error occurred, please fix errors and try again.",
                    innerException: invalidCsvIdentificationRequestException);

            // when
            ValueTask<CsvIdentificationRequest> addCsvIdentificationRequestTask =
                csvIdentificationRequestServiceMock.Object.AddCsvIdentificationRequestAsync(
                    invalidCsvIdentificationRequest);

            CsvIdentificationRequestValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<CsvIdentificationRequestValidationException>(
                    testCode: addCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should().BeEquivalentTo(
                expectedCsvIdentificationRequestValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedCsvIdentificationRequestValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertCsvIdentificationRequestAsync(It.IsAny<CsvIdentificationRequest>()),
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

            CsvIdentificationRequest randomCsvIdentificationRequest = CreateRandomCsvIdentificationRequest(
                dateTimeOffset: randomDateTimeOffset,
                userId: randomEntraUser.EntraUserId);

            CsvIdentificationRequest invalidCsvIdentificationRequest = randomCsvIdentificationRequest;

            DateTimeOffset invalidDate =
                randomDateTimeOffset.AddSeconds(invalidSeconds);

            invalidCsvIdentificationRequest.CreatedDate = invalidDate;
            invalidCsvIdentificationRequest.UpdatedDate = invalidDate;

            var csvIdentificationRequestServiceMock = new Mock<CsvIdentificationRequestService>(
                reIdentificationStorageBroker.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object)
            {
                CallBase = true
            };

            csvIdentificationRequestServiceMock.Setup(service =>
                service.ApplyAddAuditAsync(invalidCsvIdentificationRequest))
                    .ReturnsAsync(invalidCsvIdentificationRequest);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            var invalidCsvIdentificationRequestException = new InvalidCsvIdentificationRequestException(
                message: "Invalid CSV identification request. Please correct the errors and try again.");

            invalidCsvIdentificationRequestException.AddData(
            key: nameof(CsvIdentificationRequest.CreatedDate),
                values:
                    $"Date is not recent. Expected a value between " +
                    $"{startDate} and {endDate} but found {invalidDate}");

            var expectedCsvIdentificationRequestValidationException =
                new CsvIdentificationRequestValidationException(
                    message: "CSV Identification request validation error occurred, please fix errors and try again.",
                    innerException: invalidCsvIdentificationRequestException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<CsvIdentificationRequest> addCsvIdentificationRequestTask =
                csvIdentificationRequestServiceMock.Object.AddCsvIdentificationRequestAsync(
                    invalidCsvIdentificationRequest);

            CsvIdentificationRequestValidationException actualCsvIdentificationRequestValidationException =
                await Assert.ThrowsAsync<CsvIdentificationRequestValidationException>(
                    testCode: addCsvIdentificationRequestTask.AsTask);

            // then
            actualCsvIdentificationRequestValidationException.Should().BeEquivalentTo(
                expectedCsvIdentificationRequestValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedCsvIdentificationRequestValidationException))),
                        Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertCsvIdentificationRequestAsync(It.IsAny<CsvIdentificationRequest>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
        }
    }
}
