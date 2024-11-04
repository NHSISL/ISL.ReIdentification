// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Brokers;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.Accesses;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.CsvIdentificationRequests;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.ImpersonationContexts;
using Tynamix.ObjectFiller;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Apis
{
    [Collection(nameof(ApiTestCollection))]
    public partial class ReIdentificationApiTests
    {
        private readonly ApiBroker apiBroker;

        public ReIdentificationApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static AccessRequest CreateAccessRequest() =>
            new AccessRequest
            {
                ImpersonationContext = CreateRandomImpersonationContext(),
                CsvIdentificationRequest = CreateRandomCsvIdentificationRequest(),
            };


        private static ImpersonationContext CreateRandomImpersonationContext() =>
           CreateRandomImpersonationContextFiller().Create();

        private static CsvIdentificationRequest CreateRandomCsvIdentificationRequest() =>
            CreateCsvIdentificationRequestFiller().Create();

        private static Filler<ImpersonationContext> CreateRandomImpersonationContextFiller()
        {
            string user = Guid.NewGuid().ToString();
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var filler = new Filler<ImpersonationContext>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)

                .OnProperty(impersonationContext => impersonationContext.RequesterEmail)
                    .Use(() => GetRandomStringWithLengthOf(320))

                .OnProperty(impersonationContext => impersonationContext.ResponsiblePersonEmail)
                    .Use(() => GetRandomStringWithLengthOf(320))

                .OnProperty(impersonationContext => impersonationContext.Organisation)
                    .Use(() => GetRandomStringWithLengthOf(255))

                .OnProperty(impersonationContext => impersonationContext.ProjectName)
                    .Use(() => GetRandomStringWithLengthOf(255))

                .OnProperty(impersonationContext => impersonationContext.IdentifierColumn)
                    .Use(() => GetRandomStringWithLengthOf(10))

                .OnProperty(impersonationContext => impersonationContext.CreatedDate).Use(now)
                .OnProperty(impersonationContext => impersonationContext.CreatedBy).Use(user)
                .OnProperty(impersonationContext => impersonationContext.UpdatedDate).Use(now)
                .OnProperty(impersonationContext => impersonationContext.UpdatedBy).Use(user);

            return filler;
        }

        private static Filler<CsvIdentificationRequest> CreateCsvIdentificationRequestFiller()
        {
            string user = Guid.NewGuid().ToString();
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var filler = new Filler<CsvIdentificationRequest>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnType<DateTimeOffset?>().Use(now)

                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.IdentifierColumn)
                    .Use(() => GetRandomStringWithLengthOf(10))

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
    }
}
