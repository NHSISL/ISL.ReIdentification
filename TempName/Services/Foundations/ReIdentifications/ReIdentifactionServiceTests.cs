// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ISL.Providers.ReIdentification.Abstractions.Models;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.ReIdentifications;
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
        private readonly Mock<IReIdentificationBroker> reIdentificationBrokerMock = new Mock<IReIdentificationBroker>();
        private readonly Mock<ILoggingBroker> loggingBrokerMock = new Mock<ILoggingBroker>();
        private readonly IReIdentificationService reIdentificationService;
        private readonly ICompareLogic compareLogic;

        public ReIdentificationServiceTests()
        {
            this.compareLogic = new CompareLogic();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.reIdentificationService = new ReIdentificationService(
                reIdentificationBroker: this.reIdentificationBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private Expression<Func<ReIdentificationRequest, bool>> SameReIdentificationRequestAs(
            ReIdentificationRequest expectedReIdentificationRequest)
        {
            return actualReIdentificationRequest =>
                this.compareLogic.Compare(expectedReIdentificationRequest, actualReIdentificationRequest)
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

        private (List<ReIdentificationRequest> requests, List<ReIdentificationRequest> responses)
            CreateBatchedItems(IdentificationRequest identificationRequest, int batchSize, Guid identifier)
        {
            List<ReIdentificationRequest> requests = new List<ReIdentificationRequest>();
            List<ReIdentificationRequest> responses = new List<ReIdentificationRequest>();

            for (int i = 0; i < identificationRequest.IdentificationItems.Count; i += batchSize)
            {
                ReIdentificationRequest necsReIdentificationRequest = new ReIdentificationRequest
                {
                    RequestId = identifier,
                    UserIdentifier = identificationRequest.EntraUserId.ToString(),
                    Organisation = identificationRequest.Organisation,
                    Reason = identificationRequest.Reason,
                    ReIdentificationItems = identificationRequest.IdentificationItems.Skip(i)
                    .Take(batchSize).ToList().Select(item =>
                        new ReIdentificationItem { RowNumber = item.RowNumber, Identifier = item.Identifier })
                            .ToList()
                };

                requests.Add(necsReIdentificationRequest);

                ReIdentificationRequest reIdentificationResponse = new ReIdentificationRequest
                {
                    RequestId = identifier,
                    Organisation = necsReIdentificationRequest.Organisation,
                    Reason = necsReIdentificationRequest.Reason,
                    ReIdentificationItems = identificationRequest.IdentificationItems.Skip(i)
                        .Take(batchSize).ToList().Select(item =>
                            new ReIdentificationItem
                            {
                                RowNumber = item.RowNumber,
                                Identifier = $"{item.Identifier}R",
                                Message = $"{item.Message}M",
                            })
                        .ToList()
                };

                responses.Add(reIdentificationResponse);
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

        private static ReIdentificationRequest MapToReIdentificationRequest(IdentificationRequest identificationRequest)
        {
            var request = new ReIdentificationRequest
            {
                RequestId = identificationRequest.Id,
                UserIdentifier = identificationRequest.EntraUserId.ToString(),
                Organisation = identificationRequest.Organisation,
                Reason = identificationRequest.Reason,
                ReIdentificationItems = identificationRequest.IdentificationItems
                    .Select(item => new ReIdentificationItem
                    {
                        RowNumber = item.RowNumber,
                        Identifier = item.Identifier
                    }).ToList()
            };

            return request;
        }

        private static IdentificationRequest MapToIdentificationRequest(
            ReIdentificationRequest reIdentificationRequest,
            IdentificationRequest identificationRequest)
        {
            var request = new IdentificationRequest
            {
                Id = identificationRequest.Id,
                EntraUserId = identificationRequest.EntraUserId,
                Organisation = identificationRequest.Organisation,
                Reason = identificationRequest.Reason,
                DisplayName = identificationRequest.DisplayName,
                Email = identificationRequest.Email,
                GivenName = identificationRequest.GivenName,
                JobTitle = identificationRequest.JobTitle,
                Surname = identificationRequest.Surname,
                IdentificationItems = reIdentificationRequest.ReIdentificationItems
                    .Select(item => new IdentificationItem
                    {
                        RowNumber = item.RowNumber,
                        Identifier = item.Identifier,
                        IsReidentified = true,
                        Message = "OK",
                        HasAccess = true
                    }).ToList()
            };

            return request;
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
                .OnProperty(address => address.Message).Use("OK")
                .OnProperty(address => address.HasAccess).Use(true)
                .OnProperty(address => address.IsReidentified).Use(false);

            return filler;
        }
    }
}
