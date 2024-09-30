﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Brokers.NECS.Requests;
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
                this.reIdentificationService.ProcessReidentificationRequest(nullIdentificationRequest);

            IdentificationRequestValidationException actualIdentificationRequestValidationException =
                await Assert.ThrowsAsync<IdentificationRequestValidationException>(
                    processdentificationRequestTask.AsTask);

            // then
            actualIdentificationRequestValidationException.Should()
                .BeEquivalentTo(expectedIdentificationRequestValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedIdentificationRequestValidationException))),
                    Times.Once());

            this.necsBrokerMock.Verify(broker =>
                broker.ReIdAsync(It.IsAny<NecsReidentificationRequest>()),
                    Times.Never);

            this.necsBrokerMock.VerifyNoOtherCalls();
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
                UserIdentifier = invalidText,
                Purpose = invalidText,
                Organisation = invalidText,
                Reason = invalidText,
                IdentificationItems = new List<IdentificationItem>()
            };

            var invalidIdentificationRequestException =
                new InvalidIdentificationRequestException(
                    message: "Invalid identification request. Please correct the errors and try again.");

            invalidIdentificationRequestException.AddData(
                key: nameof(IdentificationRequest.UserIdentifier),
                values: "Text is invalid");

            invalidIdentificationRequestException.AddData(
                key: nameof(IdentificationRequest.Purpose),
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
                this.reIdentificationService.ProcessReidentificationRequest(invalidIdentificationRequest);

            IdentificationRequestValidationException actualIdentificationRequestValidationException =
                await Assert.ThrowsAsync<IdentificationRequestValidationException>(addIdentificationRequestTask.AsTask);

            // then
            actualIdentificationRequestValidationException.Should()
                .BeEquivalentTo(expectedIdentificationRequestValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedIdentificationRequestValidationException))),
                        Times.Once);

            this.necsBrokerMock.Verify(broker =>
                broker.ReIdAsync(It.IsAny<NecsReidentificationRequest>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.necsBrokerMock.VerifyNoOtherCalls();
        }
    }
}
