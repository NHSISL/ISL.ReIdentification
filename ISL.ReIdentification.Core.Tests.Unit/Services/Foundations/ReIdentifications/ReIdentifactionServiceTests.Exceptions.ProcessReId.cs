// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications.Exceptions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.ReIdentifications
{
    public partial class ReIdentificationServiceTests
    {
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

            reIdentificationBrokerMock.Setup(service =>
                service.ReIdentifyAsync(It.IsAny<ReIdentificationRequest>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IdentificationRequest> processIdentificationRequestTask =
                reIdentificationService.ProcessReIdentificationRequest(someIdentificationRequest);

            ReIdentificationServiceException actualReIdentificationServiceException =
                await Assert.ThrowsAsync<ReIdentificationServiceException>(
                    testCode: processIdentificationRequestTask.AsTask);

            // then
            actualReIdentificationServiceException.Should().BeEquivalentTo(
                expectedReIdentificationServiceException);

            reIdentificationBrokerMock.Verify(service =>
                service.ReIdentifyAsync(It.IsAny<ReIdentificationRequest>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedReIdentificationServiceException))),
                        Times.Once);

            this.reIdentificationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
