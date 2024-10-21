// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Persists.Exceptions;
using Moq;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Persists
{
    public partial class PersistanceOrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnPersistCsvIdentificationRequestAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            AccessRequest someAccessRequest = CreateRandomAccessRequest();

            this.hashBrokerMock.Setup(broker =>
                broker.GenerateSha256Hash(It.IsAny<MemoryStream>()))
                    .Throws(dependencyValidationException);

            var expectedPersistanceOrchestrationDependencyValidationException =
                new PersistanceOrchestrationDependencyValidationException(
                    message: "Persistance orchestration dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            // when
            ValueTask<AccessRequest> persistCsvIdentificationRequestAsyncTask =
                this.persistanceOrchestrationService.PersistCsvIdentificationRequestAsync(
                    accessRequest: someAccessRequest);

            PersistanceOrchestrationDependencyValidationException
                actualPersistanceOrchestrationDependencyValidationException =
                await Assert.ThrowsAsync<PersistanceOrchestrationDependencyValidationException>(
                    testCode: persistCsvIdentificationRequestAsyncTask.AsTask);

            // then
            actualPersistanceOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedPersistanceOrchestrationDependencyValidationException);

            this.hashBrokerMock.Verify(broker =>
                broker.GenerateSha256Hash(It.IsAny<MemoryStream>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPersistanceOrchestrationDependencyValidationException))),
                       Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyOnPersistCsvIdentificationRequestAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            AccessRequest someAccessRequest = CreateRandomAccessRequest();

            this.hashBrokerMock.Setup(broker =>
                broker.GenerateSha256Hash(It.IsAny<MemoryStream>()))
                    .Throws(dependencyException);

            var expectedPersistanceOrchestrationDependencyException =
                new PersistanceOrchestrationDependencyException(
                    message: "Persistance orchestration dependency error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyException.InnerException as Xeption);

            // when
            ValueTask<AccessRequest> persistCsvIdentificationRequestAsyncTask =
                this.persistanceOrchestrationService.PersistCsvIdentificationRequestAsync(
                    accessRequest: someAccessRequest);

            PersistanceOrchestrationDependencyException
                actualPersistanceOrchestrationDependencyException =
                await Assert.ThrowsAsync<PersistanceOrchestrationDependencyException>(
                    testCode: persistCsvIdentificationRequestAsyncTask.AsTask);

            // then
            actualPersistanceOrchestrationDependencyException
                .Should().BeEquivalentTo(expectedPersistanceOrchestrationDependencyException);

            this.hashBrokerMock.Verify(broker =>
                broker.GenerateSha256Hash(It.IsAny<MemoryStream>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedPersistanceOrchestrationDependencyException))),
                       Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
        }
    }
}
