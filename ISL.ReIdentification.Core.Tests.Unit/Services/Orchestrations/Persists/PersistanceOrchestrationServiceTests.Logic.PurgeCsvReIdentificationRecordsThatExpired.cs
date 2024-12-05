// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            List<CsvIdentificationRequest> unmodifiedExpiredCsvIdentificationRequests =
                expiredCsvIdentificationRequests.DeepClone();

            outputCsvIdentificationRequests.AddRange(csvIdentificationRequests);
            outputCsvIdentificationRequests.AddRange(unmodifiedExpiredCsvIdentificationRequests);

            List<AccessAudit> accessAudits = new List<AccessAudit>();

            foreach (CsvIdentificationRequest csvIdentificationRequest in expiredCsvIdentificationRequests)
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

            var accessAuditId = Guid.NewGuid();

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ReturnsAsync(accessAuditId);

            foreach (CsvIdentificationRequest csvIdentificationRequest in expiredCsvIdentificationRequests)
            {
                this.csvIdentificationRequestServiceMock.Setup(service =>
                    service.ModifyCsvIdentificationRequestAsync(
                        It.Is(SameCsvIdentificationRequestAs(csvIdentificationRequest))))
                            .ReturnsAsync(csvIdentificationRequest);

                var accessAudit = CreateRandomPurgedAccessAudit(accessAuditId, csvIdentificationRequest.Id, now);
                accessAudits.Add(accessAudit);

                this.accessAuditServiceMock.Setup(service =>
                    service.AddAccessAuditAsync(It.Is(SameAccessAuditAs(accessAudit))))
                        .ReturnsAsync(accessAudit);
            }

            // when
            await this.persistanceOrchestrationService
                .PurgeCsvReIdentificationRecordsThatExpired();

            // then
            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.RetrieveAllCsvIdentificationRequestsAsync(),
                    Times.Once);

            foreach (CsvIdentificationRequest csvIdentificationRequest in expiredCsvIdentificationRequests)
            {
                this.csvIdentificationRequestServiceMock.Verify(service =>
                    service.ModifyCsvIdentificationRequestAsync(
                        It.Is(SameCsvIdentificationRequestAs(csvIdentificationRequest)))
                            , Times.Once);
            }

            foreach (AccessAudit accessAudit in accessAudits)
            {
                this.accessAuditServiceMock.Verify(service =>
                    service.AddAccessAuditAsync(It.Is(SameAccessAuditAs(accessAudit)))
                        , Times.Once);
            }

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Exactly(accessAudits.Count));

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
