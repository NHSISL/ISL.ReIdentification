// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Identifications.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Identifications
{
    public partial class IdentificationOrchestrationTests
    {
        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnExpireRenewImpersonationContextTokensWhenInvalidAndLogItAsync()
        {
            // given
            var invalidAccessRequest = new AccessRequest();
            var inputIsPreviouslyApproved = true;

            var invalidArgumentIdentificationOrchestrationException =
                new InvalidArgumentIdentificationOrchestrationException(
                    message: "Invalid argument identification orchestration exception, " +
                        "please correct the errors and try again.");

            invalidArgumentIdentificationOrchestrationException.AddData(
                key: "ImpersonationContext",
                values: "Object is invalid");

            var expectedIdentificationOrchestrationValidationException =
            new IdentificationOrchestrationValidationException(
                message: "Identification orchestration validation error occurred, fix the errors and try again.",
                innerException: invalidArgumentIdentificationOrchestrationException);

            // when
            ValueTask<AccessRequest> expireRenewImpersonationContextTokenTask = this.identificationOrchestrationService
                    .ExpireRenewImpersonationContextTokensAsync(invalidAccessRequest, inputIsPreviouslyApproved);

            IdentificationOrchestrationValidationException actualImpersonationContextValidationException =
                await Assert.ThrowsAsync<IdentificationOrchestrationValidationException>(
                    testCode: expireRenewImpersonationContextTokenTask.AsTask);

            // then
            actualImpersonationContextValidationException.Should().BeEquivalentTo(
                expectedIdentificationOrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedIdentificationOrchestrationValidationException))),
                    Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.reIdentificationServiceMock.VerifyNoOtherCalls();
            this.documentServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
