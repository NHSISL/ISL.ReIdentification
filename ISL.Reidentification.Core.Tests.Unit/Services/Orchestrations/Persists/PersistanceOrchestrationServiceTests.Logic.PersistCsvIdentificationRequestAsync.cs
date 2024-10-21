// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Persists
{
    public partial class PersistanceOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldPersistCsvIdentificationRequestAsync()
        {
            // given
            string randomString = GetRandomString();
            string returnedHash = randomString;
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            CsvIdentificationRequest randomCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            randomCsvIdentificationRequest.Sha256Hash = returnedHash;
            randomAccessRequest.IdentificationRequest = null;
            randomAccessRequest.ImpersonationContext = null;
            AccessRequest inputAccessRequest = randomAccessRequest.DeepClone();
            inputAccessRequest.CsvIdentificationRequest.Sha256Hash = returnedHash;
            AccessRequest outputAccessRequest = randomAccessRequest.DeepClone();
            CsvIdentificationRequest inputCsvIdentificationRequest = randomCsvIdentificationRequest.DeepClone();
            CsvIdentificationRequest outputCsvIdentificationRequest = randomCsvIdentificationRequest.DeepClone();
            outputAccessRequest.CsvIdentificationRequest = outputCsvIdentificationRequest;
            AccessRequest expectedAccessRequest = outputAccessRequest.DeepClone();

            IQueryable<CsvIdentificationRequest> emptyRetrieveAllCsvIdentificationRequests =
                Enumerable.Empty<CsvIdentificationRequest>().AsQueryable();

            IQueryable<CsvIdentificationRequest> outputRetrieveAllCsvIdentificationRequests =
                emptyRetrieveAllCsvIdentificationRequests.DeepClone();

            this.hashBrokerMock.Setup(broker =>
                broker.GenerateSha256Hash(new MemoryStream(inputAccessRequest.CsvIdentificationRequest.Data)))
                    .Returns(returnedHash);

            this.csvIdentificationRequestServiceMock.Setup(service =>
                service.RetrieveAllCsvIdentificationRequestsAsync())
                    .ReturnsAsync(outputRetrieveAllCsvIdentificationRequests);

            this.csvIdentificationRequestServiceMock.Setup(service =>
                service.AddCsvIdentificationRequestAsync(inputAccessRequest.CsvIdentificationRequest))
                    .ReturnsAsync(outputCsvIdentificationRequest);

            // when
            AccessRequest actualAccessRequest =
                await this.persistanceOrchestrationService
                    .PersistCsvIdentificationRequestAsync(inputAccessRequest);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.RetrieveAllCsvIdentificationRequestsAsync(),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.AddCsvIdentificationRequestAsync(inputAccessRequest.CsvIdentificationRequest),
                    Times.Once);

            this.notificationServiceMock.Verify(service =>
                service.SendPendingApprovalNotificationAsync(It.IsAny<AccessRequest>()),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }


    }
}
