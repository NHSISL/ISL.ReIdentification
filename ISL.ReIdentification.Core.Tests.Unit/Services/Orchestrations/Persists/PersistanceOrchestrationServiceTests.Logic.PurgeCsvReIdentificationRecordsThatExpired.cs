// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Persists
{
    public partial class PersistanceOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldPurgeCsvReIdentificationRecordsThatExpiredAsync()
        {
            // given
            DateTimeOffset now = DateTimeOffset.UtcNow;
            List<CsvIdentificationRequest> outputCsvIdentificationRequests = new List<CsvIdentificationRequest>();
            List<CsvIdentificationRequest> csvIdentificationRequests = CreateRandomCsvIdentificationRequests();

            List<CsvIdentificationRequest> expiredCsvIdentificationRequests =
                CreateRandomExpiredCsvIdentificationRequests();

            outputCsvIdentificationRequests.AddRange(csvIdentificationRequests);
            outputCsvIdentificationRequests.AddRange(expiredCsvIdentificationRequests);

            List<CsvIdentificationRequest> expectedExpiredCsvIdentificationRequests = expiredCsvIdentificationRequests
                .DeepClone();

            List<AccessAudit> accessAudits = new List<AccessAudit>();

            foreach (CsvIdentificationRequest csvIdentificationRequest in expectedExpiredCsvIdentificationRequests)
            {
                csvIdentificationRequest.Data = null;
                csvIdentificationRequest.UpdatedDate = now;
            }

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            this.csvIdentificationRequestServiceMock.Setup(service =>
                service.RetrieveAllCsvIdentificationRequestsAsync())
                    .ReturnsAsync(outputCsvIdentificationRequests.AsQueryable());

            foreach (CsvIdentificationRequest csvIdentificationRequest in expectedExpiredCsvIdentificationRequests)
            {
                this.csvIdentificationRequestServiceMock.Setup(service =>
                    service.ModifyCsvIdentificationRequestAsync(csvIdentificationRequest))
                        .ReturnsAsync(csvIdentificationRequest);

                var accessAudit = CreateRandomPurgedAccessAudit(csvIdentificationRequest.Id);
                accessAudits.Add(accessAudit);

                this.accessAuditServiceMock.Setup(service =>
                    service.AddAccessAuditAsync(accessAudit))
                        .ReturnsAsync(accessAudit);
            }

            // when
            await this.persistanceOrchestrationService
                .PurgeCsvReIdentificationRecordsThatExpired();

            List<CsvIdentificationRequest> actualExpiredCsvIdentificationRequests = expiredCsvIdentificationRequests;

            // then
            actualExpiredCsvIdentificationRequests.Should().BeEquivalentTo(expectedExpiredCsvIdentificationRequests);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.RetrieveAllCsvIdentificationRequestsAsync(),
                    Times.Once);

            foreach (CsvIdentificationRequest csvIdentificationRequest in expectedExpiredCsvIdentificationRequests)
            {
                this.csvIdentificationRequestServiceMock.Verify(service =>
                    service.ModifyCsvIdentificationRequestAsync(csvIdentificationRequest)
                        , Times.Once);
            }

            foreach (AccessAudit accessAudit in accessAudits)
            {
                this.accessAuditServiceMock.Verify(service =>
                    service.AddAccessAuditAsync(accessAudit)
                        , Times.Once);
            }

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
