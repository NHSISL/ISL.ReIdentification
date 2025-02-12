// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Securities;
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Foundations.OdsDatas;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.OdsDatas
{
    public partial class OdsDataServiceTests
    {
        private readonly Mock<IReIdentificationStorageBroker> reIdentificationStorageBroker;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ISecurityBroker> securityBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IOdsDataService odsDataService;

        public OdsDataServiceTests()
        {
            this.reIdentificationStorageBroker = new Mock<IReIdentificationStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.securityBrokerMock = new Mock<ISecurityBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.odsDataService = new OdsDataService(
                reIdentificationStorageBroker: this.reIdentificationStorageBroker.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                securityBroker: this.securityBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private SqlException CreateSqlException() =>
            (SqlException)RuntimeHelpers.GetUninitializedObject(type: typeof(SqlException));

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static OdsData CreateRandomModifyOdsData(DateTimeOffset dateTimeOffset , string odsId)
        {
            int randomDaysInPast = GetRandomNegativeNumber();
            OdsData randomOdsData = CreateRandomOdsData(dateTimeOffset, odsId);

            return randomOdsData;
        }

        private static List<OdsData> CreateRandomOdsDatas()
        {
            return CreateOdsDataFiller(
                dateTimeOffset: GetRandomDateTimeOffset(),
                odsId: GetRandomStringWithLengthOf(255))
                .Create(count: GetRandomNumber())
                    .ToList();
        }

        private static List<OdsData> CreateRandomOdsDataChildren(HierarchyId parentHierarchyId, int count = 0)
        {
            if (parentHierarchyId == null)
            {
                parentHierarchyId = HierarchyId.Parse("/");
            }

            if (count == 0)
            {
                count = GetRandomNumber();
            }

            List<OdsData> children = CreateOdsDataFiller(
                dateTimeOffset: GetRandomDateTimeOffset(),
                odsId: GetRandomStringWithLengthOf(255))
                .Create(count)
                    .ToList();

            HierarchyId lastChildHierarchy = null;

            foreach (var child in children)
            {
                child.OdsHierarchy = parentHierarchyId.GetDescendant(lastChildHierarchy, null);
                lastChildHierarchy = child.OdsHierarchy;
            }

            return children;
        }

        private static OdsData CreateRandomOdsData() =>
            CreateOdsDataFiller(
                dateTimeOffset: GetRandomDateTimeOffset(),
                odsId: GetRandomStringWithLengthOf(255)).Create();

        private static OdsData CreateRandomParentOdsData() =>
            CreateOdsDataFiller(
                dateTimeOffset: GetRandomDateTimeOffset(),
                odsId: GetRandomStringWithLengthOf(255)).Create();

        private static OdsData CreateRandomOdsData(DateTimeOffset dateTimeOffset, string odsId) =>
            CreateOdsDataFiller(dateTimeOffset, odsId).Create();

        private static Filler<OdsData> CreateOdsDataFiller(
            DateTimeOffset dateTimeOffset, string odsId ,HierarchyId hierarchyId = null)
        {
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<OdsData>();

            if (hierarchyId == null)
            {
                hierarchyId = HierarchyId.Parse("/");
            }

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default)
                .OnProperty(odsData => odsData.OrganisationCode).Use(GetRandomStringWithLengthOf(15))
                .OnProperty(odsData => odsData.OrganisationName).Use(GetRandomStringWithLengthOf(30))
                .OnProperty(odsData => odsData.OdsHierarchy).Use(hierarchyId)
                .OnProperty(odsData => odsData.RelationshipWithParentStartDate).Use(dateTimeOffset)
                .OnProperty(odsData => odsData.RelationshipWithParentEndDate).Use((dateTimeOffset));

            return filler;
        }

        private EntraUser CreateRandomEntraUser(string entraUserId = "")
        {
            var userId = string.IsNullOrWhiteSpace(entraUserId) ? GetRandomStringWithLengthOf(255) : entraUserId;

            return new EntraUser(
                entraUserId: userId,
                givenName: GetRandomString(),
                surname: GetRandomString(),
                displayName: GetRandomString(),
                email: GetRandomString(),
                jobTitle: GetRandomString(),
                roles: new List<string> { GetRandomString() },

                claims: new List<System.Security.Claims.Claim>
                {
                    new System.Security.Claims.Claim(type: GetRandomString(), value: GetRandomString())
                });
        }
    }
}