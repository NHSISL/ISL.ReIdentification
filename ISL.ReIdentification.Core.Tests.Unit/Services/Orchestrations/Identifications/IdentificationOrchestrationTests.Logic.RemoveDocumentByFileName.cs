// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Identifications
{
    public partial class IdentificationOrchestrationTests
    {
        [Fact]
        public async Task ShouldRemoveDocumentByFileNameAsync()
        {
            // given
            string randomFilename = GetRandomString();
            string randomContainer = GetRandomString();

            // when
            await this.identificationOrchestrationService
                .RemoveDocumentByFileNameAsync(randomFilename, randomContainer);

            // then
            this.documentServiceMock.Verify(service =>
                service.RemoveDocumentByFileNameAsync(randomFilename, randomContainer),
                    Times.Once);

            this.documentServiceMock.VerifyNoOtherCalls();
            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.reIdentificationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
