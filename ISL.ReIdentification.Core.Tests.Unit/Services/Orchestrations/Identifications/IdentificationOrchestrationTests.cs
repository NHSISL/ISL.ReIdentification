// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Identifiers;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.Documents;
using ISL.ReIdentification.Core.Models.Foundations.Documents.Exceptions;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Services.Foundations.Documents;
using ISL.ReIdentification.Core.Services.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Services.Orchestrations.Identifications;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit.Abstractions;

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
        private readonly ProjectStorageConfiguration projectStorageConfiguration;
        private readonly IIdentificationOrchestrationService identificationOrchestrationService;
        private readonly ICompareLogic compareLogic;
        private readonly ITestOutputHelper testOutputHelper;

        public IdentificationOrchestrationTests(ITestOutputHelper testOutputHelper)
        {
            this.reIdentificationServiceMock = new Mock<IReIdentificationService>();
            this.accessAuditServiceMock = new Mock<IAccessAuditService>();
            this.documentServiceMock = new Mock<IDocumentService>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.identifierBrokerMock = new Mock<IIdentifierBroker>();
            this.projectStorageConfiguration = CreateRandomProjectStorageConfiguration();
            this.compareLogic = new CompareLogic();
            this.testOutputHelper = testOutputHelper;

            this.identificationOrchestrationService = new IdentificationOrchestrationService(
                this.reIdentificationServiceMock.Object,
                this.accessAuditServiceMock.Object,
                this.documentServiceMock.Object,
                this.loggingBrokerMock.Object,
                this.dateTimeBrokerMock.Object,
                this.identifierBrokerMock.Object,
                this.projectStorageConfiguration);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static string GetRandomStringWithLength(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static List<string> GetRandomStringList()
        {
            int randomNumber = GetRandomNumber();
            List<string> randomStringList = new List<string>();

            for (int index = 0; index < randomNumber; index++)
            {
                string randomString = GetRandomStringWithLength(randomNumber);
                randomStringList.Add(randomString);
            }

            return randomStringList;
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

        private static AccessRequest CreateRandomAccessRequest() =>
            CreateAccessRequestFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static Filler<AccessRequest> CreateAccessRequestFiller(DateTimeOffset dateTimeOffset)
        {
            var filler = new Filler<AccessRequest>();

            filler.Setup()
                .OnProperty(accessRequest => accessRequest.ImpersonationContext)
                    .Use(CreateRandomImpersonationContext())

                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default);

            return filler;
        }

        private static ImpersonationContext CreateRandomImpersonationContext() =>
            CreateRandomImpersonationContext(dateTimeOffset: GetRandomDateTimeOffset());

        private static ImpersonationContext CreateRandomImpersonationContext(DateTimeOffset dateTimeOffset) =>
            CreateImpersonationContextsFiller(dateTimeOffset).Create();

        private static Filler<ImpersonationContext> CreateImpersonationContextsFiller(DateTimeOffset dateTimeOffset)
        {
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<ImpersonationContext>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)

                .OnProperty(impersonationContext => impersonationContext.RequesterEmail)
                    .Use(GetRandomStringWithLength(320))

                .OnProperty(impersonationContext => impersonationContext.ResponsiblePersonEmail)
                    .Use(GetRandomStringWithLength(320))

                .OnProperty(impersonationContext => impersonationContext.Organisation)
                    .Use(GetRandomStringWithLength(255))

                .OnProperty(impersonationContext => impersonationContext.ProjectName)
                    .Use(GetRandomStringWithLength(255))

                .OnProperty(impersonationContext => impersonationContext.IdentifierColumn)
                    .Use(GetRandomStringWithLength(9))

                .OnProperty(impersonationContext => impersonationContext.IsApproved)
                    .Use(true)

                .OnProperty(impersonationContext => impersonationContext.CreatedBy).Use(user)
                .OnProperty(impersonationContext => impersonationContext.UpdatedBy).Use(user);

            return filler;
        }

        private static ProjectStorageConfiguration CreateRandomProjectStorageConfiguration() =>
            CreateProjectStorageConfigurationFiller().Create();

        private static Filler<ProjectStorageConfiguration> CreateProjectStorageConfigurationFiller() =>
            new Filler<ProjectStorageConfiguration>();

        private static List<AccessPolicy> GetAccessPolicies(
            string inboxPolicyname,
            string outboxPolicyname,
            string errorsPolicyname) =>
            new List<AccessPolicy>
            {
                new AccessPolicy
                {
                    PolicyName = inboxPolicyname,
                    Permissions = new List<string>{ "read", "list"}
                },
                new AccessPolicy
                {
                    PolicyName = outboxPolicyname,
                    Permissions = new List<string>{ "write", "add", "create", "read", "list" }
                },
                new AccessPolicy
                {
                    PolicyName = errorsPolicyname,
                    Permissions = new List<string>{ "read", "list"}
                },
            };

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

        private Expression<Func<Stream, bool>> SameStreamAs(Stream expectedStream) =>
            actualStream => this.compareLogic.Compare(expectedStream, actualStream).AreEqual;

        private Expression<Func<List<AccessPolicy>, bool>> SameAccessPolicyListAs(
            List<AccessPolicy> expectedAccessPolicyList) =>
                actualAccessPolicyList => this.compareLogic
                    .Compare(expectedAccessPolicyList, actualAccessPolicyList).AreEqual;

        private static byte[] ReadAllBytesFromStream(Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public class HasLengthStream : MemoryStream
        {
            public override long Length => 1;
        }

        public class ZeroLengthStream : MemoryStream
        {
            public override long Length => 0;
        }

        public static TheoryData<Stream, string> InvalidArgumentsStreamIsNull()
        {
            Stream stream = new ZeroLengthStream();

            return new TheoryData<Stream, string>
            {
                { null, null },
                { null, "" },
                { null, " " }
            };
        }

        public static TheoryData<Stream, string> InvalidArgumentsStreamHasLength()
        {
            Stream stream = new HasLengthStream();

            return new TheoryData<Stream, string>
            {
                { null, null },
                { stream, ""},
                { stream, " " }
            };
        }

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

        public static TheoryData<Xeption> DocumentDependencyExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new DocumentDependencyException(
                    message: "Document dependency error occurred, please contact support.",
                    innerException),

                new DocumentServiceException(
                    message: "Document service error occurred, please contact support.",
                    innerException),
            };
        }
    }
}
