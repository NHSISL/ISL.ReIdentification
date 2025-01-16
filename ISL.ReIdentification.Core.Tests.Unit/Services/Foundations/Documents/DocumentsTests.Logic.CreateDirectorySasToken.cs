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
        public async Task ShouldCreateSasTokenAsync()
        {
            // given
            string randomContainer = GetRandomString();
            string path = GetRandomString();
            string accessPolicyIdentifier = GetRandomString();
            DateTimeOffset futureDateTimeOffset = GetRandomFutureDateTimeOffset();
            string expectedSasToken = GetRandomString();

            this.blobStorageBrokerMock.Setup(broker =>
                broker.CreateSasTokenAsync(
                    randomContainer,
                    path,
                    accessPolicyIdentifier,
                    futureDateTimeOffset))
                        .ReturnsAsync(expectedSasToken);

            // when
            string actualSasToken =
                await this.documentService.CreateSasTokenAsync(
                    randomContainer,
                    path,
                    accessPolicyIdentifier,
                    futureDateTimeOffset);

            // then
            actualSasToken.Should().Be(expectedSasToken);

            this.blobStorageBrokerMock.Verify(broker =>
                broker.CreateSasTokenAsync(
                    randomContainer,
                    path,
                    accessPolicyIdentifier,
                    futureDateTimeOffset),
                        Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}