// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnImpersonationContextApprovalIfIdIsInvalidAndLogItAsync()
        {
            // given
            var invalidImpersonationContextId = Guid.Empty;

            var invalidIdentificationCoordinationException =
                new InvalidIdentificationCoordinationException(
                    message: "Invalid identification coordination exception. " +
                        "Please correct the errors and try again.");

            invalidIdentificationCoordinationException.AddData(
                key: nameof(ImpersonationContext.Id),
                values: "Id is invalid");

            var expectedIdentificationCoordinationValidationException =
                new IdentificationCoordinationValidationException(
                    message: "Identification coordination validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: invalidIdentificationCoordinationException);

            // when
            ValueTask impersonationContextApprovalTask = this.identificationCoordinationService
                .ImpersonationContextApprovalAsync(invalidImpersonationContextId, true);

            IdentificationCoordinationValidationException actualIdentificationCoordinationValidationException =
                await Assert.ThrowsAsync<IdentificationCoordinationValidationException>(
                    testCode: impersonationContextApprovalTask.AsTask);

            // then
            actualIdentificationCoordinationValidationException.Should()
                .BeEquivalentTo(expectedIdentificationCoordinationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedIdentificationCoordinationValidationException))),
                        Times.Once);

            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
