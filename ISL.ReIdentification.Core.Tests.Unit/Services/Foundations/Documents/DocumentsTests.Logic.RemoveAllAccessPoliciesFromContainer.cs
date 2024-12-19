// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.Documents
{
    public partial class DocumentsTests
    {
        [Fact]
        public async Task ShouldRemoveAllAccessPoliciesAsync()
        {
            // given
            string randomContainer = GetRandomString();

            // when
            await this.documentService.RemoveAllAccessPoliciesAsync(randomContainer);

            // then
            this.blobStorageBrokerMock.Verify(broker =>
                broker.RemoveAllAccessPoliciesAsync(randomContainer),
                    Times.Once);

            this.blobStorageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
