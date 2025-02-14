// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.CsvIdentificationRequests
{
    public partial class CsvIdentificationRequestsTests
    {
        [Fact]
        public async Task ShouldRemoveCsvIdentificationRequestByIdAsync()
        {
            // given
            Guid someCsvIdentificationRequestId = Guid.NewGuid();
            CsvIdentificationRequest randomCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            CsvIdentificationRequest storageCsvIdentificationRequest = randomCsvIdentificationRequest;
            CsvIdentificationRequest inputCsvIdentificationRequest = storageCsvIdentificationRequest;
            CsvIdentificationRequest removedCsvIdentificationRequest = inputCsvIdentificationRequest;
            CsvIdentificationRequest expectedCsvIdentificationRequest = removedCsvIdentificationRequest.DeepClone();

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectCsvIdentificationRequestByIdAsync(someCsvIdentificationRequestId))
                    .ReturnsAsync(storageCsvIdentificationRequest);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.DeleteCsvIdentificationRequestAsync(inputCsvIdentificationRequest))
                    .ReturnsAsync(removedCsvIdentificationRequest);

            // when
            CsvIdentificationRequest actualCsvIdentificationRequest =
                await this.csvIdentificationRequestService.RemoveCsvIdentificationRequestByIdAsync(someCsvIdentificationRequestId);

            // then
            actualCsvIdentificationRequest.Should().BeEquivalentTo(expectedCsvIdentificationRequest);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectCsvIdentificationRequestByIdAsync(someCsvIdentificationRequestId),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.DeleteCsvIdentificationRequestAsync(storageCsvIdentificationRequest),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}