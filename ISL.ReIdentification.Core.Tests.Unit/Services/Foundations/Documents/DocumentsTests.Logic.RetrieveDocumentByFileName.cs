// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Documents
{
    public partial class DocumentsTests
    {
        [Fact]
        public async Task ShouldRetrieveDocumentByFileNameAsync()
        {
            // given
            string randomFileName = GetRandomString();
            string randomContainer = GetRandomString();
            MemoryStream randomStream = new MemoryStream();
            byte[] randomfileData = Encoding.UTF8.GetBytes(GetRandomString());
            byte[] expectedData = randomfileData;
            MemoryStream returnedStream = new MemoryStream(randomfileData);
            MemoryStream outputStream = randomStream;

            this.blobStorageBrokerMock
                .Setup(broker => broker
                    .SelectByFileNameAsync(randomStream, randomFileName, randomContainer))
                        .Callback<Stream, string, string>((output, fileName, container) =>
                        {
                            returnedStream.Position = 0;
                            returnedStream.CopyTo(output);
                        })
                        .Returns(ValueTask.CompletedTask);

            // when
            await this.documentService.RetrieveDocumentByFileNameAsync(outputStream, randomFileName, randomContainer);

            // then
            byte[] actualData = ReadAllBytesFromStream(outputStream);
            actualData.Should().BeEquivalentTo(expectedData);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.SelectByFileNameAsync(
                    It.Is(SameStreamAs(outputStream)),
                    randomFileName,
                    randomContainer),
                        Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
