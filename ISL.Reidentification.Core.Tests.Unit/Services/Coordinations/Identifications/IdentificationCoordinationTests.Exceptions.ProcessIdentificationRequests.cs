// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Securities;
using Moq;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnProcessIdentificationRequestAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            AccessRequest someAccessRequest = CreateRandomAccessRequest();

            EntraUser outputEntraUser = CreateRandomEntraUser();

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                .ReturnsAsync(outputEntraUser);

            this.accessOrchestrationServiceMock.Setup(service =>
                service.ValidateAccessForIdentificationRequestAsync(someAccessRequest))
                    .ThrowsAsync(dependencyValidationException);

            var expectedIdentificationCoordinationDependencyValidationException =
                new IdentificationCoordinationDependencyValidationException(
                    message: "Identification coordination dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            // when
            ValueTask<AccessRequest> accessRequestTask =
                this.identificationCoordinationService.ProcessIdentificationRequestsAsync(someAccessRequest);

            IdentificationCoordinationDependencyValidationException
                actualIdentificationCoordinationDependencyValidationException =
                    await Assert.ThrowsAsync<IdentificationCoordinationDependencyValidationException>(
                        testCode: accessRequestTask.AsTask);

            // then
            actualIdentificationCoordinationDependencyValidationException
                .Should().BeEquivalentTo(expectedIdentificationCoordinationDependencyValidationException);

            this.accessOrchestrationServiceMock.Verify(service =>
                service.ValidateAccessForIdentificationRequestAsync(someAccessRequest),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationCoordinationDependencyValidationException))),
                       Times.Once);

            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnProcessIdentificationRequestAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            AccessRequest someAccessRequest = CreateRandomAccessRequest();

            EntraUser outputEntraUser = CreateRandomEntraUser();

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                .ReturnsAsync(outputEntraUser);

            this.accessOrchestrationServiceMock.Setup(service =>
                service.ValidateAccessForIdentificationRequestAsync(someAccessRequest))
                    .ThrowsAsync(dependencyException);

            var expectedIdentificationCoordinationDependencyException =
                new IdentificationCoordinationDependencyException(
                    message: "Identification coordination dependency error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyException.InnerException as Xeption);

            // when
            ValueTask<AccessRequest> accessRequestTask =
                this.identificationCoordinationService.ProcessIdentificationRequestsAsync(someAccessRequest);

            IdentificationCoordinationDependencyException
                actualIdentificationCoordinationDependencyException =
                    await Assert.ThrowsAsync<IdentificationCoordinationDependencyException>(
                        testCode: accessRequestTask.AsTask);

            // then
            actualIdentificationCoordinationDependencyException
                .Should().BeEquivalentTo(expectedIdentificationCoordinationDependencyException);

            this.accessOrchestrationServiceMock.Verify(service =>
                service.ValidateAccessForIdentificationRequestAsync(someAccessRequest),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedIdentificationCoordinationDependencyException))),
                       Times.Once);

            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnProcessIdentificationRequestAndLogItAsync()
        {
            // given
            AccessRequest someAccessRequest = CreateRandomAccessRequest();
            Exception someException = new Exception();

            EntraUser outputEntraUser = CreateRandomEntraUser();

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                .ReturnsAsync(outputEntraUser);

            this.accessOrchestrationServiceMock.Setup(service =>
                service.ValidateAccessForIdentificationRequestAsync(someAccessRequest))
                    .ThrowsAsync(someException);

            var expectedIdentificationCoordinationServiceException =
                new IdentificationCoordinationServiceException(
                    message: "Identification coordination service error occurred, " +
                        "fix the errors and try again.",
                    innerException: someException);

            // when
            ValueTask<AccessRequest> accessRequestTask =
                this.identificationCoordinationService.ProcessIdentificationRequestsAsync(someAccessRequest);

            IdentificationCoordinationServiceException
                actualIdentificationCoordinationServiceException =
                    await Assert.ThrowsAsync<IdentificationCoordinationServiceException>(
                        testCode: accessRequestTask.AsTask);

            // then
            actualIdentificationCoordinationServiceException
                .Should().BeEquivalentTo(expectedIdentificationCoordinationServiceException);

            this.accessOrchestrationServiceMock.Verify(service =>
                service.ValidateAccessForIdentificationRequestAsync(someAccessRequest),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedIdentificationCoordinationServiceException))),
                    Times.Once);

            this.accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.identificationOrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.persistanceOrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
