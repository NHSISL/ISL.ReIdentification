﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Orchestrations.Identifications.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Identifications
{
    public partial class IdentificationOrchestrationTests
    {
        [Theory]
        [MemberData(nameof(InvalidArgumentsStreamIsNull))]
        public async Task ShouldThrowValidationExceptionOnAddDocumentWhenArgumentsInvalidAndLogItAsync(
            Stream invalidStream, string invalidString)
        {
            // given
            Stream invalidInput = invalidStream;
            string invalidFileName = invalidString;
            string invalidContainer = invalidString;

            var invalidArgumentIdentificationOrchestrationException =
                new InvalidArgumentIdentificationOrchestrationException(
                    message: "Invalid argument identification orchestration exception, " +
                        "please correct the errors and try again.");

            invalidArgumentIdentificationOrchestrationException.AddData(
                key: "input",
                values: "Stream is invalid");

            invalidArgumentIdentificationOrchestrationException.AddData(
                key: "fileName",
                values: "Text is invalid");

            invalidArgumentIdentificationOrchestrationException.AddData(
                key: "container",
                values: "Text is invalid");

            var expectedIdentificationOrchestrationValidationException =
                new IdentificationOrchestrationValidationException(
                    message: "Identification orchestration validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: invalidArgumentIdentificationOrchestrationException);

            // when
            ValueTask addDocumentTask =
                this.identificationOrchestrationService
                    .AddDocumentAsync(invalidInput, invalidFileName, invalidContainer);

            IdentificationOrchestrationValidationException
                actualIdentificationOrchestrationValidationException =
                    await Assert.ThrowsAsync<IdentificationOrchestrationValidationException>(
                        testCode: addDocumentTask.AsTask);

            // then
            actualIdentificationOrchestrationValidationException
                .Should().BeEquivalentTo(expectedIdentificationOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationOrchestrationValidationException))),
                       Times.Once);

            this.documentServiceMock.VerifyNoOtherCalls();
            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.reIdentificationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
