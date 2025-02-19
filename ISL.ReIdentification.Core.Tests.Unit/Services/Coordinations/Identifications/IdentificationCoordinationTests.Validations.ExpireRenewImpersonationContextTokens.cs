// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnExpireRenewTokensIfIdIsInvalidAndLogItAsync()
        {
            // given
            var invalidImpersonationContextId = Guid.Empty;

            var invalidIdentificationCoordinationException =
                new InvalidIdentificationCoordinationException(
                    message: "Invalid identification coordination exception. Please correct the errors and try again.");

            invalidIdentificationCoordinationException.AddData(
                key: nameof(ImpersonationContext.Id),
                values: "Id is invalid");

            var expectedIdentificationCoordinationValidationException =
                new IdentificationCoordinationValidationException(
                    message: "Identification coordination validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: invalidIdentificationCoordinationException);

            // when
            ValueTask<AccessRequest> expireRenewTokensTask = this.identificationCoordinationService
                .ExpireRenewImpersonationContextTokensAsync(invalidImpersonationContextId);

            IdentificationCoordinationValidationException actualIdentificationCoordinationValidationException =
                await Assert.ThrowsAsync<IdentificationCoordinationValidationException>(
                    testCode: expireRenewTokensTask.AsTask);

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

        [Fact]
        public async Task ShouldThrowValidationExceptionOnExpireRenewTokensIfImpersonationContextNotApprovedLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomFutureDateTimeOffset();
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            randomAccessRequest.IdentificationRequest = null;
            randomAccessRequest.CsvIdentificationRequest = null;
            Guid inputImpersonationContextId = randomAccessRequest.ImpersonationContext.Id;
            AccessRequest retrievedAccessRequest = randomAccessRequest.DeepClone();
            retrievedAccessRequest.ImpersonationContext.IsApproved = false;
            AccessRequest approvedAccessRequest = retrievedAccessRequest.DeepClone();

            var invalidAccessIdentificationCoordinationException =
                new InvalidAccessIdentificationCoordinationException(message: "Project not approved. " +
                    "Please contact responsible person to approve this request.");

            var expectedIdentificationCoordinationValidationException =
                new IdentificationCoordinationValidationException(
                    message: "Identification coordination validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: invalidAccessIdentificationCoordinationException);

            this.persistanceOrchestrationServiceMock.Setup(service =>
                service.RetrieveImpersonationContextByIdAsync(inputImpersonationContextId))
                    .ReturnsAsync(retrievedAccessRequest);

            // when
            ValueTask<AccessRequest> expireRenewTokensTask = this.identificationCoordinationService
                .ExpireRenewImpersonationContextTokensAsync(inputImpersonationContextId);

            IdentificationCoordinationValidationException actualIdentificationCoordinationValidationException =
                await Assert.ThrowsAsync<IdentificationCoordinationValidationException>(
                    testCode: expireRenewTokensTask.AsTask);

            // then
            actualIdentificationCoordinationValidationException.Should()
                .BeEquivalentTo(expectedIdentificationCoordinationValidationException);

            this.persistanceOrchestrationServiceMock.Verify(service =>
                service.RetrieveImpersonationContextByIdAsync(inputImpersonationContextId),
                    Times.Once);

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
