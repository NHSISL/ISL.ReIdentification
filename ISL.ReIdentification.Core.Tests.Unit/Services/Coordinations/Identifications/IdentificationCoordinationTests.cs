﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using ISL.ReIdentification.Core.Brokers.CsvHelpers;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Securities;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Identifications.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Persists.Exceptions;
using ISL.ReIdentification.Core.Models.Securities;
using ISL.ReIdentification.Core.Services.Coordinations.Identifications;
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
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly IdentificationCoordinationService identificationCoordinationService;
        private readonly ProjectStorageConfiguration projectStorageConfiguration;
        private readonly ICompareLogic compareLogic;

        public IdentificationCoordinationTests()
        {
            this.accessOrchestrationServiceMock = new Mock<IAccessOrchestrationService>();
            this.csvHelperBrokerMock = new Mock<ICsvHelperBroker>();
            this.securityBrokerMock = new Mock<ISecurityBroker>();
            this.persistanceOrchestrationServiceMock = new Mock<IPersistanceOrchestrationService>();
            this.identificationOrchestrationServiceMock = new Mock<IIdentificationOrchestrationService>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.compareLogic = new CompareLogic();

            this.projectStorageConfiguration = new ProjectStorageConfiguration
            {
                Container = GetRandomString(),
                LandingFolder = GetRandomString(),
                PickupFolder = GetRandomString(),
                ErrorFolder = GetRandomString()
            };

            this.identificationCoordinationService = new IdentificationCoordinationService(
                accessOrchestrationService: this.accessOrchestrationServiceMock.Object,
                persistanceOrchestrationService: this.persistanceOrchestrationServiceMock.Object,
                identificationOrchestrationService: this.identificationOrchestrationServiceMock.Object,
                csvHelperBroker: this.csvHelperBrokerMock.Object,
                securityBroker: this.securityBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                projectStorageConfiguration);
        }

        private IdentificationRequest HydrateAccessRequestIdentificationRequest(
            AccessRequest accessRequest)
        {
            IdentificationRequest identificationRequest = new IdentificationRequest();
            identificationRequest.Id = accessRequest.CsvIdentificationRequest.Id;
            identificationRequest.EntraUserId = accessRequest.CsvIdentificationRequest.RecipientEntraUserId;
            identificationRequest.Email = accessRequest.CsvIdentificationRequest.RecipientEmail;
            identificationRequest.JobTitle = accessRequest.CsvIdentificationRequest.RecipientJobTitle;
            identificationRequest.DisplayName = accessRequest.CsvIdentificationRequest.RecipientDisplayName;
            identificationRequest.GivenName = accessRequest.CsvIdentificationRequest.RecipientFirstName;
            identificationRequest.Surname = accessRequest.CsvIdentificationRequest.RecipientLastName;
            identificationRequest.Reason = accessRequest.CsvIdentificationRequest.Reason;
            identificationRequest.Organisation = accessRequest.CsvIdentificationRequest.Organisation;

            return identificationRequest;
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(max: 15, min: 2).GetValue();

        private static int GetRandomNumber(int max, int min) =>
            new IntRange(max: max, min: min).GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static AccessRequest CreateRandomAccessRequest() =>
            CreateAccessRequestFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static AccessRequest CreateRandomCsvIdentificationRequestAccessRequest()
        {
            AccessRequest accessRequest = CreateAccessRequestFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();
            accessRequest.IdentificationRequest = null;
            accessRequest.ImpersonationContext = null;

            return accessRequest;
        }

        private static AccessRequest CreateRandomIdentificationRequestAccessRequest()
        {
            AccessRequest accessRequest = CreateAccessRequestFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();
            accessRequest.CsvIdentificationRequest = null;
            accessRequest.ImpersonationContext = null;

            return accessRequest;
        }

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

        private static DateTimeOffset GetRandomFutureDateTimeOffset()
        {
            DateTime futureStartDate = DateTimeOffset.UtcNow.AddDays(1).Date;
            int randomDaysInFuture = GetRandomNumber();
            DateTime futureEndDate = futureStartDate.AddDays(randomDaysInFuture).Date;

            return new DateTimeRange(earliestDate: futureStartDate, latestDate: futureEndDate).GetValue();
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
                .OnType<DateTimeOffset?>().Use((DateTimeOffset?)default)

                .OnProperty(impersonationContext => impersonationContext.RequesterEmail)
                    .Use(GetRandomStringWithLengthOf(320))

                .OnProperty(impersonationContext => impersonationContext.ResponsiblePersonEmail)
                    .Use(GetRandomStringWithLengthOf(320))

                .OnProperty(impersonationContext => impersonationContext.Organisation)
                    .Use(GetRandomStringWithLengthOf(255))

                .OnProperty(impersonationContext => impersonationContext.ProjectName)
                    .Use(GetRandomStringWithLengthOf(255))

                .OnProperty(impersonationContext => impersonationContext.IdentifierColumn)
                    .Use(GetRandomStringWithLengthOf(10))

                .OnProperty(impersonationContext => impersonationContext.CreatedBy).Use(user)
                .OnProperty(impersonationContext => impersonationContext.UpdatedBy).Use(user);


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

        private static Filler<CsvIdentificationItem> CreateCsvIdentificationItemFiller() =>
            new Filler<CsvIdentificationItem>();

        private static Filler<IdentificationItem> CreateIdentificationItemFiller() =>
            new Filler<IdentificationItem>();

        private static EntraUser CreateRandomEntraUser()
        {
            string randomId = GetRandomStringWithLengthOf(255);
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

        private static IQueryable<CsvIdentificationItem> CreateRandomCsvIdentificationItems(int count = 0)
        {
            if (count == 0)
            {
                count = GetRandomNumber();
            }

            IQueryable<CsvIdentificationItem> csvIdentificationItems =
                CreateCsvIdentificationItemFiller().Create(count).AsQueryable();

            return csvIdentificationItems;
        }

        private static IQueryable<IdentificationItem> CreateRandomIdentificationItems(int count = 0)
        {
            if (count == 0)
            {
                count = GetRandomNumber();
            }

            IQueryable<IdentificationItem> identificationItems =
                CreateIdentificationItemFiller().Create(count).AsQueryable();

            return identificationItems;
        }

        private static string CsvDataString() =>
            "Um93TnVtYmVyLElkZW50aWZpZXIKVGVzdFJvd051bWJlcixUZXN0SWRlbnRpZmllcg==";

        private static List<Claim> CreateRandomClaims()
        {
            string randomString = GetRandomString();

            return Enumerable.Range(start: 1, count: GetRandomNumber())
                .Select(_ => new Claim(type: randomString, value: randomString)).ToList();
        }

        private static List<dynamic> CreateRandomDynamicObjects(int count = 0)
        {
            List<dynamic> dynamicObjects = new List<dynamic>();

            if (count == 0)
            {
                count = GetRandomNumber();
            }

            for (int i = 0; i < count; i++)
            {
                dynamic obj = new ExpandoObject();
                obj.Property1 = $"value{i}";
                obj.Property2 = i;
                obj.Identifier = GetRandomString();
                dynamicObjects.Add(obj);
            }

            return dynamicObjects;
        }

        private AccessRequest ConvertImpersonationContextToCsvIdentificationRequest(AccessRequest accessRequest)
        {
            accessRequest.CsvIdentificationRequest = new CsvIdentificationRequest();
            accessRequest.CsvIdentificationRequest.HasHeaderRecord = true;
            accessRequest.CsvIdentificationRequest.IdentifierColumnIndex = 2;
            accessRequest.CsvIdentificationRequest.Id = accessRequest.ImpersonationContext.Id;

            accessRequest.CsvIdentificationRequest.RecipientEntraUserId =
                accessRequest.ImpersonationContext.RequesterEntraUserId;

            accessRequest.CsvIdentificationRequest.RecipientEmail =
                accessRequest.ImpersonationContext.RequesterEmail;

            accessRequest.CsvIdentificationRequest.RecipientJobTitle =
                accessRequest.ImpersonationContext.RequesterJobTitle;

            accessRequest.CsvIdentificationRequest.RecipientDisplayName =
                accessRequest.ImpersonationContext.RequesterDisplayName;

            accessRequest.CsvIdentificationRequest.RecipientFirstName =
                accessRequest.ImpersonationContext.RequesterFirstName;

            accessRequest.CsvIdentificationRequest.RecipientLastName =
                accessRequest.ImpersonationContext.RequesterLastName;

            accessRequest.CsvIdentificationRequest.Reason = accessRequest.ImpersonationContext.Reason;
            accessRequest.CsvIdentificationRequest.Organisation = accessRequest.ImpersonationContext.Organisation;

            return accessRequest;
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private Expression<Func<Stream, bool>> SameStreamAs(Stream expectedStream) =>
            actualStream => this.compareLogic.Compare(expectedStream, actualStream).AreEqual;

        private Expression<Func<AccessRequest, bool>> SameAccessRequestAs(AccessRequest expectedAccessRequest) =>
            actualAccessRequest => this.compareLogic.Compare(expectedAccessRequest, actualAccessRequest).AreEqual;

        private Expression<Func<IdentificationRequest, bool>> SameIdentificationRequestAs(
            IdentificationRequest expectedIdentificationRequest) =>
                actualIdentificationRequest => this.compareLogic.Compare(
                    expectedIdentificationRequest,
                    actualIdentificationRequest).AreEqual;

        private Expression<Func<string, bool>> SameStringAs(string expectedString) =>
            actualString => this.compareLogic.Compare(expectedString, actualString).AreEqual;

        private Expression<Func<List<dynamic>, bool>> SameDynamicListAs(List<dynamic> expectedDynamicList) =>
            actualDynamicList => this.compareLogic.Compare(expectedDynamicList, actualDynamicList).AreEqual;

        private Expression<Func<List<CsvIdentificationItem>, bool>> SameCsvIdentificationItemListAs(
            List<CsvIdentificationItem> expectedList) =>
                actualList => this.compareLogic.Compare(expectedList, actualList).AreEqual;

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
