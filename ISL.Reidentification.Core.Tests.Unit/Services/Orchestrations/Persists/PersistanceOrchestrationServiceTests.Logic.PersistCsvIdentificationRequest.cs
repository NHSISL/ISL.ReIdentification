// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            CsvIdentificationRequest randomCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            string returnedHash = randomString;
            CsvIdentificationRequest inputCsvIdentificationRequest = randomCsvIdentificationRequest.DeepClone();
            AccessRequest inputAccessRequest = randomAccessRequest.DeepClone();
            inputCsvIdentificationRequest.Sha256Hash = returnedHash;
            inputAccessRequest.IdentificationRequest = null;
            inputAccessRequest.ImpersonationContext = null;
            inputAccessRequest.CsvIdentificationRequest.Sha256Hash = returnedHash;
            AccessRequest outputAccessRequest = inputAccessRequest.DeepClone();
            CsvIdentificationRequest outputCsvIdentificationRequest = inputCsvIdentificationRequest.DeepClone();
            outputAccessRequest.CsvIdentificationRequest = outputCsvIdentificationRequest;
            AccessRequest expectedAccessRequest = outputAccessRequest.DeepClone();

            IQueryable<CsvIdentificationRequest> emptyRetrieveAllCsvIdentificationRequests =
                Enumerable.Empty<CsvIdentificationRequest>().AsQueryable();

            IQueryable<CsvIdentificationRequest> outputRetrieveAllCsvIdentificationRequests =
                emptyRetrieveAllCsvIdentificationRequests.DeepClone();

            this.hashBrokerMock.Setup(broker =>
                broker.GenerateSha256Hash(It.IsAny<MemoryStream>()))
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

            this.hashBrokerMock.Verify(broker =>
                broker.GenerateSha256Hash(It.IsAny<MemoryStream>()),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.RetrieveAllCsvIdentificationRequestsAsync(),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.AddCsvIdentificationRequestAsync(inputAccessRequest.CsvIdentificationRequest),
                    Times.Once);

            this.notificationServiceMock.Verify(service =>
                service.SendCsvPendingApprovalNotificationAsync(It.IsAny<AccessRequest>()),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.documentServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnAccessRequestOnPersistCsvIdentificationRequestIfExistsAsync()
        {
            // given
            string randomString = GetRandomString();
            string returnedHash = randomString;
            Guid randomGuid = Guid.NewGuid();
            Guid randomId = randomGuid;
            AccessRequest randomAccessRequest = CreateRandomAccessRequest();
            CsvIdentificationRequest randomCsvIdentificationRequest = CreateRandomCsvIdentificationRequest();
            AccessRequest inputAccessRequest = randomAccessRequest.DeepClone();
            CsvIdentificationRequest returnedCsvIdentificationRequest = randomCsvIdentificationRequest.DeepClone();
            returnedCsvIdentificationRequest.Sha256Hash = returnedHash;
            inputAccessRequest.IdentificationRequest = null;
            inputAccessRequest.ImpersonationContext = null;
            inputAccessRequest.CsvIdentificationRequest.Sha256Hash = returnedHash;
            inputAccessRequest.CsvIdentificationRequest.RecipientEntraUserId = randomId;
            returnedCsvIdentificationRequest.RecipientEntraUserId = randomId;
            AccessRequest outputAccessRequest = inputAccessRequest.DeepClone();
            outputAccessRequest.CsvIdentificationRequest = returnedCsvIdentificationRequest;
            AccessRequest expectedAccessRequest = outputAccessRequest.DeepClone();

            IQueryable<CsvIdentificationRequest> emptyRetrieveAllCsvIdentificationRequests =
                Enumerable.Empty<CsvIdentificationRequest>().AsQueryable();

            IQueryable<CsvIdentificationRequest> outputRetrieveAllCsvIdentificationRequests =
                emptyRetrieveAllCsvIdentificationRequests.Append(returnedCsvIdentificationRequest);

            this.hashBrokerMock.Setup(broker =>
                broker.GenerateSha256Hash(It.IsAny<MemoryStream>()))
                    .Returns(returnedHash);

            this.csvIdentificationRequestServiceMock.Setup(service =>
                service.RetrieveAllCsvIdentificationRequestsAsync())
                    .ReturnsAsync(outputRetrieveAllCsvIdentificationRequests);

            // when
            AccessRequest actualAccessRequest =
                await this.persistanceOrchestrationService
                    .PersistCsvIdentificationRequestAsync(inputAccessRequest);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            this.hashBrokerMock.Verify(broker =>
                broker.GenerateSha256Hash(It.IsAny<MemoryStream>()),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.RetrieveAllCsvIdentificationRequestsAsync(),
                    Times.Once);

            this.csvIdentificationRequestServiceMock.Verify(service =>
                service.AddCsvIdentificationRequestAsync(inputAccessRequest.CsvIdentificationRequest),
                    Times.Never);

            this.notificationServiceMock.Verify(service =>
                service.SendCsvPendingApprovalNotificationAsync(It.IsAny<AccessRequest>()),
                    Times.Never);

            this.csvIdentificationRequestServiceMock.VerifyNoOtherCalls();
            this.impersonationContextServiceMock.VerifyNoOtherCalls();
            this.notificationServiceMock.VerifyNoOtherCalls();
            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.documentServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
