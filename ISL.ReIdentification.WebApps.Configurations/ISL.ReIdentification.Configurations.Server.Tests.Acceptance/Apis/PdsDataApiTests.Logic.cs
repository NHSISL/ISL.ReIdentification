// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Models.PdsDatas;
using RESTFulSense.Exceptions;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Apis
{
    public partial class PdsDataApiTests
    {
        [Fact]
        public async Task ShouldPostPdsDataAsync()
        {
            // given
            PdsData randomPdsData = CreateRandomPdsData();
            PdsData inputPdsData = randomPdsData;
            PdsData expectedPdsData = inputPdsData;

            // when 
            await this.apiBroker.PostPdsDataAsync(inputPdsData);

            PdsData actuaPdsData =
                await this.apiBroker.GetPdsDataByIdAsync(inputPdsData.Id);

            // then
            actuaPdsData.Should().BeEquivalentTo(expectedPdsData);
            await this.apiBroker.DeletePdsDataByIdAsync(actuaPdsData.Id);
        }

        [Fact]
        public async Task ShouldGetAllPdsDataAsync()
        {
            // given
            List<PdsData> randomPdsDatas = await PostRandomPdsDatasAsync();
            List<PdsData> expectedPdsDatas = randomPdsDatas;

            // when
            List<PdsData> actualPdsDatas = await this.apiBroker.GetAllPdsDataAsync();

            // then
            foreach (PdsData expectedPdsData in expectedPdsDatas)
            {
                PdsData actualPdsData = actualPdsDatas.Single(pdsData => pdsData.Id == expectedPdsData.Id);
                actualPdsData.Should().BeEquivalentTo(expectedPdsData);
                await this.apiBroker.DeletePdsDataByIdAsync(actualPdsData.Id);
            }
        }

        [Fact]
        public async Task ShouldGetPdsDataByIdAsync()
        {
            // given
            PdsData randomPdsData = await PostRandomPdsDataAsync();
            PdsData expectedPdsData = randomPdsData;

            // when
            PdsData actualPdsData = await this.apiBroker.GetPdsDataByIdAsync(randomPdsData.Id);

            // then
            actualPdsData.Should().BeEquivalentTo(expectedPdsData);
            await this.apiBroker.DeletePdsDataByIdAsync(actualPdsData.Id);
        }

        [Fact]
        public async Task ShouldPutPdsDataAsync()
        {
            // given
            PdsData randomPdsData = await PostRandomPdsDataAsync();
            PdsData modifiedPdsData = UpdatePdsDataWithRandomValues(randomPdsData);

            // when
            await this.apiBroker.PutPdsDataAsync(modifiedPdsData);
            PdsData actualPdsData = await this.apiBroker.GetPdsDataByIdAsync(randomPdsData.Id);

            // then
            actualPdsData.Should().BeEquivalentTo(modifiedPdsData);
            await this.apiBroker.DeletePdsDataByIdAsync(actualPdsData.Id);
        }

        [Fact]
        public async Task ShouldDeletePdsDataAsync()
        {
            // given
            PdsData randomPdsData = await PostRandomPdsDataAsync();
            PdsData inputPdsData = randomPdsData;
            PdsData expectedPdsData = inputPdsData;

            // when
            PdsData deletedPdsData =
                await this.apiBroker.DeletePdsDataByIdAsync(inputPdsData.Id);

            ValueTask<PdsData> getPdsDatabyIdTask =
                this.apiBroker.GetPdsDataByIdAsync(inputPdsData.Id);

            // then
            deletedPdsData.Should().BeEquivalentTo(expectedPdsData);

            await Assert.ThrowsAsync<HttpResponseNotFoundException>(
                testCode: getPdsDatabyIdTask.AsTask);
        }
    }
}
