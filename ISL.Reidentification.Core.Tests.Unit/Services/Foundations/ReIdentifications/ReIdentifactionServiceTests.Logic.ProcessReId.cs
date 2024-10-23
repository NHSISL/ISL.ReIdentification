// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Services.Foundations.ReIdentifications;
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
            int batchSize = this.necsConfiguration.ApiMaxBatchSize;
            IdentificationRequest randomIdentificationRequest = CreateRandomIdentificationRequest(count: randomCount);
            IdentificationRequest inputIdentificationRequest = randomIdentificationRequest;
            IdentificationRequest storageIdentificationRequest = inputIdentificationRequest.DeepClone();
            IdentificationRequest expectedIdentificationRequest = storageIdentificationRequest.DeepClone();

            Mock<ReIdentificationService> reIdentificationServiceMock =
                new Mock<ReIdentificationService>(
                    this.necsBrokerMock.Object,
                    this.identifierBrokerMock.Object,
                    this.necsConfiguration,
                    this.loggingBrokerMock.Object)
                { CallBase = true };

            reIdentificationServiceMock.Setup(service =>
                service.BulkProcessRequestsAsync(inputIdentificationRequest, batchSize))
                    .ReturnsAsync(storageIdentificationRequest);

            ReIdentificationService service = reIdentificationServiceMock.Object;

            // When
            IdentificationRequest actualIdentificationRequest = await service
                .ProcessReIdentificationRequest(inputIdentificationRequest);

            // Then
            actualIdentificationRequest.Should().BeEquivalentTo(expectedIdentificationRequest);

            reIdentificationServiceMock.Verify(service =>
                service.BulkProcessRequestsAsync(inputIdentificationRequest, batchSize),
                    Times.Once());

            this.necsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
