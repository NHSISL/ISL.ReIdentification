﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using ISL.ReIdentification.Configurations.Server.Tests.Integration.Brokers;
using ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.PdsData;
using ISL.ReIdentification.Configurations.Server.Tests.Integration.ReIdentification.Brokers;
using Tynamix.ObjectFiller;

namespace ISL.ReIdentification.Configurations.Server.Tests.Integration.Apis.PdsDatas
{
    [Collection(nameof(ApiTestCollection))]
    public partial class PdsDataApiTests
    {
        private readonly ApiBroker apiBroker;

        public PdsDataApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static PdsData CreateRandomPdsData() =>
            CreatePdsDataFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static PdsData CreateRandomPdsData(DateTimeOffset dateTimeOffset) =>
            CreatePdsDataFiller(dateTimeOffset).Create();

        private static Filler<PdsData> CreatePdsDataFiller(DateTimeOffset dateTimeOffset)
        {
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<PdsData>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default)
                .OnProperty(pdsData => pdsData.PseudoNhsNumber).Use(GetRandomStringWithLengthOf(9))
                .OnProperty(pdsData => pdsData.OrgCode).Use(GetRandomStringWithLengthOf(9));

            return filler;
        }

        private async ValueTask<PdsData> PostRandomPdsDataAsync()
        {
            PdsData randomPdsData = CreateRandomPdsData();
            PdsData createdPdsData = await apiBroker.PostPdsDataAsync(randomPdsData);
            createdPdsData.Should().BeEquivalentTo(randomPdsData);

            return createdPdsData;
        }

        private async ValueTask<List<PdsData>> PostRandomPdsDatasAsync()
        {
            int randomNumber = GetRandomNumber();
            var randomPdsDatas = new List<PdsData>();

            for (int i = 0; i < randomNumber; i++)
            {
                randomPdsDatas.Add(await PostRandomPdsDataAsync());
            }

            return randomPdsDatas;
        }

        private static PdsData UpdatePdsDataWithRandomValues(PdsData inputPdsData)
        {
            var updatedPdsData = CreateRandomPdsData();
            updatedPdsData.Id = inputPdsData.Id;

            return updatedPdsData;
        }
    }
}
