// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Brokers;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.CsvIdentificationRequests;
using Tynamix.ObjectFiller;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class CsvIdentificationRequestsApiTests
    {
        private readonly ApiBroker apiBroker;

        public CsvIdentificationRequestsApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static CsvIdentificationRequest CreateRandomCsvIdentificationRequest() =>
            CreateRandomCsvIdentificationRequestFiller().Create();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static Filler<CsvIdentificationRequest> CreateRandomCsvIdentificationRequestFiller()
        {
            string user = Guid.NewGuid().ToString();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<CsvIdentificationRequest>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnType<DateTimeOffset?>().Use(now)

                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.RequesterEmail)
                    .Use(GetRandomStringWithLengthOf(320))

                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.RecipientEmail)
                    .Use(GetRandomStringWithLengthOf(320))

                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.Organisation)
                    .Use(GetRandomStringWithLengthOf(255))

                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.CreatedBy).Use(user)
                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.UpdatedBy).Use(user);

            return filler;
        }

        private static CsvIdentificationRequest UpdateCsvIdentificationRequest(
            CsvIdentificationRequest inputCsvIdentificationRequest)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var updatedCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            updatedCsvIdentificationRequest.Id = inputCsvIdentificationRequest.Id;
            updatedCsvIdentificationRequest.CreatedDate = inputCsvIdentificationRequest.CreatedDate;
            updatedCsvIdentificationRequest.CreatedBy = inputCsvIdentificationRequest.CreatedBy;
            updatedCsvIdentificationRequest.UpdatedDate = now;

            return updatedCsvIdentificationRequest;
        }

        private async ValueTask<List<CsvIdentificationRequest>> PostRandomCsvIdentificationRequests()
        {
            int randomNumber = GetRandomNumber();
            List<CsvIdentificationRequest> randomCsvIdentificationRequestes = new List<CsvIdentificationRequest>();

            for (int i = 0; i < randomNumber; i++)
            {
                randomCsvIdentificationRequestes.Add(await this.PostRandomCsvIdentificationRequest());
            }

            return randomCsvIdentificationRequestes;
        }

        private async ValueTask<CsvIdentificationRequest> PostRandomCsvIdentificationRequest()
        {
            CsvIdentificationRequest randomCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();

            return await this.apiBroker.PostCsvIdentificationRequestAsync(randomCsvIdentificationRequest);
        }
    }
}
