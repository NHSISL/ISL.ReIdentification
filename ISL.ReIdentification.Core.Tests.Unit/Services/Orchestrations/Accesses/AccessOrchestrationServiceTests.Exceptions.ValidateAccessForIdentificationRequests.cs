// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses.Exceptions;
using ISL.ReIdentification.Core.Services.Orchestrations.Accesses;
using Moq;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Accesses
{
    public partial class AccessOrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnValidateAccessForIdentificationRequestAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            var accessOrchestrationServiceMock = new Mock<AccessOrchestrationService>
                (this.userAccessServiceMock.Object,
                this.pdsDataServiceMock.Object,
                this.dateTimeBrokerMock.Object,
                this.loggingBrokerMock.Object)
            { CallBase = true };

            AccessRequest someAccessRequest = CreateRandomAccessRequest();

            this.userAccessServiceMock.Setup(service =>
                 service.RetrieveAllActiveOrganisationsUserHasAccessTo(It.IsAny<Guid>()))
                     .ThrowsAsync(dependencyValidationException);

            var expectedAccessOrchestrationDependencyValidationException =
                new AccessOrchestrationDependencyValidationException(
                    message: "Access orchestration dependency validation error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            AccessOrchestrationService accessOrchestrationService = accessOrchestrationServiceMock.Object;

            // when
            ValueTask<AccessRequest> validateAccessForIdentificationRequestTask =
                accessOrchestrationService
                    .ValidateAccessForIdentificationRequestAsync(someAccessRequest);

            AccessOrchestrationDependencyValidationException
                actualAccessOrchestrationDependencyValidationException =
                await Assert.ThrowsAsync<AccessOrchestrationDependencyValidationException>(
                    testCode: validateAccessForIdentificationRequestTask.AsTask);

            // then
            actualAccessOrchestrationDependencyValidationException
                .Should().BeEquivalentTo(expectedAccessOrchestrationDependencyValidationException);

            this.userAccessServiceMock.Verify(service =>
                service.RetrieveAllActiveOrganisationsUserHasAccessTo(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedAccessOrchestrationDependencyValidationException))),
                       Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.pdsDataServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyOnValidateAccessForIdentificationRequestAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            var accessOrchestrationServiceMock = new Mock<AccessOrchestrationService>
                (this.userAccessServiceMock.Object,
                this.pdsDataServiceMock.Object,
                this.dateTimeBrokerMock.Object,
                this.loggingBrokerMock.Object)
            { CallBase = true };

            AccessRequest someAccessRequest = CreateRandomAccessRequest();

            this.userAccessServiceMock.Setup(service =>
                 service.RetrieveAllActiveOrganisationsUserHasAccessTo(It.IsAny<Guid>()))
                     .ThrowsAsync(dependencyException);

            var expectedAccessOrchestrationDependencyException =
                new AccessOrchestrationDependencyException(
                    message: "Access orchestration dependency error occurred, " +
                        "fix the errors and try again.",
                    innerException: dependencyException.InnerException as Xeption);

            AccessOrchestrationService accessOrchestrationService = accessOrchestrationServiceMock.Object;

            // when
            ValueTask<AccessRequest> validateAccessForIdentificationRequestTask =
                accessOrchestrationService
                    .ValidateAccessForIdentificationRequestAsync(someAccessRequest);

            AccessOrchestrationDependencyException
                actualAccessOrchestrationDependencyException =
                await Assert.ThrowsAsync<AccessOrchestrationDependencyException>(
                    testCode: validateAccessForIdentificationRequestTask.AsTask);

            // then
            actualAccessOrchestrationDependencyException
                .Should().BeEquivalentTo(expectedAccessOrchestrationDependencyException);

            this.userAccessServiceMock.Verify(service =>
                service.RetrieveAllActiveOrganisationsUserHasAccessTo(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedAccessOrchestrationDependencyException))),
                       Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.pdsDataServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowAggregateServiceExceptionOnProcessIfServiceErrorsOccurredAndLogItAsync()
        {
            // given
            var accessOrchestrationServiceMock = new Mock<AccessOrchestrationService>
                (this.userAccessServiceMock.Object,
                this.pdsDataServiceMock.Object,
                this.dateTimeBrokerMock.Object,
                this.loggingBrokerMock.Object)
            { CallBase = true };

            AccessOrchestrationService service = accessOrchestrationServiceMock.Object;

            AccessRequest someAccessRequest = CreateRandomAccessRequest();
            string userOrganisation = GetRandomStringWithLength(5);
            string message = GetRandomStringWithLength(10);

            List<string> userOrganisations =
                new List<string> { userOrganisation };

            var aggregateException =
                new AggregateException(
                    message: message);

            var failedAccessOrchestrationServiceException =
                new FailedServiceAccessOrchestrationException(
                    message: "Failed access aggregate orchestration service error occurred, " +
                        "please contact support.",
                    innerException: aggregateException);

            var expectedAccessOrchestrationServiceException =
                new AccessOrchestrationServiceException(
                    message: "Access orchestration service error occurred, please contact support.",
                    innerException: failedAccessOrchestrationServiceException);

            this.userAccessServiceMock.Setup(service =>
                 service.RetrieveAllActiveOrganisationsUserHasAccessTo(It.IsAny<Guid>()))
                     .ThrowsAsync(aggregateException);

            // when
            ValueTask<AccessRequest> validateAccessForIdentificationRequestTask =
                service.ValidateAccessForIdentificationRequestAsync(someAccessRequest);

            AccessOrchestrationServiceException
                actualAccessOrchestrationServiceException =
                await Assert.ThrowsAsync<AccessOrchestrationServiceException>(
                    testCode: validateAccessForIdentificationRequestTask.AsTask);

            // then
            actualAccessOrchestrationServiceException
                .Should().BeEquivalentTo(expectedAccessOrchestrationServiceException);

            this.userAccessServiceMock.Verify(service =>
                service.RetrieveAllActiveOrganisationsUserHasAccessTo(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   actualAccessOrchestrationServiceException))),
                       Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.pdsDataServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccurredAndLogItAsync()
        {
            var accessOrchestrationServiceMock = new Mock<AccessOrchestrationService>
                (this.userAccessServiceMock.Object,
                this.pdsDataServiceMock.Object,
                this.dateTimeBrokerMock.Object,
                this.loggingBrokerMock.Object)
            { CallBase = true };

            AccessRequest someAccessRequest = CreateRandomAccessRequest();
            var serviceException = new Exception();

            var failedServiceAccessOrchestrationException =
                new FailedServiceAccessOrchestrationException(
                    message: "Failed access orchestration service error occurred, please contact support.",
                    innerException: serviceException);

            var expectedAccessOrchestrationServiceException =
                new AccessOrchestrationServiceException(
                    message: "Access orchestration service error occurred, please contact support.",
                    innerException: failedServiceAccessOrchestrationException);

            this.userAccessServiceMock.Setup(service =>
                 service.RetrieveAllActiveOrganisationsUserHasAccessTo(It.IsAny<Guid>()))
                     .ThrowsAsync(serviceException);

            AccessOrchestrationService accessOrchestrationService = accessOrchestrationServiceMock.Object;

            // when
            ValueTask<AccessRequest> validateAccessForIdentificationRequestTask =
                accessOrchestrationService
                    .ValidateAccessForIdentificationRequestAsync(someAccessRequest);

            AccessOrchestrationServiceException
                actualAccessOrchestrationServiceException =
                await Assert.ThrowsAsync<AccessOrchestrationServiceException>(
                    testCode: validateAccessForIdentificationRequestTask.AsTask);

            // then
            actualAccessOrchestrationServiceException
                .Should().BeEquivalentTo(expectedAccessOrchestrationServiceException);

            this.userAccessServiceMock.Verify(service =>
                service.RetrieveAllActiveOrganisationsUserHasAccessTo(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   expectedAccessOrchestrationServiceException))),
                       Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.pdsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}
