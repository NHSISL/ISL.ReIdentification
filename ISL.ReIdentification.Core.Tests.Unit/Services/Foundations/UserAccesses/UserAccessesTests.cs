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
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Foundations.UserAccesses;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.UserAccesses
{
    public partial class UserAccessesTests
    {
        private readonly Mock<IReIdentificationStorageBroker> reIdentificationStorageBroker;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ISecurityBroker> securityBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly UserAccessService userAccessService;
        private readonly ICompareLogic compareLogic;

        public UserAccessesTests()
        {
            this.reIdentificationStorageBroker = new Mock<IReIdentificationStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.securityBrokerMock = new Mock<ISecurityBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.compareLogic = new CompareLogic();

            this.userAccessService = new UserAccessService(
                reIdentificationStorageBroker: reIdentificationStorageBroker.Object,
                dateTimeBroker: dateTimeBrokerMock.Object,
                securityBroker: securityBrokerMock.Object,
                loggingBroker: loggingBrokerMock.Object);
        }

        private Expression<Func<UserAccess, bool>> SameUserAccessAs(UserAccess expectedUserAccess)
        {
            return actualUserAccess =>
                this.compareLogic.Compare(expectedUserAccess, actualUserAccess)
                    .AreEqual;
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static UserAccess CreateRandomUserAccess() =>
            CreateRandomUserAccess(dateTimeOffset: GetRandomDateTimeOffset());

        private static UserAccess CreateRandomUserAccess(DateTimeOffset dateTimeOffset) =>
            CreateUserAccessesFiller(dateTimeOffset).Create();

        private static List<UserAccess> CreateRandomUserAccesses()
        {
            return CreateUserAccessesFiller(GetRandomDateTimeOffset())
                .Create(GetRandomNumber())
                .ToList();
        }

        private static List<UserAccess> CreateUserAccesses(int count = 0)
        {
            if (count == 0)
            {
                count = GetRandomNumber();
            }

            return Enumerable.Range(start: 1, count)
                .Select(number => CreateRandomUserAccess()).ToList();
        }

        private static DateTime? RandomReturnOfDateTimeOffsetOrNull(DateTimeOffset randomDateTimeOffset)
        {
            Random random = new Random();
            bool returnNull = random.Next(2) == 0;

            if (returnNull)
            {
                return null;
            }
            else
            {
                return DateTime.Now.AddDays(random.Next(30));
            }
        }

        private static List<OdsData> CreateRandomOdsDatasByOrgCode(
            string orgCode,
            DateTimeOffset dateTimeOffset,
            int childrenCount)
        {
            OdsData futureOdsData = CreateRandomOdsData(GetRandomString());
            futureOdsData.RelationshipWithParentStartDate = dateTimeOffset.AddDays(GetRandomNumber());
            futureOdsData.RelationshipWithParentEndDate = dateTimeOffset.AddDays(GetRandomNumber());
            OdsData pastOdsData = CreateRandomOdsData(GetRandomString());
            pastOdsData.RelationshipWithParentStartDate = dateTimeOffset.AddDays(-GetRandomNumber());
            pastOdsData.RelationshipWithParentEndDate = dateTimeOffset.AddDays(-1);
            OdsData currentActiveOdsData = CreateRandomOdsData(orgCode);
            currentActiveOdsData.RelationshipWithParentStartDate = dateTimeOffset.AddDays(-GetRandomNumber());

            currentActiveOdsData.RelationshipWithParentEndDate =
                RandomReturnOfDateTimeOffsetOrNull(dateTimeOffset.AddDays(GetRandomNumber()));

            List<OdsData> parentOdsDatas = new List<OdsData> {
                futureOdsData,
                pastOdsData,
                currentActiveOdsData,
            };

            List<OdsData> allOdsDatas = new List<OdsData>();
            allOdsDatas.AddRange(parentOdsDatas);

            for (var index = 0; index < parentOdsDatas.Count; index++)
            {
                parentOdsDatas[index].OdsHierarchy = HierarchyId.Parse($"/{index}/");

                List<OdsData> childOdsDatas = CreateRandomOdsDataChildren(
                    parentHierarchyId: parentOdsDatas[index].OdsHierarchy,
                    dateTimeOffset,
                    count: childrenCount);

                allOdsDatas.AddRange(childOdsDatas);
            }

            return allOdsDatas;
        }

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static string GetRandomStringWithLength(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(max: 10, min: 2).GetValue();

        private SqlException CreateSqlException() =>
            (SqlException)RuntimeHelpers.GetUninitializedObject(type: typeof(SqlException));

        private static UserAccess CreateRandomModifyUserAccess(DateTimeOffset dateTimeOffset)
        {
            int randomDaysInThePast = GetRandomNegativeNumber();
            UserAccess randomUserAccess = CreateRandomUserAccess(dateTimeOffset);
            randomUserAccess.CreatedDate = dateTimeOffset.AddDays(randomDaysInThePast);

            return randomUserAccess;
        }

        private EntraUser CreateRandomEntraUser()
        {
            return new EntraUser(
                entraUserId: Guid.NewGuid(),
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

        private static Filler<EntraUser> CreateRandomEntraUserFiller()
        {
            var filler = new Filler<EntraUser>();
            filler.Setup();

            return filler;
        }

        private static Filler<UserAccess> CreateUserAccessesFiller(DateTimeOffset dateTimeOffset)
        {
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<UserAccess>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)
                .OnProperty(userAccess => userAccess.CreatedBy).Use(user)
                .OnProperty(userAccess => userAccess.UpdatedBy).Use(user);

            return filler;
        }

        private static List<OdsData> CreateRandomOdsDataChildren(
            HierarchyId parentHierarchyId,
            DateTimeOffset randomDateTimeOffset,
            int count = 0)
        {
            if (parentHierarchyId == null)
            {
                parentHierarchyId = HierarchyId.Parse("/");
            }

            if (count == 0)
            {
                count = GetRandomNumber();
            }

            List<OdsData> children = CreateOdsDataFiller(organisationCode: GetRandomString())
                .Create(count)
                    .ToList();

            HierarchyId lastChildHierarchy = null;

            foreach (var child in children)
            {
                child.OdsHierarchy = parentHierarchyId.GetDescendant(lastChildHierarchy, null);
                lastChildHierarchy = child.OdsHierarchy;
                child.RelationshipWithParentStartDate = randomDateTimeOffset.AddDays(-GetRandomNumber());
                child.RelationshipWithParentEndDate = randomDateTimeOffset.AddDays(GetRandomNumber());
            }

            return children;
        }

        private static List<OdsData> CreateRandomOdsDatas(List<string> organisationCodes)
        {
            List<OdsData> odsDatas = new List<OdsData>();

            foreach (var organisationCode in organisationCodes)
            {
                odsDatas.Add(CreateRandomOdsData(organisationCode));
            }

            return odsDatas;
        }

        private static OdsData CreateRandomOdsData(string organisationCode) =>
            CreateOdsDataFiller(organisationCode).Create();

        private static Filler<OdsData> CreateOdsDataFiller(
            string organisationCode, HierarchyId hierarchyId = null)
        {
            string user = Guid.NewGuid().ToString();
            DateTimeOffset dateTimeOffset = GetRandomDateTimeOffset();
            var filler = new Filler<OdsData>();

            if (hierarchyId == null)
            {
                hierarchyId = HierarchyId.Parse("/");
            }

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default)
                .OnProperty(odsData => odsData.OrganisationCode).Use(organisationCode)
                .OnProperty(odsData => odsData.OrganisationName).Use(GetRandomStringWithLengthOf(30))
                .OnProperty(odsData => odsData.OdsHierarchy).Use(hierarchyId);

            return filler;
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(
            Xeption expectedException)
        {
            return actualException =>
                actualException.SameExceptionAs(expectedException);
        }
    }
}