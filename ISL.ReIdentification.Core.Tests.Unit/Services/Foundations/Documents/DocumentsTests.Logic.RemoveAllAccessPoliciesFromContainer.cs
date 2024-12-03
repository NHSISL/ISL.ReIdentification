// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Documents
{
    public partial class DocumentsTests
    {
        [Fact]
        public async Task ShouldRemoveAllAccessPoliciesFromContainerAsync()
        {
            // given
            string randomContainer = GetRandomString();

            // when
            await this.documentService.RemoveAllAccessPoliciesFromContainerAsync(randomContainer);

            // then
            this.blobStorageBrokerMock.Verify(broker =>
                broker.RemoveAccessPoliciesFromContainerAsync(randomContainer),
                    Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
