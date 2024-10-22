// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Notifications;
using ISL.ReIdentification.Core.Models.Brokers.Notifications;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Foundations.Notifications;
using Moq;
using Tynamix.ObjectFiller;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Notifications
{
    public partial class NotificationTests
    {
        private readonly Mock<INotificationBroker> notificationBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly NotificationConfigurations notificationConfigurations;
        private readonly NotificationService notificationService;

        public NotificationTests()
        {
            this.notificationBrokerMock = new Mock<INotificationBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.notificationConfigurations = new NotificationConfigurations
            {
                CsvPendingApprovalRequestTemplateId = GetRandomString(),
                CsvApprovedRequestTemplateId = GetRandomString(),
                CsvDeniedRequestTemplateId = GetRandomString()
            };

            this.notificationService = new NotificationService(
                notificationConfigurations: notificationConfigurations,
                notificationBroker: notificationBrokerMock.Object,
                loggingBroker: loggingBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

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
                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.CreatedBy).Use(user)
                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.UpdatedBy).Use(user);

            return filler;
        }
    }
}
