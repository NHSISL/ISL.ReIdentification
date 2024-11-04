// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ISL.Providers.Notifications.Abstractions.Models.Exceptions;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Notifications;
using ISL.ReIdentification.Core.Models.Brokers.Notifications;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Foundations.Notifications;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Notifications
{
    public partial class NotificationTests
    {
        private readonly Mock<INotificationBroker> notificationBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly NotificationConfigurations notificationConfigurations;
        private readonly NotificationService notificationService;
        private readonly CompareLogic compareLogic;

        public NotificationTests()
        {
            this.notificationBrokerMock = new Mock<INotificationBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.compareLogic = new CompareLogic();

            this.notificationConfigurations = new NotificationConfigurations
            {
                CsvPendingApprovalRequestTemplateId = GetRandomString(),
                CsvApprovedRequestTemplateId = GetRandomString(),
                CsvDeniedRequestTemplateId = GetRandomString(),
                ConfigurationBaseUrl = GetRandomString(),
                PortalBaseUrl = GetRandomString()
            };

            this.notificationService = new NotificationService(
                notificationConfigurations: notificationConfigurations,
                notificationBroker: notificationBrokerMock.Object,
                loggingBroker: loggingBrokerMock.Object);
        }
        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static AccessRequest CreateRandomCsvAccessRequest() =>
            new AccessRequest { CsvIdentificationRequest = CreateRandomCsvIdentificationRequest() };

        private static CsvIdentificationRequest CreateRandomCsvIdentificationRequest() =>
            CreateCsvIdentificationRequestFiller().Create();

        private static Filler<CsvIdentificationRequest> CreateCsvIdentificationRequestFiller()
        {
            string user = Guid.NewGuid().ToString();
            DateTimeOffset dateTimeOffset = GetRandomDateTimeOffset();
            var filler = new Filler<CsvIdentificationRequest>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)

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

        private Expression<Func<Dictionary<string, dynamic>, bool>> SameDictionaryAs(
            Dictionary<string, dynamic> expectedDictionary)
        {
            return actualDictionary =>
                this.compareLogic.Compare(expectedDictionary, actualDictionary)
                    .AreEqual;
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            var innerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new NotificationProviderValidationException(
                    message: GetRandomString(),
                    innerException,
                    data: innerException.Data),
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string randomMessage = GetRandomString();
            var innerException = new Xeption(message: GetRandomString());

            return new TheoryData<Xeption>
            {
                new NotificationProviderDependencyException(
                    message: randomMessage,
                    innerException,
                    data: innerException.Data),

                new NotificationProviderServiceException(
                    message: randomMessage,
                    innerException,
                    data: innerException.Data)
            };
        }
    }
}
