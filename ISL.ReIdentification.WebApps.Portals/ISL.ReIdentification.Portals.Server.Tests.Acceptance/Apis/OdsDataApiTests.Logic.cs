// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.OdsDatas;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Apis
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

        [Fact]
        public async Task ShouldGetAllOdsDatasAsync()
        {
            // given
            List<OdsData> randomOdsDatas = await PostRandomOdsDatasAsync();
            List<OdsData> expectedOdsDatas = randomOdsDatas;

            // when
            List<OdsData> actualOdsDatas = await this.apiBroker.GetAllOdsDatasAsync();

            // then
            foreach (OdsData expectedOdsData in expectedOdsDatas)
            {
                OdsData actualOdsData = actualOdsDatas.Single(approval => approval.Id == expectedOdsData.Id);
                actualOdsData.Should().BeEquivalentTo(expectedOdsData);
                await this.apiBroker.DeleteOdsDataByIdAsync(actualOdsData.Id);
            }
        }

        [Fact]
        public async Task ShouldGetOdsDataByIdAsync()
        {
            // given
            OdsData randomOdsData = await PostRandomOdsDataAsync();
            OdsData expectedOdsData = randomOdsData;

            // when
            OdsData actualOdsData = await this.apiBroker.GetOdsDataByIdAsync(randomOdsData.Id);

            // then
            actualOdsData.Should().BeEquivalentTo(expectedOdsData);
            await this.apiBroker.DeleteOdsDataByIdAsync(actualOdsData.Id);
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
        public async Task ShouldDeleteOdsDataAsync()
        {
            // given
            OdsData randomOdsData = await PostRandomOdsDataAsync();
            OdsData inputOdsData = randomOdsData;
            OdsData expectedOdsData = inputOdsData;

            // when
            OdsData deletedOdsData =
                await this.apiBroker.DeleteOdsDataByIdAsync(inputOdsData.Id);

            List<OdsData> actualResult =
                await this.apiBroker.GetSpecificOdsDataByIdAsync(inputOdsData.Id);

            // then
            actualResult.Count().Should().Be(0);
        }

        [Fact]
        public async Task ShouldGetChildrenAsync()
        {
            // given
            OdsData randomOdsData = await PostRandomOdsDataAsync();
            List<OdsData> randomOdsDatas = await PostRandomChildOdsDatasAsync(randomOdsData.OdsHierarchy);
            List<OdsData> grandchildrenDatas = await PostRandomChildOdsDatasAsync(randomOdsDatas[0].OdsHierarchy);
            List<OdsData> expectedOdsDatas = randomOdsDatas;

            // when
            List<OdsData> actualOdsDatas = await this.apiBroker.GetChildrenAsync(randomOdsData.Id);

            // then
            foreach (OdsData expectedOdsData in expectedOdsDatas)
            {
                OdsData actualOdsData = actualOdsDatas.Single(odsData => odsData.Id == expectedOdsData.Id);
                actualOdsData.Should().BeEquivalentTo(expectedOdsData);
                await this.apiBroker.DeleteOdsDataByIdAsync(actualOdsData.Id);
            }

            foreach (OdsData grandchildOdsData in grandchildrenDatas)
            {
                await this.apiBroker.DeleteOdsDataByIdAsync(grandchildOdsData.Id);
            }

            await this.apiBroker.DeleteOdsDataByIdAsync(randomOdsData.Id);
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

            await this.apiBroker.DeleteOdsDataByIdAsync(randomOdsData.Id);
        }

        [Fact]
        public async Task ShouldGetAncestorsAsync()
        {
            // given
            OdsData randomOdsData = await PostRandomOdsDataAsync();
            List<OdsData> childOdsDatas = await PostRandomChildOdsDatasAsync(randomOdsData.OdsHierarchy);
            List<OdsData> grandchildrenOdsDatas = await PostRandomChildOdsDatasAsync(childOdsDatas[0].OdsHierarchy);

            List<OdsData> expectedOdsDatas = new List<OdsData>()
            {
                randomOdsData,
                childOdsDatas[0],
                grandchildrenOdsDatas[0]
            };

            Guid inputOdsDataId = grandchildrenOdsDatas[0].Id;

            // when
            List<OdsData> actualOdsDatas = await this.apiBroker.GetAncestorsAsync(inputOdsDataId);

            // then
            foreach (OdsData expectedOdsData in expectedOdsDatas)
            {
                OdsData actualOdsData = actualOdsDatas.Single(odsData => odsData.Id == expectedOdsData.Id);
                actualOdsData.Should().BeEquivalentTo(expectedOdsData);
            }

            foreach (OdsData odsData in childOdsDatas)
            {
                await this.apiBroker.DeleteOdsDataByIdAsync(odsData.Id);
            }

            foreach (OdsData odsData in grandchildrenOdsDatas)
            {
                await this.apiBroker.DeleteOdsDataByIdAsync(odsData.Id);
            }

            await this.apiBroker.DeleteOdsDataByIdAsync(randomOdsData.Id);
        }
    }
}
