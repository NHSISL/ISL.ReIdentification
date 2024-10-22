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

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.ReIdentifications
{
    public partial class ReIdentificationServiceTests
    {
        [Fact]
        public async Task ShouldThrowAggregateServiceExceptionOnProcessIfAggregateOccurredAndLogItAsync()
        {
            // given
            int randomCount = GetRandomNumber();
            IdentificationRequest someIdentificationRequest = CreateRandomIdentificationRequest(count: randomCount);
            var aggregateException = new AggregateException();

            var failedServiceReIdentificationException =
                new FailedServiceReIdentificationException(
                    message: "Failed re-identification aggregate service error occurred, please contact support.",
                    innerException: aggregateException,
                    data: aggregateException.Data);

            var expectedReIdentificationServiceException =
                new ReIdentificationServiceException(
                    message: "Service error occurred, please contact support.",
                    innerException: failedServiceReIdentificationException);

            Mock<ReIdentificationService> reIdentificationServiceMock =
                new Mock<ReIdentificationService>(
                    this.necsBrokerMock.Object,
                    this.identifierBrokerMock.Object,
                    this.necsConfiguration,
                    this.loggingBrokerMock.Object)
                { CallBase = true };

            reIdentificationServiceMock.Setup(service =>
                service.BulkProcessRequestsAsync(It.IsAny<IdentificationRequest>(), It.IsAny<int>()))
                    .ThrowsAsync(aggregateException);

            IReIdentificationService service = reIdentificationServiceMock.Object;

            // when
            ValueTask<IdentificationRequest> processIdentificationRequestTask =
                service.ProcessReIdentificationRequest(someIdentificationRequest);

            ReIdentificationServiceException actualReIdentificationServiceException =
                await Assert.ThrowsAsync<ReIdentificationServiceException>(
                    testCode: processIdentificationRequestTask.AsTask);

            // then
            actualReIdentificationServiceException.Should().BeEquivalentTo(
                expectedReIdentificationServiceException);

            reIdentificationServiceMock.Verify(service =>
                service.BulkProcessRequestsAsync(It.IsAny<IdentificationRequest>(), It.IsAny<int>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedReIdentificationServiceException))),
                        Times.Once);

            this.necsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnProcessIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            int randomCount = GetRandomNumber();
            IdentificationRequest someIdentificationRequest = CreateRandomIdentificationRequest(count: randomCount);
            var serviceException = new Exception();

            var failedServiceReIdentificationException =
                new FailedServiceReIdentificationException(
                    message: "Failed re-identification service error occurred, please contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedReIdentificationServiceException =
                new ReIdentificationServiceException(
                    message: "Service error occurred, please contact support.",
                    innerException: failedServiceReIdentificationException);

            Mock<ReIdentificationService> reIdentificationServiceMock =
                new Mock<ReIdentificationService>(
                    this.necsBrokerMock.Object,
                    this.identifierBrokerMock.Object,
                    this.necsConfiguration,
                    this.loggingBrokerMock.Object)
                { CallBase = true };

            reIdentificationServiceMock.Setup(service =>
                service.BulkProcessRequestsAsync(It.IsAny<IdentificationRequest>(), It.IsAny<int>()))
                    .ThrowsAsync(serviceException);

            IReIdentificationService service = reIdentificationServiceMock.Object;

            // when
            ValueTask<IdentificationRequest> processIdentificationRequestTask =
                service.ProcessReIdentificationRequest(someIdentificationRequest);

            ReIdentificationServiceException actualReIdentificationServiceException =
                await Assert.ThrowsAsync<ReIdentificationServiceException>(
                    testCode: processIdentificationRequestTask.AsTask);

            // then
            actualReIdentificationServiceException.Should().BeEquivalentTo(
                expectedReIdentificationServiceException);

            reIdentificationServiceMock.Verify(service =>
                service.BulkProcessRequestsAsync(It.IsAny<IdentificationRequest>(), It.IsAny<int>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedReIdentificationServiceException))),
                        Times.Once);

            this.necsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
