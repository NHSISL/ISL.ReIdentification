// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Brokers;
using ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Models.OdsDatas;
using Microsoft.EntityFrameworkCore;
using Tynamix.ObjectFiller;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class OdsDataApiTests
    {
        private readonly ApiBroker apiBroker;

        public OdsDataApiTests(ApiBroker apiBroker)
        {
            this.apiBroker = apiBroker;
        }

        private async ValueTask<OdsData> PostRandomOdsDataAsync()
        {
            OdsData randomOdsData = CreateRandomOdsData();

            return await this.apiBroker.PostOdsDataAsync(randomOdsData);
        }

        private async ValueTask<List<OdsData>> PostRandomOdsDatasAsync()
        {
            int randomNumber = GetRandomNumber();
            var randomOdsDatas = new List<OdsData>();

            for (int i = 0; i < randomNumber; i++)
            {
                randomOdsDatas.Add(await PostRandomOdsDataAsync());
            }

            return randomOdsDatas;
        }

        private int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

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
                hierarchyId = HierarchyId.Parse("/");
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
