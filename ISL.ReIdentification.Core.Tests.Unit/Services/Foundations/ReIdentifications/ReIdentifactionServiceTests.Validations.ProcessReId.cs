// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.ReIdentifications
{
    public partial class ReIdentificationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnProcessIdentificationRequestAsync()
        {
            // given
            IdentificationRequest nullIdentificationRequest = null;

            var nullIdentificationRequestException =
                new NullIdentificationRequestException(message: "Identification request is null.");

            var expectedIdentificationRequestValidationException =
                new IdentificationRequestValidationException(
                    message: "Re-identification validation error occurred, please fix errors and try again.",
                    innerException: nullIdentificationRequestException);

            // when
            ValueTask<IdentificationRequest> processdentificationRequestTask =
                this.reIdentificationService.ProcessReIdentificationRequest(nullIdentificationRequest);

            IdentificationRequestValidationException actualIdentificationRequestValidationException =
                await Assert.ThrowsAsync<IdentificationRequestValidationException>(
                    testCode: processdentificationRequestTask.AsTask);

            // then
            actualIdentificationRequestValidationException.Should()
                .BeEquivalentTo(expectedIdentificationRequestValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedIdentificationRequestValidationException))),
                    Times.Once());

            this.reIdentificationBrokerMock.Verify(broker =>
                broker.ReIdentifyAsync(It.IsAny<ReIdentificationRequest>()),
                    Times.Never);

            this.reIdentificationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnProcessIfIdentificationRequestIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            var invalidIdentificationRequest = new IdentificationRequest
            {
                GivenName = invalidText,
                Surname = invalidText,
                DisplayName = invalidText,
                JobTitle = invalidText,
                Email = invalidText,
                Organisation = invalidText,
                Reason = invalidText,
                IdentificationItems = new List<IdentificationItem>()
            };

            var invalidIdentificationRequestException =
                new InvalidIdentificationRequestException(
                    message: "Invalid identification request. Please correct the errors and try again.");

            invalidIdentificationRequestException.AddData(
                key: nameof(IdentificationRequest.EntraUserId),
                values: "Id is invalid");

            invalidIdentificationRequestException.AddData(
                key: nameof(IdentificationRequest.Email),
                values: "Text is invalid");

            invalidIdentificationRequestException.AddData(
                key: nameof(IdentificationRequest.Organisation),
                values: "Text is invalid");

            invalidIdentificationRequestException.AddData(
                key: nameof(IdentificationRequest.Reason),
                values: "Text is invalid");

            invalidIdentificationRequestException.AddData(
                key: nameof(IdentificationRequest.IdentificationItems),
                values: "IdentificationItems is invalid");

            var expectedIdentificationRequestValidationException =
                new IdentificationRequestValidationException(
                    message: "Re-identification validation error occurred, please fix errors and try again.",
                    innerException: invalidIdentificationRequestException);

            // when
            ValueTask<IdentificationRequest> addIdentificationRequestTask =
                this.reIdentificationService.ProcessReIdentificationRequest(invalidIdentificationRequest);

            IdentificationRequestValidationException actualIdentificationRequestValidationException =
                await Assert.ThrowsAsync<IdentificationRequestValidationException>(
                    testCode: addIdentificationRequestTask.AsTask);

            // then
            actualIdentificationRequestValidationException.Should()
                .BeEquivalentTo(expectedIdentificationRequestValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedIdentificationRequestValidationException))),
                        Times.Once);

            this.reIdentificationBrokerMock.Verify(broker =>
                broker.ReIdentifyAsync(It.IsAny<ReIdentificationRequest>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnProcessIfDuplicateRowNumbersFoundAndLogItAsync()
        {
            // Given
            int randomCount = GetRandomNumber();
            string duplicateRowNumber = GetRandomString();
            IdentificationRequest randomIdentificationRequest = CreateRandomIdentificationRequest(count: randomCount);
            randomIdentificationRequest.IdentificationItems.ForEach(item => item.RowNumber = duplicateRowNumber);
            IdentificationRequest invalidIdentificationRequest = randomIdentificationRequest;

            var invalidIdentificationRequestException =
                new InvalidIdentificationRequestException(
                    message: "Invalid identification request. Please correct the errors and try again.");

            invalidIdentificationRequestException.AddData(
                key: nameof(IdentificationRequest.IdentificationItems),
                values: "IdentificationItems.RowNumber is invalid.  There are duplicate RowNumbers.");

            var expectedIdentificationRequestValidationException =
                new IdentificationRequestValidationException(
                    message: "Re-identification validation error occurred, please fix errors and try again.",
                    innerException: invalidIdentificationRequestException);

            // when
            ValueTask<IdentificationRequest> addIdentificationRequestTask =
                this.reIdentificationService.ProcessReIdentificationRequest(invalidIdentificationRequest);

            IdentificationRequestValidationException actualIdentificationRequestValidationException =
                await Assert.ThrowsAsync<IdentificationRequestValidationException>(
                    testCode: addIdentificationRequestTask.AsTask);

            // then
            actualIdentificationRequestValidationException.Should()
                .BeEquivalentTo(expectedIdentificationRequestValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedIdentificationRequestValidationException))),
                        Times.Once);

            this.reIdentificationBrokerMock.Verify(broker =>
                broker.ReIdentifyAsync(It.IsAny<ReIdentificationRequest>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationBrokerMock.VerifyNoOtherCalls();
        }
    }
}
