// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Foundations.PdsDatas;
using ISL.ReIdentification.Core.Services.Foundations.UserAccesses;
using ISL.ReIdentification.Core.Services.Orchestrations.Accesses;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Accesses
{
    public partial class AccessOrchestrationServiceTests
    {
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<IUserAccessService> userAccessServiceMock;
        private readonly Mock<IPdsDataService> pdsDataServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly AccessOrchestrationService accessOrchestrationService;

        public AccessOrchestrationServiceTests()
        {
            this.userAccessServiceMock = new Mock<IUserAccessService>();
            this.pdsDataServiceMock = new Mock<IPdsDataService>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.accessOrchestrationService =
                new AccessOrchestrationService(
                    userAccessServiceMock.Object,
                    pdsDataServiceMock.Object,
                    dateTimeBrokerMock.Object,
                    loggingBrokerMock.Object);
        }

        private static DateTimeOffset GetRandomPastDateTimeOffset()
        {
            DateTime now = DateTimeOffset.UtcNow.Date;
            int randomDaysInPast = GetRandomNegativeNumber();
            DateTime pastDateTime = now.AddDays(randomDaysInPast).Date;

            return new DateTimeRange(earliestDate: pastDateTime, latestDate: now).GetValue();
        }

        private static DateTimeOffset GetRandomFutureDateTimeOffset()
        {
            DateTime futureStartDate = DateTimeOffset.UtcNow.AddDays(1).Date;
            int randomDaysInFuture = GetRandomNumber();
            DateTime futureEndDate = futureStartDate.AddDays(randomDaysInFuture).Date;

            return new DateTimeRange(earliestDate: futureStartDate, latestDate: futureEndDate).GetValue();
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static PdsData CreateRandomPdsData(DateTimeOffset dateTimeOffset) =>
            CreatePdsDataFiller(dateTimeOffset).Create();

        private static UserAccess CreateRandomUserAccess(DateTimeOffset dateTimeOffset) =>
            CreateUserAccessesFiller(dateTimeOffset).Create();

        private static List<IdentificationItem> CreateRandomIdentificationItems()
        {
            return CreateIdentificationItemFiller()
                .Create(GetRandomNumber())
                .ToList();
        }

        private static IdentificationRequest CreateRandomIdentificationRequest() =>
            CreateIdentificationRequestFiller().Create();

        private static AccessRequest CreateRandomAccessRequest() =>
            CreateAccessRequestFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static ImpersonationContext CreateRandomImpersonationContext() =>
            CreateRandomImpersonationContext(dateTimeOffset: GetRandomDateTimeOffset());

        private static ImpersonationContext CreateRandomImpersonationContext(DateTimeOffset dateTimeOffset) =>
            CreateImpersonationContextsFiller(dateTimeOffset).Create();

        private static int GetRandomNumber() =>
            new IntRange(max: 15, min: 2).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static string GetRandomStringWithLength(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static Filler<OdsData> CreateOdsDataFiller(DateTimeOffset dateTimeOffset)
        {
            var filler = new Filler<OdsData>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset);

            return filler;
        }

        private static Filler<PdsData> CreatePdsDataFiller(DateTimeOffset dateTimeOffset)
        {
            var filler = new Filler<PdsData>();

            filler.Setup()
                .OnType<DateTimeOffset?>().Use(dateTimeOffset);

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

        private static Filler<IdentificationItem> CreateIdentificationItemFiller()
        {
            var filler = new Filler<IdentificationItem>();

            filler.Setup()
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default)
                .OnProperty(identificationItem => identificationItem.HasAccess).Use(false);

            return filler;
        }

        private static Filler<IdentificationRequest> CreateIdentificationRequestFiller()
        {
            var filler = new Filler<IdentificationRequest>();

            filler.Setup()
                .OnProperty(identificationRequest => identificationRequest.IdentificationItems)
                    .Use(CreateRandomIdentificationItems());

            return filler;
        }

        private static Filler<AccessRequest> CreateAccessRequestFiller(DateTimeOffset dateTimeOffset)
        {
            var filler = new Filler<AccessRequest>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default)
                .OnProperty(accessRequest => accessRequest.IdentificationRequest)
                    .Use(CreateRandomIdentificationRequest())
                .OnProperty(accessRequest => accessRequest.ImpersonationContext)
                    .Use(CreateRandomImpersonationContext());

            return filler;
        }

        private static Filler<ImpersonationContext> CreateImpersonationContextsFiller(DateTimeOffset dateTimeOffset)
        {
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<ImpersonationContext>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default)

                .OnProperty(impersonationContext => impersonationContext.IdentifierColumn)
                    .Use(() => GetRandomStringWithLength(10))

                .OnProperty(impersonationContext => impersonationContext.CreatedBy).Use(user)
                .OnProperty(impersonationContext => impersonationContext.UpdatedBy).Use(user);

            return filler;
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(
            Xeption expectedException)
        {
            return actualException =>
                actualException.SameExceptionAs(expectedException);
        }

        public static IEnumerable<object[]> GetTestInvalidParameters()
        {
            yield return new object[] { null, null };
            yield return new object[] { "", new List<string> { "" } };
            yield return new object[] { " ", new List<string> { " " } };
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new UserAccessValidationException(
                    message: "User access validation errors occured, please try again",
                    innerException),

                new UserAccessDependencyValidationException(
                    message: "User access dependency validation occurred, please try again.",
                    innerException),

                new PdsDataValidationException(
                    message: "Pds data validation errors occurred, please try again.",
                    innerException)
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new UserAccessDependencyException(
                    message: "User access dependency error occurred, please contact support.",
                    innerException),

                new UserAccessServiceException(
                    message: "User access service error occurred, please contact support.",
                    innerException),

                new PdsDataDependencyException(
                    message: "Pds data dependency error occurred, please contact support.",
                    innerException),

                new PdsDataServiceException(
                    message: "Pds data service error occurred, please contact support.",
                    innerException),
            };
        }

        public static TheoryData<UserAccess> GetOrganisationsReturnsOrgs()
        {
            UserAccess randomUserAccess = CreateRandomUserAccess(GetRandomPastDateTimeOffset());
            UserAccess nullActiveToUserAccess = randomUserAccess.DeepClone();
            UserAccess futureActiveToUserAccess = randomUserAccess.DeepClone();
            nullActiveToUserAccess.ActiveTo = null;
            futureActiveToUserAccess.ActiveTo = GetRandomFutureDateTimeOffset();

            return new TheoryData<UserAccess>
            {
                nullActiveToUserAccess,
                futureActiveToUserAccess
            };
        }

        public static TheoryData<UserAccess, bool> GetOrganisationsReturnsNoOrgs()
        {
            UserAccess randomUserAccess = CreateRandomUserAccess(GetRandomPastDateTimeOffset());
            UserAccess futureActiveFromUserAccess = randomUserAccess.DeepClone();
            UserAccess pastActiveToUserAccess = randomUserAccess.DeepClone();
            UserAccess differentEntraUserIdUserAccess = randomUserAccess.DeepClone();
            futureActiveFromUserAccess.ActiveFrom = GetRandomFutureDateTimeOffset();
            pastActiveToUserAccess.ActiveTo = GetRandomPastDateTimeOffset();
            differentEntraUserIdUserAccess.ActiveTo = null;

            return new TheoryData<UserAccess, bool>
            {
                { pastActiveToUserAccess, false },
                { futureActiveFromUserAccess, false },
                { differentEntraUserIdUserAccess, true }
            };
        }

        public static TheoryData<PdsData, string> UserHasAccessToPatientTrue()
        {
            string randomString = GetRandomString();
            string organisation = randomString;
            PdsData randomPdsData = CreateRandomPdsData(GetRandomPastDateTimeOffset());
            PdsData nullEffectiveToPdsData = randomPdsData.DeepClone();
            PdsData futureEffectiveToPdsData = randomPdsData.DeepClone();
            PdsData basePdsData = randomPdsData.DeepClone();
            PdsData currentCcgOfRegistrationPdsData = basePdsData.DeepClone();
            PdsData currentIcbOfRegistration = basePdsData.DeepClone();
            PdsData icbOfRegistration = basePdsData.DeepClone();

            return new TheoryData<PdsData, string>
            {
                { nullEffectiveToPdsData, organisation },
                { futureEffectiveToPdsData, organisation },
                { currentCcgOfRegistrationPdsData, organisation },
                { currentIcbOfRegistration, organisation },
                { icbOfRegistration, organisation }
            };
        }

        public static TheoryData<PdsData, string, bool> UserHasAccessToPatientFalse()
        {
            string randomString = GetRandomString();
            string organisation = randomString;
            PdsData randomPdsData = CreateRandomPdsData(GetRandomPastDateTimeOffset());
            PdsData differentIdentifierPdsData = randomPdsData.DeepClone();
            PdsData pastEffectiveToPdsData = randomPdsData.DeepClone();
            PdsData futureEffectiveFromPdsData = randomPdsData.DeepClone();
            PdsData notRegisteredPdsData = randomPdsData.DeepClone();
            string differentOrganisation = GetRandomString();

            return new TheoryData<PdsData, string, bool>
            {
                { differentIdentifierPdsData, organisation, true },
                { pastEffectiveToPdsData, organisation, false },
                { futureEffectiveFromPdsData, organisation, false },
                { notRegisteredPdsData, organisation, false }
            };
        }
    }
}
