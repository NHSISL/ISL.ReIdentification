// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Brokers.NECS.Requests;
using ISL.ReIdentification.Core.Models.Brokers.NECS.Responses;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Services.Foundations.ReIdentifications;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.ReIdentifications
{
    public partial class ReIdentificationServiceTests
    {
        [Fact]
        public async Task ShouldBulkProcessRequestsAsync()
        {
            // Given
            Guid randomIdentifier = Guid.NewGuid();
            int batchSize = GetRandomNumber();
            int randomCount = (batchSize * GetRandomNumber()) + GetRandomNumber();
            IdentificationRequest randomIdentificationRequest = CreateRandomIdentificationRequest(count: randomCount);

            (List<NecsReidentificationRequest> requests, List<NecsReIdentificationResponse> responses) =
                CreateBatchedItems(randomIdentificationRequest, batchSize, randomIdentifier);

            for (int i = 0; i < requests.Count; i++)
            {
                NecsReidentificationRequest necsReidentificationRequest = requests[i];
                NecsReIdentificationResponse necsReIdentificationResponse = responses[i];

                this.necsBrokerMock.Setup(broker =>
                    broker.ReIdAsync(It.Is(SameNecsReidentificationRequestAs(necsReidentificationRequest))))
                        .ReturnsAsync(necsReIdentificationResponse);
            }


            IdentificationRequest inputIdentificationRequest = randomIdentificationRequest;
            IdentificationRequest outputIdentificationRequest = randomIdentificationRequest.DeepClone();

            outputIdentificationRequest.IdentificationItems.ForEach(item =>
                {
                    item.Identifier = $"{item.Identifier}R";
                    item.Message = $"{item.Message}M";
                    item.IsReidentified = true;
                });

            IdentificationRequest expectedIdentificationRequest = outputIdentificationRequest.DeepClone();

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
                    .ReturnsAsync(randomIdentifier);

            // When
            IdentificationRequest actualIdentificationRequest = await service
                .BulkProcessRequestsAsync(inputIdentificationRequest, batchSize);

            // Then
            actualIdentificationRequest.Should().BeEquivalentTo(expectedIdentificationRequest);

            for (int i = 0; i < requests.Count; i++)
            {
                NecsReidentificationRequest necsReidentificationRequest = requests[i];
                NecsReIdentificationResponse necsReIdentificationResponse = responses[i];

                this.necsBrokerMock.Verify(broker =>
                    broker.ReIdAsync(It.Is(SameNecsReidentificationRequestAs(necsReidentificationRequest))),
                        Times.Once);
            }

            this.necsBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

    }
}
