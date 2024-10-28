// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.IO;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Identifications
{
    public partial class IdentificationOrchestrationTests
    {
        [Fact]
        public async Task ShouldAddDocumentAsync()
        {
            // given
            string randomFileName = GetRandomString();
            string randomContainer = GetRandomString();
            byte[] randomfileData = Encoding.UTF8.GetBytes(GetRandomString());
            MemoryStream randomStream = new MemoryStream(randomfileData);

            // when
            await this.identificationOrchestrationService
                .AddDocumentAsync(input: randomStream, fileName: randomFileName, container: randomContainer);

            // then
            this.documentServiceMock.Verify(service =>
                service.AddDocumentAsync(randomStream, randomFileName, randomContainer),
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
