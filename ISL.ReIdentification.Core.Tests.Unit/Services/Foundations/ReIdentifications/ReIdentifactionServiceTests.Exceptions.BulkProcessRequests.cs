// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.ReIdentifications;
using Moq;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.ReIdentifications
{
    public partial class ReIdentificationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowAggregateExceptionForDependencyValidationErrorsOnBulkProcessIfRequestsFailsAsync(
            Xeption dependancyValidationException)
        {
            // Given
            Guid randomIdentifier = Guid.NewGuid();
            int batchSize = 1;
            int randomCount = 1;
            IdentificationRequest randomIdentificationRequest = CreateRandomIdentificationRequest(count: randomCount);
            IdentificationRequest inputIdentificationRequest = randomIdentificationRequest;

            Mock<ReIdentificationService> reIdentificationServiceMock =
                new Mock<ReIdentificationService>(
                    this.necsBrokerMock.Object,
                    this.identifierBrokerMock.Object,
                    this.necsConfiguration,
                    this.loggingBrokerMock.Object)
                { CallBase = true };

            ReIdentificationService service = reIdentificationServiceMock.Object;

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ThrowsAsync(dependancyValidationException);

            var serverException =
                new Exception(message: GetRandomString());

            var failedClientReIdentificationException = new FailedClientReIdentificationException(
                message: "Failed NECS client error occurred, please contact support.",
                innerException: dependancyValidationException,
                data: dependancyValidationException.Data);

            var reIdentificationDependencyValidationException = new ReIdentificationDependencyValidationException(
                message: "Re-identification dependency validation error occurred, fix errors and try again.",
                innerException: failedClientReIdentificationException);

            var expectedAggregateException = new AggregateException(
                message: $"Unable to process addresses in 1 of the batch(es) from {inputIdentificationRequest.Id}",
                innerExceptions: reIdentificationDependencyValidationException);

            // When
            ValueTask<IdentificationRequest> reIdentificationRequestTask = service
                .BulkProcessRequestsAsync(inputIdentificationRequest, batchSize);

            AggregateException actualAggregateException =
                await Assert.ThrowsAsync<AggregateException>(
                    testCode: reIdentificationRequestTask.AsTask);

            // Then
            actualAggregateException.Should().BeEquivalentTo(expectedAggregateException);

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   reIdentificationDependencyValidationException))),
                       Times.Once);

            this.necsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowAggregateExceptionForDependencyExceptionsOnBulkProcessIfRequestsFailsAsync(
            Xeption dependancyValidationException)
        {
            // Given
            Guid randomIdentifier = Guid.NewGuid();
            int batchSize = 1;
            int randomCount = 1;
            IdentificationRequest randomIdentificationRequest = CreateRandomIdentificationRequest(count: randomCount);
            IdentificationRequest inputIdentificationRequest = randomIdentificationRequest;

            Mock<ReIdentificationService> reIdentificationServiceMock =
                new Mock<ReIdentificationService>(
                    this.necsBrokerMock.Object,
                    this.identifierBrokerMock.Object,
                    this.necsConfiguration,
                    this.loggingBrokerMock.Object)
                { CallBase = true };

            ReIdentificationService service = reIdentificationServiceMock.Object;

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ThrowsAsync(dependancyValidationException);

            var serverException =
                new Exception(message: GetRandomString());

            var failedServerReIdentificationException = new FailedServerReIdentificationException(
                message: "Failed NECS server error occurred, please contact support.",
                innerException: dependancyValidationException,
                data: dependancyValidationException.Data);

            var reIdentificationDependencyException = new ReIdentificationDependencyException(
                message: "Re-identification dependency error occurred, contact support.",
                innerException: failedServerReIdentificationException);

            var expectedAggregateException = new AggregateException(
                message: $"Unable to process addresses in 1 of the batch(es) from {inputIdentificationRequest.Id}",
                innerExceptions: reIdentificationDependencyException);

            // When
            ValueTask<IdentificationRequest> reIdentificationRequestTask = service
                .BulkProcessRequestsAsync(inputIdentificationRequest, batchSize);

            AggregateException actualAggregateException =
                await Assert.ThrowsAsync<AggregateException>(
                    testCode: reIdentificationRequestTask.AsTask);

            // Then
            actualAggregateException.Should().BeEquivalentTo(expectedAggregateException);

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   reIdentificationDependencyException))),
                       Times.Once);

            this.necsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowAggregateExceptionForServiceExceptionsOnBulkProcessIfRequestsFailsAsync()
        {
            // Given
            Guid randomIdentifier = Guid.NewGuid();
            int batchSize = 1;
            int randomCount = 1;
            IdentificationRequest randomIdentificationRequest = CreateRandomIdentificationRequest(count: randomCount);
            IdentificationRequest inputIdentificationRequest = randomIdentificationRequest;

            Mock<ReIdentificationService> reIdentificationServiceMock =
                new Mock<ReIdentificationService>(
                    this.necsBrokerMock.Object,
                    this.identifierBrokerMock.Object,
                    this.necsConfiguration,
                    this.loggingBrokerMock.Object)
                { CallBase = true };

            ReIdentificationService service = reIdentificationServiceMock.Object;
            var serviceException = new Exception();

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ThrowsAsync(serviceException);

            var serverException =
                new Exception(message: GetRandomString());

            var failedServiceIdentificationRequestException =
                new FailedServiceReIdentificationException(
                    message: "Failed re-identification service error occurred, please contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var reIdentificationServiceException = new ReIdentificationServiceException(
                message: "Service error occurred, please contact support.",
                innerException: failedServiceIdentificationRequestException);

            var expectedAggregateException = new AggregateException(
                message: $"Unable to process addresses in 1 of the batch(es) from {inputIdentificationRequest.Id}",
                innerExceptions: reIdentificationServiceException);

            // When
            ValueTask<IdentificationRequest> reIdentificationRequestTask = service
                .BulkProcessRequestsAsync(inputIdentificationRequest, batchSize);

            AggregateException actualAggregateException =
                await Assert.ThrowsAsync<AggregateException>(
                    testCode: reIdentificationRequestTask.AsTask);

            // Then
            actualAggregateException.Should().BeEquivalentTo(expectedAggregateException);

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogErrorAsync(It.Is(SameExceptionAs(
                   reIdentificationServiceException))),
                       Times.Once);

            this.necsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
