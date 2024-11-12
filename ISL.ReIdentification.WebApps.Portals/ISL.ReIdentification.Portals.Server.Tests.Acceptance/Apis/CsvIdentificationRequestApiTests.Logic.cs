// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.CsvIdentificationRequests;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Apis
{
    public partial class CsvIdentificationRequestsApiTests
    {
        [Fact]
        public async Task ShouldPostCsvIdentificationRequestAsync()
        {
            // given
            CsvIdentificationRequest randomCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            CsvIdentificationRequest inputCsvIdentificationRequest = randomCsvIdentificationRequest;
            CsvIdentificationRequest expectedCsvIdentificationRequest = inputCsvIdentificationRequest;

            // when
            await this.apiBroker.PostCsvIdentificationRequestAsync(inputCsvIdentificationRequest);

            CsvIdentificationRequest actualCsvIdentificationRequest =
                await this.apiBroker.GetCsvIdentificationRequestByIdAsync(inputCsvIdentificationRequest.Id);

            // then
            actualCsvIdentificationRequest.Should().BeEquivalentTo(expectedCsvIdentificationRequest);
            await this.apiBroker.DeleteCsvIdentificationRequestByIdAsync(inputCsvIdentificationRequest.Id);
        }

        [Fact]
        public async Task ShouldGetAllCsvIdentificationRequestesAsync()
        {
            // given
            List<CsvIdentificationRequest> randomCsvIdentificationRequestes =
                await PostRandomCsvIdentificationRequests();

            List<CsvIdentificationRequest> expectedCsvIdentificationRequestes = randomCsvIdentificationRequestes;

            // when
            List<CsvIdentificationRequest> actualCsvIdentificationRequestes =
                await this.apiBroker.GetAllCsvIdentificationRequestsAsync();

            // then
            foreach (var expectedCsvIdentificationRequest in expectedCsvIdentificationRequestes)
            {
                CsvIdentificationRequest actualCsvIdentificationRequest = actualCsvIdentificationRequestes.Single(
                    actual => actual.Id == expectedCsvIdentificationRequest.Id);

                actualCsvIdentificationRequest.Should().BeEquivalentTo(expectedCsvIdentificationRequest);
                await this.apiBroker.DeleteCsvIdentificationRequestByIdAsync(actualCsvIdentificationRequest.Id);
            }
        }

        [Fact]
        public async Task ShouldGetCsvIdentificationRequestByIdAsync()
        {
            // given
            CsvIdentificationRequest randomCsvIdentificationRequest = await PostRandomCsvIdentificationRequest();
            CsvIdentificationRequest expectedCsvIdentificationRequest = randomCsvIdentificationRequest;

            // when
            CsvIdentificationRequest actualCsvIdentificationRequest =
                await this.apiBroker.GetCsvIdentificationRequestByIdAsync(randomCsvIdentificationRequest.Id);

            // then
            actualCsvIdentificationRequest.Should().BeEquivalentTo(expectedCsvIdentificationRequest);
            await this.apiBroker.DeleteCsvIdentificationRequestByIdAsync(actualCsvIdentificationRequest.Id);
        }

        [Fact]
        public async Task ShouldPutCsvIdentificationRequestAsync()
        {
            // given
            CsvIdentificationRequest randomCsvIdentificationRequest = await PostRandomCsvIdentificationRequest();

            CsvIdentificationRequest updatedCsvIdentificationRequest =
                UpdateCsvIdentificationRequest(randomCsvIdentificationRequest);

            // when
            await this.apiBroker.PutCsvIdentificationRequestAsync(updatedCsvIdentificationRequest);

            CsvIdentificationRequest actualCsvIdentificationRequest =
                await this.apiBroker.GetCsvIdentificationRequestByIdAsync(randomCsvIdentificationRequest.Id);

            // then
            actualCsvIdentificationRequest.Should().BeEquivalentTo(updatedCsvIdentificationRequest);
            await this.apiBroker.DeleteCsvIdentificationRequestByIdAsync(actualCsvIdentificationRequest.Id);
        }

        [Fact]
        public async Task ShouldDeleteCsvIdentificationRequestAsync()
        {
            // given
            CsvIdentificationRequest randomCsvIdentificationRequest = await PostRandomCsvIdentificationRequest();
            CsvIdentificationRequest expectedDeletedCsvIdentificationRequest = randomCsvIdentificationRequest;

            // when
            CsvIdentificationRequest actualCsvIdentificationRequest =
                await this.apiBroker.DeleteCsvIdentificationRequestByIdAsync(
                    expectedDeletedCsvIdentificationRequest.Id);

            List<CsvIdentificationRequest> actualResult =
                await this.apiBroker.GetSpecificCsvIdentificationRequestByIdAsync(
                    expectedDeletedCsvIdentificationRequest.Id);

            //then
            actualResult.Count().Should().Be(0);
        }
    }
}
