// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ISL.ReIdentification.Core.Brokers.Identifiers;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.NECS;
using ISL.ReIdentification.Core.Models.Brokers.NECS;
using ISL.ReIdentification.Core.Models.Brokers.NECS.Requests;
using ISL.ReIdentification.Core.Models.Brokers.NECS.Responses;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using ISL.ReIdentification.Core.Services.Foundations.ReIdentifications;
using KellermanSoftware.CompareNetObjects;
using Moq;
using RESTFulSense.Exceptions;
using Tynamix.ObjectFiller;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.ReIdentifications
{
    public partial class ReIdentificationServiceTests
    {
        private readonly Mock<INECSBroker> necsBrokerMock = new Mock<INECSBroker>();
        private readonly Mock<IIdentifierBroker> identifierBrokerMock = new Mock<IIdentifierBroker>();
        private readonly Mock<ILoggingBroker> loggingBrokerMock = new Mock<ILoggingBroker>();
        private NECSConfiguration necsConfiguration;
        private readonly IReIdentificationService reIdentificationService;
        private readonly ICompareLogic compareLogic;

        public ReIdentificationServiceTests()
        {
            this.compareLogic = new CompareLogic();
            this.necsBrokerMock = new Mock<INECSBroker>();
            this.identifierBrokerMock = new Mock<IIdentifierBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.necsConfiguration = new NECSConfiguration
            {
                ApiUrl = GetRandomString(),
                ApiKey = GetRandomString(),
                ApiMaxBatchSize = GetRandomNumber()
            };

            this.reIdentificationService = new ReIdentificationService(
                necsBroker: this.necsBrokerMock.Object,
                identifierBroker: this.identifierBrokerMock.Object,
                necsConfiguration: necsConfiguration,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private Expression<Func<NecsReIdentificationRequest, bool>> SameNecsReIdentificationRequestAs(
            NecsReIdentificationRequest expectedNecsReIdentificationRequest)
        {
            return actualNecsReIdentificationRequest =>
                this.compareLogic.Compare(expectedNecsReIdentificationRequest, actualNecsReIdentificationRequest)
                    .AreEqual;
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new HttpResponseUnauthorizedException(),
                new HttpResponseUrlNotFoundException(),
                new HttpResponseBadRequestException()
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;
            var innerException = new Xeption(exceptionMessage);

            return new TheoryData<Xeption>
            {
                new HttpResponseInternalServerErrorException()
            };
        }

        private (List<NecsReIdentificationRequest> requests, List<NecsReIdentificationResponse> responses)
            CreateBatchedItems(IdentificationRequest identificationRequest, int batchSize, Guid identifier)
        {
            List<NecsReIdentificationRequest> requests = new List<NecsReIdentificationRequest>();
            List<NecsReIdentificationResponse> responses = new List<NecsReIdentificationResponse>();

            for (int i = 0; i < identificationRequest.IdentificationItems.Count; i += batchSize)
            {
                NecsReIdentificationRequest necsReIdentificationRequest = new NecsReIdentificationRequest
                {
                    RequestId = identifier,
                    UserIdentifier = identificationRequest.EntraUserId.ToString(),
                    Organisation = identificationRequest.Organisation,
                    Reason = identificationRequest.Reason,
                    PseudonymisedNumbers = identificationRequest.IdentificationItems.Skip(i)
                    .Take(batchSize).ToList().Select(item =>
                        new NecsPseudonymisedItem { RowNumber = item.RowNumber, Psuedo = item.Identifier })
                            .ToList()
                };

                requests.Add(necsReIdentificationRequest);

                NecsReIdentificationResponse necsReIdentificationResponse = new NecsReIdentificationResponse
                {
                    UniqueRequestId = identifier,
                    ElapsedTime = GetRandomNumber(),
                    ProcessedCount = necsReIdentificationRequest.PseudonymisedNumbers.Count,
                    Results = identificationRequest.IdentificationItems.Skip(i)
                        .Take(batchSize).ToList().Select(item =>
                            new NecsReidentifiedItem
                            {
                                RowNumber = item.RowNumber,
                                NhsNumber = $"{item.Identifier}R",
                                Message = $"{item.Message}M",
                            })
                        .ToList()
                };

                responses.Add(necsReIdentificationResponse);
            }

            return (requests, responses);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static string GetRandomString(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static int GetRandomNumber() =>
            new IntRange(max: 15, min: 2).GetValue();

        private static string GetRandomStringWithLength(int length)
        {
            string result = new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

            return result.Length > length ? result.Substring(0, length) : result;
        }

        private static IdentificationRequest CreateRandomIdentificationRequest(int count) =>
            CreateIdentificationRequestFiller(count).Create();

        private static Filler<IdentificationRequest> CreateIdentificationRequestFiller(int count)
        {
            var filler = new Filler<IdentificationRequest>();

            filler.Setup()
                .OnProperty(request => request.IdentificationItems).Use(CreateRandomIdentificationItems(count));

            return filler;
        }

        private static List<IdentificationItem> CreateRandomIdentificationItems(int count)
        {
            return CreateIdentificationItemFiller()
                .Create(count)
                    .ToList();
        }

        private static Filler<IdentificationItem> CreateIdentificationItemFiller()
        {
            var filler = new Filler<IdentificationItem>();

            filler.Setup()
                .OnProperty(address => address.HasAccess).Use(true)
                .OnProperty(address => address.IsReidentified).Use(false);

            return filler;
        }
    }
}
