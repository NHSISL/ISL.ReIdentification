// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Identifications.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Identifications
{
    public partial class IdentificationOrchestrationTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnProcessIdentificationRequestWhenNullAndLogItAsync()
        {
            // given
            int itemCount = GetRandomNumber();

            IdentificationRequest nullIdentificationRequest = null;

            var nullIdentificationRequestException =
                new NullIdentificationRequestException(message: "Identification request is null.");


            var expectedIdentificationOrchestrationValidationException =
                new IdentificationOrchestrationValidationException(
                    message: "Identification orchestration validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: nullIdentificationRequestException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(nullIdentificationRequestException);

            // when
            ValueTask<IdentificationRequest> identificationRequestTask =
                this.identificationOrchestrationService
                    .ProcessIdentificationRequestAsync(nullIdentificationRequest);

            IdentificationOrchestrationValidationException
                actualIdentificationOrchestrationValidationException =
                    await Assert.ThrowsAsync<IdentificationOrchestrationValidationException>(
                        testCode: identificationRequestTask.AsTask);

            // then
            actualIdentificationOrchestrationValidationException
                .Should().BeEquivalentTo(expectedIdentificationOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationOrchestrationValidationException))),
                       Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.reIdentificationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
