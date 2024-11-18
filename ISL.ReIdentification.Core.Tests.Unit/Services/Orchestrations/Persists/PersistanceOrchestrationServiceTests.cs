// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Hashing;
using ISL.ReIdentification.Core.Brokers.Identifiers;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.Notifications.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Persists;
using ISL.ReIdentification.Core.Services.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Services.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Services.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Services.Foundations.Notifications;
using ISL.ReIdentification.Core.Services.Orchestrations.Persists;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Persists
{
    public partial class PersistanceOrchestrationServiceTests
    {
        private readonly Mock<IImpersonationContextService> impersonationContextServiceMock;
        private readonly Mock<ICsvIdentificationRequestService> csvIdentificationRequestServiceMock;
        private readonly Mock<INotificationService> notificationServiceMock;
        private readonly Mock<IAccessAuditService> accessAuditServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly Mock<IHashBroker> hashBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<IIdentifierBroker> identifierBrokerMock;
        private readonly CsvReIdentificationConfigurations csvReIdentificationConfigurations;
        private readonly PersistanceOrchestrationService persistanceOrchestrationService;
        private readonly ICompareLogic compareLogic;
        private static readonly int expireAfterDays = 7;

        public PersistanceOrchestrationServiceTests()
        {
            this.impersonationContextServiceMock = new Mock<IImpersonationContextService>();
            this.csvIdentificationRequestServiceMock = new Mock<ICsvIdentificationRequestService>();
            this.notificationServiceMock = new Mock<INotificationService>();
            this.accessAuditServiceMock = new Mock<IAccessAuditService>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.hashBrokerMock = new Mock<IHashBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.identifierBrokerMock = new Mock<IIdentifierBroker>();
            this.compareLogic = new CompareLogic();

            this.csvReIdentificationConfigurations = new CsvReIdentificationConfigurations
            {
                ExpireAfterDays = expireAfterDays
            };



            this.persistanceOrchestrationService =
                new PersistanceOrchestrationService(
                    impersonationContextService: impersonationContextServiceMock.Object,
                    csvIdentificationRequestService: csvIdentificationRequestServiceMock.Object,
                    notificationService: notificationServiceMock.Object,
                    accessAuditService: accessAuditServiceMock.Object,
                    loggingBroker: loggingBrokerMock.Object,
                    hashBroker: hashBrokerMock.Object,
                    dateTimeBroker: dateTimeBrokerMock.Object,
                    identifierBroker: identifierBrokerMock.Object,
                    csvReIdentificationConfigurations: csvReIdentificationConfigurations);
        }

        private static int GetRandomNumber() =>
            new IntRange(max: 15, min: 2).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static AccessRequest CreateRandomAccessRequest() =>
            CreateAccessRequestFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static ImpersonationContext CreateRandomImpersonationContext() =>
            CreateRandomImpersonationContext(dateTimeOffset: GetRandomDateTimeOffset());

        private static ImpersonationContext CreateRandomImpersonationContext(DateTimeOffset dateTimeOffset) =>
            CreateImpersonationContextsFiller(dateTimeOffset).Create();

        private static Filler<AccessRequest> CreateAccessRequestFiller(DateTimeOffset dateTimeOffset)
        {
            var filler = new Filler<AccessRequest>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default);

            return filler;
        }

        private static CsvIdentificationRequest CreateRandomCsvIdentificationRequest() =>
            CreateCsvIdentificationRequestFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static List<CsvIdentificationRequest> CreateRandomCsvIdentificationRequests()
        {
            List<CsvIdentificationRequest> csvIdentificationRequests =
                CreateCsvIdentificationRequestFiller(DateTimeOffset.UtcNow).Create(GetRandomNumber()).ToList();

            return csvIdentificationRequests;
        }

        private static List<CsvIdentificationRequest> CreateRandomExpiredCsvIdentificationRequests()
        {
            List<CsvIdentificationRequest> expiredCsvIdentificationRequests =
                CreateExpiredCsvIdentificationRequestFiller().Create(GetRandomNumber()).ToList();

            return expiredCsvIdentificationRequests;
        }

        private static Filler<CsvIdentificationRequest> CreateCsvIdentificationRequestFiller(DateTimeOffset dateTimeOffset)
        {
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<CsvIdentificationRequest>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default)

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

        private static Filler<CsvIdentificationRequest> CreateExpiredCsvIdentificationRequestFiller()
        {
            var expiredDays = expireAfterDays + GetRandomNumber();
            var expiredDateTimeOffset = DateTimeOffset.UtcNow.AddDays(expiredDays * -1);
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<CsvIdentificationRequest>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(expiredDateTimeOffset)
                .OnType<DateTimeOffset?>().Use(expiredDateTimeOffset)

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

        private static AccessAudit CreateRandomPurgedAccessAudit(Guid accessAuditId, Guid requestId, DateTimeOffset now) =>
            CreateRandomPurgedAccessAuditFiller(accessAuditId, requestId, now).Create();

        private static Filler<AccessAudit> CreateRandomPurgedAccessAuditFiller(
            Guid accessAuditId,
            Guid requestId,
            DateTimeOffset now)
        {
            string user = "System";
            Guid entraUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            string purgedValue = "PURGED";
            string message = $"Purged on {now}";
            var filler = new Filler<AccessAudit>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)

                .OnProperty(accessAudit => accessAudit.Id).Use(accessAuditId)
                .OnProperty(accessAudit => accessAudit.RequestId).Use(requestId)
                .OnProperty(accessAudit => accessAudit.PseudoIdentifier).Use(purgedValue)
                .OnProperty(accessAudit => accessAudit.EntraUserId).Use(entraUserId)
                .OnProperty(accessAudit => accessAudit.GivenName).Use(purgedValue)
                .OnProperty(accessAudit => accessAudit.Surname).Use(purgedValue)
                .OnProperty(accessAudit => accessAudit.Email).Use(purgedValue)
                .OnProperty(accessAudit => accessAudit.Purpose).Use(purgedValue)
                .OnProperty(accessAudit => accessAudit.Reason).Use(purgedValue)
                .OnProperty(accessAudit => accessAudit.Organisation).Use(purgedValue)
                .OnProperty(accessAudit => accessAudit.HasAccess).Use(false)
                .OnProperty(accessAudit => accessAudit.Message).Use(message)
                .OnProperty(accessAudit => accessAudit.CreatedBy).Use(user)
                .OnProperty(accessAudit => accessAudit.UpdatedBy).Use(user);

            return filler;
        }

        private static Filler<ImpersonationContext> CreateImpersonationContextsFiller(DateTimeOffset dateTimeOffset)
        {
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<ImpersonationContext>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)

                .OnProperty(impersonationContext => impersonationContext.RequesterEmail)
                    .Use(GetRandomStringWithLengthOf(320))

                .OnProperty(impersonationContext => impersonationContext.ResponsiblePersonEmail)
                    .Use(GetRandomStringWithLengthOf(320))

                .OnProperty(impersonationContext => impersonationContext.Organisation)
                    .Use(GetRandomStringWithLengthOf(255))

                .OnProperty(impersonationContext => impersonationContext.ProjectName)
                    .Use(GetRandomStringWithLengthOf(255))

                .OnProperty(impersonationContext => impersonationContext.IdentifierColumn)
                    .Use(GetRandomStringWithLengthOf(9))

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

        private Expression<Func<CsvIdentificationRequest, bool>> SameCsvIdentificationRequestAs(
            CsvIdentificationRequest expectedCsvIdentificationRequest) =>
                actualCsvIdentificationRequest => this.compareLogic
                    .Compare(expectedCsvIdentificationRequest, actualCsvIdentificationRequest).AreEqual;

        private Expression<Func<AccessAudit, bool>> SameAccessAuditAs(
            AccessAudit expectedAccessAudit) =>
                actualAccessAudit => this.compareLogic
                    .Compare(expectedAccessAudit, actualAccessAudit).AreEqual;

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new ImpersonationContextValidationException(
                    message: "Impersonation context validation errors occured, please try again",
                    innerException),

                new ImpersonationContextDependencyValidationException(
                    message: "Impersonation context dependency validation occurred, please try again.",
                    innerException),

                new CsvIdentificationRequestValidationException(
                    message: "CSV identification request validation error occurred, please fix errors and try again.",
                    innerException),

                new CsvIdentificationRequestDependencyValidationException(
                    message: "CSV identification request dependency validation error occurred, " +
                        "fix errors and try again.",
                    innerException),

                new NotificationValidationException(
                    message: "Notification validation error occurred, please fix errors and try again.",
                    innerException),

                new NotificationDependencyValidationException(
                    message: "Notification dependency validation error occurred, fix errors and try again.",
                    innerException),
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new ImpersonationContextDependencyException(
                    message: "Impersonation context dependency error occurred, please contact support.",
                    innerException),

                new ImpersonationContextServiceException(
                    message: "Impersonation context service error occurred, please contact support.",
                    innerException),

                new CsvIdentificationRequestDependencyException(
                    message: "CSV identification request dependency error occurred, please contact support.",
                    innerException),

                new CsvIdentificationRequestServiceException(
                    message: "CSV identification request service error occurred, please contact support.",
                    innerException),

                new NotificationDependencyException(
                    message: "Notification dependency error occurred, please contact support.",
                    innerException),

                new NotificationServiceException(
                    message: "Notification service error occurred, please contact support.",
                    innerException),
            };
        }

        public static TheoryData<Xeption> PurgeDependencyValidationExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new CsvIdentificationRequestValidationException(
                    message: "CSV identification request validation error occurred, please fix errors and try again.",
                    innerException),

                new CsvIdentificationRequestDependencyValidationException(
                    message: "CSV identification request dependency validation error occurred, " +
                        "fix errors and try again.",
                    innerException),

                new AccessAuditValidationException(
                    message: "Access audit validation error occurred, please fix errors and try again.",
                    innerException),

                new AccessAuditDependencyValidationException(
                    message: "Access audit dependency validation error occurred, " +
                        "fix errors and try again.",
                    innerException),
            };
        }
    }
}
