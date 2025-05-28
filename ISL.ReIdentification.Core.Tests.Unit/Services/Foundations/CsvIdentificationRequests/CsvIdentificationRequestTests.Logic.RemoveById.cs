// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Data;
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
        public async Task ShouldRemoveCsvIdentificationRequestByIdAsync()
        {
            //Given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EntraUser randomEntraUser = CreateRandomEntraUser();

            CsvIdentificationRequest randomCsvIdentificationRequest = 
                CreateRandomCsvIdentificationRequest(randomDateTimeOffset, randomEntraUser.EntraUserId);

            Guid inputCsvIdentificationRequestId = randomCsvIdentificationRequest.Id;
            CsvIdentificationRequest storageCsvIdentificationRequest = randomCsvIdentificationRequest;

            CsvIdentificationRequest csvIdentificationRequestWithDeleteAuditApplied = 
                storageCsvIdentificationRequest.DeepClone();

            csvIdentificationRequestWithDeleteAuditApplied.UpdatedBy = randomEntraUser.EntraUserId.ToString();
            csvIdentificationRequestWithDeleteAuditApplied.UpdatedDate = randomDateTimeOffset;
            CsvIdentificationRequest updatedCsvIdentificationRequest = csvIdentificationRequestWithDeleteAuditApplied;
            CsvIdentificationRequest deletedCsvIdentificationRequest = updatedCsvIdentificationRequest;
            CsvIdentificationRequest expectedCsvIdentificationRequest = deletedCsvIdentificationRequest.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.securityBrokerMock.Setup(broker =>
                broker.GetCurrentUserAsync())
                    .ReturnsAsync(randomEntraUser);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectCsvIdentificationRequestByIdAsync(inputCsvIdentificationRequestId))
                    .ReturnsAsync(storageCsvIdentificationRequest);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.UpdateCsvIdentificationRequestAsync(randomCsvIdentificationRequest))
                    .ReturnsAsync(updatedCsvIdentificationRequest);

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.DeleteCsvIdentificationRequestAsync(updatedCsvIdentificationRequest))
                    .ReturnsAsync(deletedCsvIdentificationRequest);

            //When
            CsvIdentificationRequest actualCsvIdentificationRequest = 
                await this.csvIdentificationRequestService.RemoveCsvIdentificationRequestByIdAsync(inputCsvIdentificationRequestId);

            //Then
            actualCsvIdentificationRequest.Should().BeEquivalentTo(expectedCsvIdentificationRequest);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectCsvIdentificationRequestByIdAsync(inputCsvIdentificationRequestId),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.securityBrokerMock.Verify(broker =>
                broker.GetCurrentUserAsync(),
                    Times.Exactly(2));

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.UpdateCsvIdentificationRequestAsync(randomCsvIdentificationRequest),
                    Times.Once);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.DeleteCsvIdentificationRequestAsync(updatedCsvIdentificationRequest),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.securityBrokerMock.VerifyNoOtherCalls();
        }
    }
}