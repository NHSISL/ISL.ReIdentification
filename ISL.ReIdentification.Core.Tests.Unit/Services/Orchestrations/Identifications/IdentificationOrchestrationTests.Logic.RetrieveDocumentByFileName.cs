// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Identifications
{
    public partial class IdentificationOrchestrationTests
    {
        [Fact]
        public async Task ShouldRetrieveDocumentByFileNameAsync()
        {
            // given
            string randomFileName = GetRandomString();
            string randomContainer = GetRandomString();
            byte[] randomfileData = Encoding.UTF8.GetBytes(GetRandomString());
            byte[] expectedData = randomfileData;
            MemoryStream returnedStream = new MemoryStream(randomfileData);
            MemoryStream randomStream = new MemoryStream();
            MemoryStream outputStream = randomStream;

            this.documentServiceMock
                .Setup(service => service
                    .RetrieveDocumentByFileNameAsync(randomStream, randomFileName, randomContainer))
                        .Callback<Stream, string, string>((output, fileName, container) =>
                        {
                            returnedStream.Position = 0;
                            returnedStream.CopyTo(output);
                        })
                        .Returns(ValueTask.CompletedTask);

            // when
            await this.identificationOrchestrationService
                .RetrieveDocumentByFileNameAsync(outputStream, randomFileName, randomContainer);

            // then
            byte[] actualData = ReadAllBytesFromStream(outputStream);
            actualData.Should().BeEquivalentTo(expectedData);

            this.documentServiceMock.Verify(service =>
                service.RetrieveDocumentByFileNameAsync(
                    It.Is(SameStreamAs(outputStream)),
                    randomFileName,
                    randomContainer),
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
