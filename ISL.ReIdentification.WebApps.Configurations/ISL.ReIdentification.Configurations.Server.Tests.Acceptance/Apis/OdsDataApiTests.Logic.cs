// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Models.OdsDatas;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Apis
{
    public partial class OdsDataApiTests
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
        public async Task ShouldPutOdsDataAsync()
        {
            // given
            OdsData randomOdsData = await PostRandomOdsDataAsync();
            OdsData modifiedOdsData = UpdateOdsDataWithRandomValues(randomOdsData);

            // when
            await this.apiBroker.PutOdsDataAsync(modifiedOdsData);
            OdsData actualOdsData = await this.apiBroker.GetOdsDataByIdAsync(randomOdsData.Id);

            // then
            actualOdsData.Should().BeEquivalentTo(modifiedOdsData);
            await this.apiBroker.DeleteOdsDataByIdAsync(actualOdsData.Id);
        }

        [Fact]
        public async Task ShouldGetDescendantsAsync()
        {
            // given
            OdsData randomOdsData = await PostRandomOdsDataAsync();
            List<OdsData> childOdsDatas = await PostRandomChildOdsDatasAsync(randomOdsData.OdsHierarchy);
            List<OdsData> grandchildrenOdsDatas = await PostRandomChildOdsDatasAsync(childOdsDatas[0].OdsHierarchy);
            List<OdsData> expectedOdsDatas = new List<OdsData>();
            expectedOdsDatas.AddRange(childOdsDatas);
            expectedOdsDatas.AddRange(grandchildrenOdsDatas);

            // when
            List<OdsData> actualOdsDatas = await this.apiBroker.GetDescendantsAsync(randomOdsData.Id);

            // then
            foreach (OdsData expectedOdsData in expectedOdsDatas)
            {
                OdsData actualOdsData = actualOdsDatas.Single(odsData => odsData.Id == expectedOdsData.Id);
                actualOdsData.Should().BeEquivalentTo(expectedOdsData);
                await this.apiBroker.DeleteOdsDataByIdAsync(actualOdsData.Id);
            }

            foreach (OdsData grandchildOdsData in grandchildrenOdsDatas)
            {
                await this.apiBroker.DeleteOdsDataByIdAsync(grandchildOdsData.Id);
            }

            await this.apiBroker.DeleteOdsDataByIdAsync(randomOdsData.Id);
        }
    }
}
