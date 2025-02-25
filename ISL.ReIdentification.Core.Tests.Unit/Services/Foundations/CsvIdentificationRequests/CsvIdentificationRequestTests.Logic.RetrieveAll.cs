// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.CsvIdentificationRequests
{
    public partial class CsvIdentificationRequestsTests
    {
        [Fact]
        public async Task ShouldRetrieveAllCsvIdentificationRequestsAsync()
        {
            //given
            IQueryable<CsvIdentificationRequest> randomCsvIdentificationRequests = CreateRandomCsvIdentificationRequests();
            IQueryable<CsvIdentificationRequest> storageCsvIdentificationRequests = randomCsvIdentificationRequests;
            IQueryable<CsvIdentificationRequest> expectedCsvIdentificationRequests = storageCsvIdentificationRequests;

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllCsvIdentificationRequestsAsync())
                    .ReturnsAsync(storageCsvIdentificationRequests);

            //when
            IQueryable<CsvIdentificationRequest> actualCsvIdentificationRequests =
                await this.csvIdentificationRequestService.RetrieveAllCsvIdentificationRequestsAsync();

            // then
            actualCsvIdentificationRequests.Should().BeEquivalentTo(expectedCsvIdentificationRequests);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllCsvIdentificationRequestsAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}