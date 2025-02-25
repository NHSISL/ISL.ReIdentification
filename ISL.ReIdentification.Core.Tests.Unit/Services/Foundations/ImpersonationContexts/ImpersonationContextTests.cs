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
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Foundations.ImpersonationContexts;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.ImpersonationContexts
{
    public partial class ImpersonationContextsTests
    {
        private readonly Mock<IReIdentificationStorageBroker> reIdentificationStorageBroker;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ISecurityBroker> securityBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IImpersonationContextService impersonationContextService;

        public ImpersonationContextsTests()
        {
            this.reIdentificationStorageBroker = new Mock<IReIdentificationStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.securityBrokerMock = new Mock<ISecurityBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.impersonationContextService = new ImpersonationContextService(
                reIdentificationStorageBroker.Object,
                dateTimeBrokerMock.Object,
                securityBrokerMock.Object,
                loggingBrokerMock.Object);
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static ImpersonationContext CreateRandomImpersonationContext() =>
            CreateImpersonationContextsFiller(
                dateTimeOffset: GetRandomDateTimeOffset(),
                impersonationContextId: GetRandomStringWithLengthOf(255)).Create();

        private static ImpersonationContext CreateRandomImpersonationContext(
            DateTimeOffset dateTimeOffset, string impersonationContextId) =>
                CreateImpersonationContextsFiller(dateTimeOffset, impersonationContextId).Create();

        private static IQueryable<ImpersonationContext> CreateRandomImpersonationContexts()
        {
            return CreateImpersonationContextsFiller(
                dateTimeOffset: GetRandomDateTimeOffset(),
                impersonationContextId: GetRandomStringWithLengthOf(255))
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static ImpersonationContext CreateRandomModifyImpersonationContext(
            DateTimeOffset dateTimeOffset, string impersonationContextId)
        {
            int randomDaysInThePast = GetRandomNegativeNumber();

            ImpersonationContext randomImpersonationContext = CreateRandomImpersonationContext(
                dateTimeOffset, 
                impersonationContextId);

            randomImpersonationContext.CreatedDate = 
                randomImpersonationContext.CreatedDate.AddDays(randomDaysInThePast);

            return randomImpersonationContext;
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static int GetRandomNumber() =>
            new IntRange(max: 15, min: 2).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private SqlException CreateSqlException() =>
            (SqlException)RuntimeHelpers.GetUninitializedObject(type: typeof(SqlException));

        private static Filler<ImpersonationContext> CreateImpersonationContextsFiller(
            DateTimeOffset dateTimeOffset, string impersonationContextId)
        {
            var filler = new Filler<ImpersonationContext>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)

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

                .OnProperty(impersonationContext => impersonationContext.CreatedBy).Use(impersonationContextId)
                .OnProperty(impersonationContext => impersonationContext.UpdatedBy).Use(impersonationContextId);

            return filler;
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException)
        {
            return actualException =>
                actualException.SameExceptionAs(expectedException);
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