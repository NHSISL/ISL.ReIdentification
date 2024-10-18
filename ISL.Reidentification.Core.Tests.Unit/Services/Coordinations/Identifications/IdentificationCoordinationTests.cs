// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using ISL.ReIdentification.Core.Brokers.CsvHelpers;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Securities;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Identifications.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Persists.Exceptions;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Orchestrations.Identifications;
using ISL.ReIdentification.Core.Services.Orchestrations.Persists;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        private readonly Mock<IAccessOrchestrationService> accessOrchestrationServiceMock;
        private readonly Mock<ICsvHelperBroker> csvHelperBrokerMock;
        private readonly Mock<ISecurityBroker> securityBrokerMock;
        private readonly Mock<IPersistanceOrchestrationService> persistanceOrchestrationServiceMock;
        private readonly Mock<IIdentificationOrchestrationService> identificationOrchestrationServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IdentificationCoordinationService identificationCoordinationService;
        private readonly ICompareLogic compareLogic;

        public IdentificationCoordinationTests()
        {
            this.accessOrchestrationServiceMock = new Mock<IAccessOrchestrationService>();
            this.csvHelperBrokerMock = new Mock<ICsvHelperBroker>();
            this.securityBrokerMock = new Mock<ISecurityBroker>();
            this.persistanceOrchestrationServiceMock = new Mock<IPersistanceOrchestrationService>();
            this.identificationOrchestrationServiceMock = new Mock<IIdentificationOrchestrationService>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.compareLogic = new CompareLogic();

            this.identificationCoordinationService = new IdentificationCoordinationService(
                this.accessOrchestrationServiceMock.Object,
                this.persistanceOrchestrationServiceMock.Object,
                this.identificationOrchestrationServiceMock.Object,
                this.csvHelperBrokerMock.Object,
                this.securityBrokerMock.Object,
                this.loggingBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(max: 15, min: 2).GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            return new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length)
                .GetValue();
        }

        private static AccessRequest CreateRandomAccessRequest() =>
            CreateAccessRequestFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static Filler<AccessRequest> CreateAccessRequestFiller(DateTimeOffset dateTimeOffset)
        {
            var filler = new Filler<AccessRequest>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default)
                .OnProperty(request => request.ImpersonationContext).Use(CreateRandomImpersonationContext());

            return filler;
        }

        private static IdentificationRequest CreateRandomIdentificationRequest() =>
            CreateIdentificationRequestFiller().Create();

        private static Filler<IdentificationRequest> CreateIdentificationRequestFiller() =>
            new Filler<IdentificationRequest>();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static ImpersonationContext CreateRandomImpersonationContext() =>
            CreateRandomImpersonationContext(dateTimeOffset: GetRandomDateTimeOffset());

        private static ImpersonationContext CreateRandomImpersonationContext(DateTimeOffset dateTimeOffset) =>
            CreateImpersonationContextsFiller(dateTimeOffset).Create();

        private static Filler<ImpersonationContext> CreateImpersonationContextsFiller(DateTimeOffset dateTimeOffset)
        {
            var filler = new Filler<ImpersonationContext>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default);

            return filler;
        }

        private static CsvIdentificationRequest CreateRandomCsvIdentificationRequest() =>
            CreateRandomCsvIdentificationRequest(dateTimeOffset: GetRandomDateTimeOffset());

        private static CsvIdentificationRequest CreateRandomCsvIdentificationRequest(DateTimeOffset dateTimeOffset) =>
            CreateCsvIdentificationRequestFiller(dateTimeOffset).Create();

        private static Filler<CsvIdentificationRequest> CreateCsvIdentificationRequestFiller(DateTimeOffset dateTimeOffset)
        {
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<CsvIdentificationRequest>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<DateTimeOffset?>().Use(dateTimeOffset)

                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.IdentifierColumn)
                    .Use(() => GetRandomStringWithLengthOf(10))

                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.CreatedBy).Use(user)
                .OnProperty(csvIdentificationRequest => csvIdentificationRequest.UpdatedBy).Use(user);

            return filler;
        }

        private static EntraUser CreateRandomEntraUser()
        {
            Guid randomId = Guid.NewGuid();
            string randomString = GetRandomString();

            EntraUser entraUser = new EntraUser(
                entraUserId: randomId,
                givenName: randomString,
                surname: randomString,
                displayName: randomString,
                email: randomString,
                jobTitle: randomString,
                roles: new List<string> { randomString },
                claims: CreateRandomClaims());

            return entraUser;
        }

        private static List<Claim> CreateRandomClaims()
        {
            string randomString = GetRandomString();

            return Enumerable.Range(start: 1, count: GetRandomNumber())
                .Select(_ => new Claim(type: randomString, value: randomString)).ToList();
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
                new AccessOrchestrationValidationException(
                    message: "Access orchestration validation errors occured, please try again.",
                    innerException),

                new AccessOrchestrationDependencyValidationException(
                    message: "Access orchestration dependency validation occurred, please try again.",
                    innerException),

                new PersistanceOrchestrationValidationException(
                    message: "Persistance orchestration validation errors occured, please try again.",
                    innerException),

                new PersistanceOrchestrationDependencyValidationException(
                    message: "Persistance orchestration dependency validation occurred, please try again.",
                    innerException),

                new IdentificationOrchestrationValidationException(
                    message: "Identification orchestration validation errors occurred, please try again.",
                    innerException),

                new IdentificationOrchestrationDependencyValidationException(
                    message: "Identification orchestration dependency validation occurred, please try again.",
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
                new AccessOrchestrationDependencyException(
                    message: "Access orchestration dependency error occurred, please contact support.",
                    innerException),

                new AccessOrchestrationServiceException(
                    message: "Access orchestration service error occurred, please contact support.",
                    innerException),

                new PersistanceOrchestrationDependencyException(
                    message: "Persistance orchestration dependency error occurred, please contact support.",
                    innerException),

                new PersistanceOrchestrationServiceException(
                    message: "Persistance orchestration service error occurred, please contact support.",
                    innerException),

                new IdentificationOrchestrationDependencyException(
                    message: "Identification orchestration dependency error occurred, please contact support.",
                    innerException),

                new IdentificationOrchestrationServiceException(
                    message: "Identification orchestration service error occurred, please contact support.",
                    innerException),
            };
        }
    }
}
