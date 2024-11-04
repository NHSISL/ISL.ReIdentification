// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.ReIdentifications
{
    public partial class ReIdentificationServiceTests
    {
        [Fact]
        public async Task ShouldProcessReIdentificationRequestsAsync()
        {
            // Given
            int randomCount = GetRandomNumber();
            IdentificationRequest randomIdentificationRequest = CreateRandomIdentificationRequest(count: randomCount);
            IdentificationRequest inputIdentificationRequest = randomIdentificationRequest;
            ReIdentificationRequest reIdentificationRequest = MapToReIdentificationRequest(randomIdentificationRequest);
            ReIdentificationRequest inputReIdentificationRequest = reIdentificationRequest;
            ReIdentificationRequest storageReIdentificationRequest = reIdentificationRequest.DeepClone();
            storageReIdentificationRequest.ReIdentificationItems.ForEach(item =>
            {
                item.Identifier = GetRandomString();
                item.Message = "OK";
            });

            IdentificationRequest expectedIdentificationRequest =
                MapToIdentificationRequest(storageReIdentificationRequest, inputIdentificationRequest).DeepClone();

            reIdentificationBrokerMock.Setup(service =>
                service.ReIdentifyAsync(It.Is(SameReIdentificationRequestAs(inputReIdentificationRequest))))
                    .ReturnsAsync(storageReIdentificationRequest);

            // When
            IdentificationRequest actualIdentificationRequest = await reIdentificationService
                .ProcessReIdentificationRequest(inputIdentificationRequest);

            // Then
            actualIdentificationRequest.Should().BeEquivalentTo(expectedIdentificationRequest);

            reIdentificationBrokerMock.Verify(service =>
                service.ReIdentifyAsync(It.Is(SameReIdentificationRequestAs(inputReIdentificationRequest))),
                    Times.Once());

            this.reIdentificationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
