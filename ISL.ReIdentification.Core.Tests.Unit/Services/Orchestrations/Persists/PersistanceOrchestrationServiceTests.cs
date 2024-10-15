// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq.Expressions;
using ISL.ReIdentification.Core.Brokers.Loggings;
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
        private readonly PersistanceOrchestrationService persistanceOrchestrationService;

        public PersistanceOrchestrationServiceTests()
        {
            this.impersonationContextServiceMock = new Mock<IImpersonationContextService>();
            this.csvIdentificationRequestServiceMock = new Mock<ICsvIdentificationRequestService>();
            this.notificationServiceMock = new Mock<INotificationService>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.persistanceOrchestrationService =
                new PersistanceOrchestrationService(
                    impersonationContextService: impersonationContextServiceMock.Object,
                    csvIdentificationRequestService: csvIdentificationRequestServiceMock.Object,
                    notificationService: notificationServiceMock.Object,
                    loggingBroker: loggingBrokerMock.Object);
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static AccessRequest CreateRandomAccessRequest() =>
            CreateAccessRequestFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static Filler<AccessRequest> CreateAccessRequestFiller(DateTimeOffset dateTimeOffset)
        {
            var filler = new Filler<AccessRequest>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default);

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
