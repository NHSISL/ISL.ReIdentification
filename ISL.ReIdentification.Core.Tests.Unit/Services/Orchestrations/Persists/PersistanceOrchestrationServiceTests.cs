﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq.Expressions;
using ISL.ReIdentification.Core.Brokers.Hashing;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.Notifications.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Services.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Services.Foundations.Notifications;
using ISL.ReIdentification.Core.Services.Orchestrations.Persists;
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
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly Mock<IHashBroker> hashBrokerMock;
        private readonly PersistanceOrchestrationService persistanceOrchestrationService;

        public PersistanceOrchestrationServiceTests()
        {
            this.impersonationContextServiceMock = new Mock<IImpersonationContextService>();
            this.csvIdentificationRequestServiceMock = new Mock<ICsvIdentificationRequestService>();
            this.notificationServiceMock = new Mock<INotificationService>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.hashBrokerMock = new Mock<IHashBroker>();

            this.persistanceOrchestrationService =
                new PersistanceOrchestrationService(
                    impersonationContextService: impersonationContextServiceMock.Object,
                    csvIdentificationRequestService: csvIdentificationRequestServiceMock.Object,
                    notificationService: notificationServiceMock.Object,
                    loggingBroker: loggingBrokerMock.Object,
                    hashBroker: hashBrokerMock.Object);
        }

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

        private static Filler<CsvIdentificationRequest> CreateCsvIdentificationRequestFiller(DateTimeOffset dateTimeOffset)
        {
            var filler = new Filler<CsvIdentificationRequest>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default);

            return filler;
        }

        private static Filler<ImpersonationContext> CreateImpersonationContextsFiller(DateTimeOffset dateTimeOffset)
        {
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<ImpersonationContext>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)

                .OnProperty(impersonationContext => impersonationContext.IdentifierColumn)
                    .Use(() => GetRandomStringWithLengthOf(10))

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
    }
}
