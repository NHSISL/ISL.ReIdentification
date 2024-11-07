// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.OdsDatas;
using RESTFulSense.Exceptions;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Apis
{
    public partial class OdsDatasApiTests
    {
        [Fact]
        public async Task ShouldPostOdsDataAsync()
        {
            // given
            OdsData randomOdsData = CreateRandomOdsData();
            OdsData inputOdsData = randomOdsData;
            OdsData expectedOdsData = inputOdsData;

            // when 
            await this.apiBroker.PostOdsDataAsync(inputOdsData);

            OdsData actualOdsData =
                await this.apiBroker.GetOdsDataByIdAsync(inputOdsData.Id);

            // then
            actualOdsData.Should().BeEquivalentTo(expectedOdsData);
            await this.apiBroker.DeleteOdsDataByIdAsync(actualOdsData.Id);
        }

        [Fact(Skip = "Need to refactor tests and add other crud operations")]
        public async Task ShouldGetAllOdsDatasAsync()
        {
            // when
            var actualOdsDatas = await this.apiBroker.GetAllOdsDatasAsync();

            // then
            actualOdsDatas.Should().NotBeNull();
        }

        [Fact(Skip = "Need to refactor tests and add other crud operations")]
        public async Task ShouldGetOdsDataByIdAsync()
        {
            // given
            Guid randomOdsDataId = Guid.NewGuid();

            // when
            var actualOdsData = await this.apiBroker.GetOdsDataByIdAsync(randomOdsDataId);

            // then
            actualOdsData.Should().NotBeNull();
        }

        [Fact]
        public async Task ShouldDeleteOdsDataAsync()
        {
            // given
            OdsData randomOdsData = await PostRandomOdsDataAsync();
            OdsData inputOdsData = randomOdsData;
            OdsData expectedOdsData = inputOdsData;

            // when
            OdsData deletedOdsData =
                await this.apiBroker.DeleteOdsDataByIdAsync(inputOdsData.Id);

            ValueTask<OdsData> getOdsDatabyIdTask =
                this.apiBroker.GetOdsDataByIdAsync(inputOdsData.Id);

            // then
            deletedOdsData.Should().BeEquivalentTo(expectedOdsData);

            await Assert.ThrowsAsync<HttpResponseNotFoundException>(
                testCode: getOdsDatabyIdTask.AsTask);
        }
    }
}
