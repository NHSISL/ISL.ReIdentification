// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Documents
{
    public partial class DocumentsTests
    {
        [Fact]
        public async Task ShouldCreateDirectorySasTokenAsync()
        {
            // given
            string randomContainer = GetRandomString();
            string directoryPath = GetRandomString();
            string accessPolicyIdentifier = GetRandomString();
            DateTimeOffset futureDateTimeOffset = GetRandomFutureDateTimeOffset();
            string expectedSasToken = GetRandomString();

            this.blobStorageBrokerMock.Setup(broker =>
                broker.CreateDirectorySasTokenAsync(
                    randomContainer,
                    directoryPath,
                    accessPolicyIdentifier,
                    futureDateTimeOffset))
                .ReturnsAsync(expectedSasToken);

            // when
            string actualSasToken =
                await this.documentService.CreateDirectorySasTokenAsync(
                    randomContainer,
                    directoryPath,
                    accessPolicyIdentifier,
                    futureDateTimeOffset);

            // then
            actualSasToken.Should().Be(expectedSasToken);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.CreateDirectorySasTokenAsync(
                    randomContainer,
                    directoryPath,
                    accessPolicyIdentifier,
                    futureDateTimeOffset),
                        Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}