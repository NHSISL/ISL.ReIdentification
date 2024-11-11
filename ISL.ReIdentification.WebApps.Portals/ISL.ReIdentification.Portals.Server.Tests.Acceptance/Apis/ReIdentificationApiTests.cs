// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Brokers;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.Accesses;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.ImpersonationContexts;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.OdsDatas;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.PdsDatas;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.ReIdentifications;
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

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static DateTimeOffset GetRandomPastDateTimeOffset()
        {
            DateTime now = DateTimeOffset.UtcNow.Date;
            int randomDaysInPast = GetRandomNegativeNumber();
            DateTime pastDateTime = now.AddDays(randomDaysInPast).Date;

            return new DateTimeRange(earliestDate: pastDateTime, latestDate: now).GetValue();
        }

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

        private async ValueTask<OdsData> PostRandomOdsDataAsync()
        {
            OdsData randomOdsData = CreateRandomOdsData();

            return await this.apiBroker.PostOdsDataAsync(randomOdsData);
        }

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
                .OnType<DateTimeOffset>().Use(GetRandomPastDateTimeOffset())
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default)
                .OnProperty(odsData => odsData.OrganisationCode).Use(GetRandomStringWithLengthOf(5))
                .OnProperty(odsData => odsData.OrganisationName).Use(GetRandomStringWithLengthOf(5))
                .OnProperty(odsData => odsData.OdsHierarchy).Use(hierarchyId.ToString());

            return filler;
        }

        private async ValueTask<PdsData> PostPdsDataAsync(string orgCode, string orgName)
        {
            PdsData randomPdsData = CreateRandomPdsData(orgCode, orgName);

            return await this.apiBroker.PostPdsDataAsync(randomPdsData);
        }

        private static PdsData CreateRandomPdsData(string orgCode, string orgName) =>
            CreatePdsDataFiller(dateTimeOffset: GetRandomDateTimeOffset(), orgCode, orgName).Create();

        private static PdsData CreateRandomPdsData(DateTimeOffset dateTimeOffset, string orgCode, string orgName) =>
            CreatePdsDataFiller(dateTimeOffset, orgCode, orgName).Create();

        private static Filler<PdsData> CreatePdsDataFiller(DateTimeOffset dateTimeOffset, string orgCode, string orgName)
        {
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<PdsData>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomPastDateTimeOffset())
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default)
                .OnProperty(pdsData => pdsData.PseudoNhsNumber).Use(GetRandomStringWithLengthOf(9))
                .OnProperty(pdsData => pdsData.OrgCode).Use(orgCode)
                .OnProperty(pdsData => pdsData.OrganisationName).Use(orgName);

            return filler;
        }

        private static UserAccess CreateRandomUserAccess(string orgCode, Guid entraUserId) =>
            CreateRandomUserAccessFiller(orgCode, entraUserId).Create();

        private static Filler<UserAccess> CreateRandomUserAccessFiller(string orgCode, Guid? entraUserId)
        {
            if (entraUserId is null || entraUserId == Guid.Empty)
            {
                entraUserId = Guid.NewGuid();
            }

            string user = Guid.NewGuid().ToString();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<UserAccess>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnType<DateTimeOffset?>().Use(now)

                .OnProperty(userAccess => userAccess.EntraUserId)
                    .Use(entraUserId.Value)

                .OnProperty(userAccess => userAccess.GivenName)
                    .Use(() => GetRandomStringWithLengthOf(255))

                .OnProperty(userAccess => userAccess.Surname)
                    .Use(() => GetRandomStringWithLengthOf(255))

                .OnProperty(userAccess => userAccess.Email)
                    .Use(() => GetRandomStringWithLengthOf(320))

                .OnProperty(userAccess => userAccess.OrgCode).Use(orgCode)
                .OnProperty(userAccess => userAccess.CreatedDate).Use(now)
                .OnProperty(userAccess => userAccess.CreatedBy).Use(user)
                .OnProperty(userAccess => userAccess.UpdatedDate).Use(now)
                .OnProperty(userAccess => userAccess.UpdatedBy).Use(user);

            return filler;
        }

        private async ValueTask<UserAccess> PostRandomUserAccessAsync(string orgCode, Guid entraUserId)
        {
            UserAccess randomUserAccess = CreateRandomUserAccess(orgCode, entraUserId);

            return await this.apiBroker.PostUserAccessAsync(randomUserAccess);
        }

        private static AccessRequest CreateIdentificationRequestAccessRequestGivenPsuedoId(
            Guid entraUserId, string pseudoId)
        {
            return new AccessRequest
            {
                IdentificationRequest = CreateRandomIdentificationRequest(entraUserId, pseudoId)
            };
        }

        private static AccessRequest CreateImpersonationContextAccessRequest() =>
            new AccessRequest
            {
                ImpersonationContext = CreateRandomImpersonationContext()
            };

        private static IdentificationRequest CreateRandomIdentificationRequest(Guid entraUserId, string pseudoId) =>
            CreateIdentificationRequestFiller(entraUserId, pseudoId).Create();

        private static Filler<IdentificationRequest> CreateIdentificationRequestFiller(
            Guid entraUserId, string pseudoId)
        {
            var filler = new Filler<IdentificationRequest>();

            filler.Setup()
                .OnProperty(request => request.EntraUserId).Use(entraUserId)
                .OnProperty(request => request.IdentificationItems).Use(CreateRandomIdentificationItems(pseudoId));

            return filler;
        }

        private static List<IdentificationItem> CreateRandomIdentificationItems(string pseudoId)
        {
            List<IdentificationItem> identificationItems = new List<IdentificationItem>();
            IdentificationItem hasAccessIdentificationItem = CreateIdentificationItemFiller(pseudoId).Create();
            identificationItems.Add(hasAccessIdentificationItem);
            IdentificationItem noAccessIdentificationItem = CreateIdentificationItemFiller(String.Empty).Create();
            identificationItems.Add(noAccessIdentificationItem);

            return identificationItems;
        }

        private static Filler<IdentificationItem> CreateIdentificationItemFiller(string psuedoNhsNumber)
        {
            if (psuedoNhsNumber == string.Empty)
            {
                psuedoNhsNumber = GetRandomStringWithLengthOf(10);
            }

            var filler = new Filler<IdentificationItem>();

            filler.Setup()
                .OnProperty(item => item.Identifier).Use(psuedoNhsNumber);

            return filler;
        }

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
    }
}
