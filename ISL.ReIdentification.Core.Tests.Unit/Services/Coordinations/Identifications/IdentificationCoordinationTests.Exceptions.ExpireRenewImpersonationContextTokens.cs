// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using Moq;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnExpireRenewTokensAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            Guid someImpersonationContextId = Guid.NewGuid();

            this.persistanceOrchestrationServiceMock.Setup(service =>
                service.RetrieveImpersonationContextByIdAsync(someImpersonationContextId))
                    .ThrowsAsync(dependencyValidationException);

            var expectedIdentificationCoordinationDependencyValidationException =
                new IdentificationCoordinationDependencyValidationException(
                    message: "Identification coordination dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            // when
            ValueTask<AccessRequest> expireRenewTokensTask = this.identificationCoordinationService
                .ExpireRenewImpersonationContextTokensAsync(someImpersonationContextId);

            IdentificationCoordinationDependencyValidationException
                actualIdentificationCoordinationDependencyValidationException =
                    await Assert.ThrowsAsync<IdentificationCoordinationDependencyValidationException>(
                        testCode: expireRenewTokensTask.AsTask);

            // then
            actualIdentificationCoordinationDependencyValidationException
                .Should().BeEquivalentTo(expectedIdentificationCoordinationDependencyValidationException);

            this.persistanceOrchestrationServiceMock.Verify(service =>
                service.RetrieveImpersonationContextByIdAsync(someImpersonationContextId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationCoordinationDependencyValidationException))),
                       Times.Once);

            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnExpireRenewTokensAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            Guid someImpersonationContextId = Guid.NewGuid();

            this.persistanceOrchestrationServiceMock.Setup(service =>
                service.RetrieveImpersonationContextByIdAsync(someImpersonationContextId))
                    .ThrowsAsync(dependencyException);

            var expectedIdentificationCoordinationDependencyException =
                new IdentificationCoordinationDependencyException(
                    message: "Identification coordination dependency error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyException.InnerException as Xeption);

            // when
            ValueTask<AccessRequest> expireRenewTokensTask = this.identificationCoordinationService
                .ExpireRenewImpersonationContextTokensAsync(someImpersonationContextId);

            IdentificationCoordinationDependencyException
                actualIdentificationCoordinationDependencyException =
                    await Assert.ThrowsAsync<IdentificationCoordinationDependencyException>(
                        testCode: expireRenewTokensTask.AsTask);

            // then
            actualIdentificationCoordinationDependencyException
                .Should().BeEquivalentTo(expectedIdentificationCoordinationDependencyException);

            this.persistanceOrchestrationServiceMock.Verify(service =>
                service.RetrieveImpersonationContextByIdAsync(someImpersonationContextId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationCoordinationDependencyException))),
                       Times.Once);

            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
