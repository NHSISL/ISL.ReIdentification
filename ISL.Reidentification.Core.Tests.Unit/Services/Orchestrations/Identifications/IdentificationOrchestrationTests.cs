// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Identifiers;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.Documents.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications.Exceptions;
using ISL.ReIdentification.Core.Services.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Services.Foundations.Documents;
using ISL.ReIdentification.Core.Services.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Services.Orchestrations.Identifications;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Identifications
{
    public partial class IdentificationOrchestrationTests
    {
        private readonly Mock<IReIdentificationService> reIdentificationServiceMock;
        private readonly Mock<IAccessAuditService> accessAuditServiceMock;
        private readonly Mock<IDocumentService> documentServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<IIdentifierBroker> identifierBrokerMock;
        private readonly IIdentificationOrchestrationService identificationOrchestrationService;
        private readonly ICompareLogic compareLogic;

        public IdentificationOrchestrationTests()
        {
            this.reIdentificationServiceMock = new Mock<IReIdentificationService>();
            this.accessAuditServiceMock = new Mock<IAccessAuditService>();
            this.documentServiceMock = new Mock<IDocumentService>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.identifierBrokerMock = new Mock<IIdentifierBroker>();
            this.compareLogic = new CompareLogic();

            this.identificationOrchestrationService = new IdentificationOrchestrationService(
                this.reIdentificationServiceMock.Object,
                this.accessAuditServiceMock.Object,
                this.documentServiceMock.Object,
                this.loggingBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.identifierBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static string GetRandomStringWithLength(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static int GetRandomNumber() =>
            new IntRange(max: 15, min: 2).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static IdentificationRequest CreateRandomIdentificationRequest(bool hasAccess, int itemCount)
        {
            IdentificationRequest randomIdentificationRequest = CreateIdentificationRequestFiller().Create();

            List<IdentificationItem> randomIdentificationItem =
                CreateRandomIdentificationItems(hasAccess, count: itemCount);

            randomIdentificationRequest.IdentificationItems = randomIdentificationItem;

            return randomIdentificationRequest;
        }

        private static Filler<IdentificationRequest> CreateIdentificationRequestFiller() =>
            new Filler<IdentificationRequest>();

        private static List<IdentificationItem> CreateRandomIdentificationItems(bool hasAccess, int count)
        {
            return CreateIdentificationItemFiller(hasAccess)
                .Create(count)
                    .ToList();
        }

        private static Filler<IdentificationItem> CreateIdentificationItemFiller(bool hasAccess)
        {
            var filler = new Filler<IdentificationItem>();

            filler.Setup()
                .OnProperty(item => item.HasAccess).Use(hasAccess);

            return filler;
        }

        private Expression<Func<AccessAudit, bool>> SameAccessAuditAs(
          AccessAudit expectedAccessAudit)
        {
            return actualAccessAudit =>
                this.compareLogic.Compare(expectedAccessAudit, actualAccessAudit)
                    .AreEqual;
        }

        private Expression<Func<IdentificationRequest, bool>> SameIdentificationRequestAs(
          IdentificationRequest expectedIdentificationRequest)
        {
            return actualIdentificationRequest =>
                this.compareLogic.Compare(expectedIdentificationRequest, actualIdentificationRequest)
                    .AreEqual;
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new AccessAuditValidationException(
                    message: "Access audit validation errors occured, please try again",
                    innerException),

                new AccessAuditDependencyValidationException(
                    message: "Access audit dependency validation occurred, please try again.",
                    innerException),

                new ReIdentificationValidationException(
                    message: "ReIdentification validation errors occurred, please try again.",
                    innerException),

                new ReIdentificationDependencyValidationException(
                    message: "ReIdentification dependency validation occurred, please try again.",
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
                new AccessAuditDependencyException(
                    message: "Access audit dependency error occurred, please contact support.",
                    innerException),

                new AccessAuditServiceException(
                    message: "Access audit service error occurred, please contact support.",
                    innerException),

                new ReIdentificationDependencyException(
                    message: "ReIdentification dependency error occurred, please contact support.",
                    innerException),

                new ReIdentificationServiceException(
                    message: "ReIdentification service error occurred, please contact support.",
                    innerException),
            };
        }

        public static TheoryData<Xeption> DocumentDependencyValidationExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new DocumentValidationException(
                    message: "Document validation errors occured, please try again",
                    innerException),

                new DocumentDependencyValidationException(
                    message: "Document dependency validation occurred, please try again.",
                    innerException),
            };
        }
    }
}
