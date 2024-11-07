// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Brokers;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.Accesses;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.CsvIdentificationRequests;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.ImpersonationContexts;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.OdsDatas;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.PdsDatas;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.UserAccesses;
using Microsoft.EntityFrameworkCore;
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

        private static string GetRandomEmail()
        {
            string randomPrefix = GetRandomStringWithLengthOf(15);
            string emailSuffix = "@email.com";

            return randomPrefix + emailSuffix;
        }

        private static AccessRequest CreateImpersonationContextAccessRequest() =>
            new AccessRequest
            {
                ImpersonationContext = CreateRandomImpersonationContext()
            };

        private static ImpersonationContext CreateRandomImpersonationContext() =>
           CreateRandomImpersonationContextFiller().Create();

        private static ImpersonationContext UpdateImpersonationContextWithRandomValues(
            ImpersonationContext inputImpersonationContext)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var updatedImpersonationContext = CreateRandomImpersonationContext();
            updatedImpersonationContext.Id = inputImpersonationContext.Id;
            updatedImpersonationContext.CreatedDate = inputImpersonationContext.CreatedDate;
            updatedImpersonationContext.CreatedBy = inputImpersonationContext.CreatedBy;
            updatedImpersonationContext.UpdatedDate = now;

            return updatedImpersonationContext;
        }

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
                    .Use(() => GetRandomEmail())

                .OnProperty(impersonationContext => impersonationContext.ResponsiblePersonEmail)
                    .Use(() => GetRandomEmail())

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

                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.RequesterEmail)
                    .Use(GetRandomEmail())

                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.RecipientEmail)
                    .Use(GetRandomEmail())

                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.Organisation)
                    .Use(GetRandomStringWithLengthOf(255))

                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.CreatedBy).Use(user)
                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.UpdatedBy).Use(user);

            return filler;
        }

        private async ValueTask<List<UserAccess>> PostRandomUserAccesses(Guid entraUserId)
        {
            int randomNumber = GetRandomNumber();
            List<UserAccess> randomUserAccesses = new List<UserAccess>();

            for (int i = 0; i < randomNumber; i++)
            {
                randomUserAccesses.Add(await this.PostRandomUserAccess(entraUserId));
            }

            return randomUserAccesses;
        }

        private async ValueTask<UserAccess> PostRandomUserAccess(Guid entraUserId)
        {
            UserAccess randomUserAccess = CreateRandomUserAccess(entraUserId);

            return await this.apiBroker.PostUserAccessAsync(randomUserAccess);
        }

        private static UserAccess CreateRandomUserAccess(Guid entraUserId) =>
            CreateRandomUserAccessFiller(entraUserId).Create();

        private static Filler<UserAccess> CreateRandomUserAccessFiller(Guid entraUserId)
        {
            string user = Guid.NewGuid().ToString();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<UserAccess>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnType<DateTimeOffset?>().Use(now)

                .OnProperty(userAccess => userAccess.EntraUserId)
                    .Use(entraUserId)

                .OnProperty(userAccess => userAccess.GivenName)
                    .Use(() => GetRandomStringWithLengthOf(255))

                .OnProperty(userAccess => userAccess.Surname)
                    .Use(() => GetRandomStringWithLengthOf(255))

                .OnProperty(userAccess => userAccess.Email)
                    .Use(() => GetRandomStringWithLengthOf(320))

                .OnProperty(userAccess => userAccess.CreatedDate).Use(now)
                .OnProperty(userAccess => userAccess.CreatedBy).Use(user)
                .OnProperty(userAccess => userAccess.UpdatedDate).Use(now)
                .OnProperty(userAccess => userAccess.UpdatedBy).Use(user);

            return filler;
        }

        private async ValueTask<OdsData> PostRandomOdsData(string orgCode)
        {
            OdsData randomOdsData = CreateRandomOdsData(orgCode);

            return await this.apiBroker.PostOdsDataAsync(randomOdsData);
        }

        private static OdsData CreateRandomOdsData(string orgCode) =>
            CreateOdsDataFiller(orgCode).Create();

        private static Filler<OdsData> CreateOdsDataFiller(string orgCode, HierarchyId hierarchyId = null)
        {
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<OdsData>();

            if (hierarchyId == null)
            {
                hierarchyId = HierarchyId.Parse("/");
            }

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default)
                .OnProperty(odsData => odsData.OrganisationCode).Use(orgCode)
                .OnProperty(odsData => odsData.OrganisationName).Use(GetRandomStringWithLengthOf(5))
                .OnProperty(odsData => odsData.OdsHierarchy).Use(hierarchyId.ToString());

            return filler;
        }
        private async ValueTask<PdsData> PostRandomPdsDataAsync(string pseudoNhsNumber, string orgCode)
        {
            PdsData randomPdsData = CreateRandomPdsData(pseudoNhsNumber, orgCode);

            return await this.apiBroker.PostPdsDataAsync(randomPdsData);
        }

        private static PdsData CreateRandomPdsData(string pseudoNhsNumber, string orgCode) =>
            CreatePdsDataFiller(pseudoNhsNumber, orgCode).Create();

        private static Filler<PdsData> CreatePdsDataFiller(string pseudoNhsNumber, string orgCode)
        {
            string user = Guid.NewGuid().ToString();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<PdsData>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default)
                .OnProperty(pdsData => pdsData.PseudoNhsNumber).Use(pseudoNhsNumber)
                .OnProperty(pdsData => pdsData.OrgCode).Use(orgCode);

            return filler;
        }
    }
}
