// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Orchestrations.Identifications.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Identifications
{
    public partial class IdentificationOrchestrationTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnRemoveDocumentByFileNameWhenArgumentsInvalidAndLogItAsync(
            string invalidString)
        {
            // given
            var invalidArgumentIdentificationOrchestrationException =
            new InvalidArgumentIdentificationOrchestrationException(
                    message: "Invalid argument identification orchestration exception, " +
                        "please correct the errors and try again.");

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
            ValueTask removeDocumentByFileNamTask =
                this.identificationOrchestrationService
                    .RemoveDocumentByFileNameAsync(invalidString, invalidString);

            IdentificationOrchestrationValidationException
                actualIdentificationOrchestrationValidationException =
                    await Assert.ThrowsAsync<IdentificationOrchestrationValidationException>(
                        testCode: removeDocumentByFileNamTask.AsTask);

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
