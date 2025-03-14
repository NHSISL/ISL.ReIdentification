// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using ISL.ReIdentification.Configurations.Server.Tests.Integration.Brokers;
using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.OdsData;
using ISL.ReIdentification.Configurations.Server.Tests.Integration.ReIdentification.Brokers;
using Microsoft.EntityFrameworkCore;
using Tynamix.ObjectFiller;

namespace ISL.ReIdentification.Configuration.Server.Tests.Integration.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class OdsDataApiTests
    {
        private readonly ApiBroker apiBroker;

        public OdsDataApiTests(ApiBroker apiBroker)
        {
            this.apiBroker = apiBroker;
        }

        private static OdsData UpdateOdsDataWithRandomValues(OdsData inputOdsData)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var updatedOdsData = CreateRandomOdsData();
            updatedOdsData.Id = inputOdsData.Id;

            return updatedOdsData;
        }

        private async ValueTask<OdsData> PostRandomOdsDataAsync()
        {
            OdsData randomOdsData = CreateRandomOdsData();
            OdsData createdOdsData = await this.apiBroker.PostOdsDataAsync(randomOdsData);
            createdOdsData.Should().BeEquivalentTo(randomOdsData);

            return createdOdsData;
        }

        private async ValueTask<List<OdsData>> PostRandomChildOdsDatasAsync(string parentHierarchyIdString)
        {
            HierarchyId parentHierarchyId = HierarchyId.Parse("/");

            if (parentHierarchyIdString is not null)
            {
                parentHierarchyId = HierarchyId.Parse(parentHierarchyIdString);
            }

            List<OdsData> children = CreateOdsDataFiller(dateTimeOffset: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber())
                    .ToList();

            HierarchyId lastChildHierarchy = null;
            List<OdsData> childItems = new List<OdsData>();

            foreach (var child in children)
            {
                child.OdsHierarchy = parentHierarchyId.GetDescendant(lastChildHierarchy, null).ToString();
                lastChildHierarchy = HierarchyId.Parse(child.OdsHierarchy);
                OdsData item = await this.apiBroker.PostOdsDataAsync(child);
                item.Should().BeEquivalentTo(child);
                childItems.Add(item);
            }

            return children;
        }

        private async ValueTask<List<OdsData>> PostRandomOdsDatasAsync()
        {
            int randomNumber = GetRandomNumber();
            var randomOdsDatas = new List<OdsData>();

            for (int i = 0; i < randomNumber; i++)
            {
                OdsData randomOdsData = await PostRandomOdsDataAsync();
                randomOdsDatas.Add(randomOdsData);
            }

            return randomOdsDatas;
        }

        private int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static int GetRandomLargeRangeNumber() =>
            new IntRange(min: 2, max: 999).GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static OdsData CreateRandomOdsData() =>
            CreateOdsDataFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static OdsData CreateRandomOdsData(DateTimeOffset dateTimeOffset) =>
            CreateOdsDataFiller(dateTimeOffset).Create();

        private static Filler<OdsData> CreateOdsDataFiller(
            DateTimeOffset dateTimeOffset, HierarchyId hierarchyId = null)
        {
            var filler = new Filler<OdsData>();

            if (hierarchyId == null)
            {
                var randomNumber = GetRandomLargeRangeNumber();
                hierarchyId = HierarchyId.Parse($"/{randomNumber}/");
            }

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default)
                .OnProperty(odsData => odsData.OrganisationCode).Use(GetRandomStringWithLengthOf(5))
                .OnProperty(odsData => odsData.OrganisationName).Use(GetRandomStringWithLengthOf(5))
                .OnProperty(odsData => odsData.OdsHierarchy).Use(hierarchyId.ToString());

            return filler;
        }
    }
}
