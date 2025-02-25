// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

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
        public async Task ShouldRetrieveCsvIdentificationRequestByIdAsync()
        {
            // given
            CsvIdentificationRequest randomCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            CsvIdentificationRequest storageCsvIdentificationRequest = randomCsvIdentificationRequest;
            CsvIdentificationRequest expectedCsvIdentificationRequest = storageCsvIdentificationRequest.DeepClone();

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectCsvIdentificationRequestByIdAsync(randomCsvIdentificationRequest.Id))
                    .ReturnsAsync(storageCsvIdentificationRequest);

            // when
            CsvIdentificationRequest actualCsvIdentificationRequest =
                await csvIdentificationRequestService.RetrieveCsvIdentificationRequestByIdAsync(randomCsvIdentificationRequest.Id);

            // then
            actualCsvIdentificationRequest.Should().BeEquivalentTo(expectedCsvIdentificationRequest);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectCsvIdentificationRequestByIdAsync(randomCsvIdentificationRequest.Id),
                    Times.Once());

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
