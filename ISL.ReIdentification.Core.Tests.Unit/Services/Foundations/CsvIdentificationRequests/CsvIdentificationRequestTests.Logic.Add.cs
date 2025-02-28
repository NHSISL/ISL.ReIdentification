// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Securities;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.CsvIdentificationRequests
{
    public partial class CsvIdentificationRequestsTests
    {
        [Fact]
        public async Task ShouldAddCsvIdentificationRequestAsync()
        {
            //given
            DateTimeOffset randomDateOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            CsvIdentificationRequest randomCsvIdentificationRequest =
                CreateRandomCsvIdentificationRequest(
                    dateTimeOffset: randomDateOffset,
                    userId: randomEntraUser.EntraUserId);

            CsvIdentificationRequest inputCsvIdentificationRequest = randomCsvIdentificationRequest;
            CsvIdentificationRequest storageCsvIdentificationRequest = inputCsvIdentificationRequest.DeepClone();
            CsvIdentificationRequest expectedCsvIdentificationRequest = inputCsvIdentificationRequest.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.InsertCsvIdentificationRequestAsync(inputCsvIdentificationRequest))
                    .ReturnsAsync(storageCsvIdentificationRequest);

            //when
            CsvIdentificationRequest actualCsvIdentificationRequest =
                await this.csvIdentificationRequestService.AddCsvIdentificationRequestAsync(
                    inputCsvIdentificationRequest);

            //then
            actualCsvIdentificationRequest.Should().BeEquivalentTo(expectedCsvIdentificationRequest);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Exactly(2));

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Exactly(2));

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.InsertCsvIdentificationRequestAsync(inputCsvIdentificationRequest),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}