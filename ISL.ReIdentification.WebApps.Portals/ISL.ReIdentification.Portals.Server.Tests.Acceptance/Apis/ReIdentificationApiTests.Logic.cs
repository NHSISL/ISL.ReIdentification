// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.Accesses;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.ImpersonationContexts;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Apis
{
    public partial class ReIdentificationApiTests
    {
        [Fact]
        public async Task ShouldPostImpersonationContextSendPendingApprovalAsync()
        {
            // given
            AccessRequest randomAccessRequest = CreateImpersonationContextAccessRequest();
            AccessRequest inputAccessRequest = randomAccessRequest.DeepClone();
            AccessRequest expectedAccessRequest = inputAccessRequest.DeepClone();

            // when
            AccessRequest actualAccessRequest = await this.apiBroker.PostImpersonationContextRequestAsync(inputAccessRequest);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);
            await this.apiBroker.DeleteImpersonationContextByIdAsync(actualAccessRequest.ImpersonationContext.Id);
        }

        [Fact]
        public async Task ShouldPostImpersonationContextApprovedAsync()
        {
            // given
            ImpersonationContext randomImpersonationContext = CreateRandomImpersonationContext();
            ImpersonationContext existingImpersonationContext = randomImpersonationContext.DeepClone();
            existingImpersonationContext.IsApproved = false;
            await this.apiBroker.PostImpersonationContextAsync(existingImpersonationContext);

            ImpersonationContext inputImpersonationContext =
                UpdateImpersonationContextWithRandomValues(existingImpersonationContext);

            inputImpersonationContext.IsApproved = true;

            AccessRequest inputAccessRequest = new AccessRequest
            {
                ImpersonationContext = inputImpersonationContext,
            };

            AccessRequest expectedAccessRequest = inputAccessRequest.DeepClone();

            // when
            AccessRequest actualAccessRequest =
                await this.apiBroker.PostImpersonationContextRequestAsync(inputAccessRequest);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);
            await this.apiBroker.DeleteImpersonationContextByIdAsync(actualAccessRequest.ImpersonationContext.Id);
        }

        [Fact]
        public async Task ShouldPostImpersonationContextDeniedAsync()
        {
            // given
            ImpersonationContext randomImpersonationContext = CreateRandomImpersonationContext();
            ImpersonationContext existingImpersonationContext = randomImpersonationContext.DeepClone();
            existingImpersonationContext.IsApproved = true;
            await this.apiBroker.PostImpersonationContextAsync(existingImpersonationContext);

            ImpersonationContext inputImpersonationContext =
                UpdateImpersonationContextWithRandomValues(existingImpersonationContext);

            inputImpersonationContext.IsApproved = false;

            AccessRequest inputAccessRequest = new AccessRequest
            {
                ImpersonationContext = inputImpersonationContext,
            };

            AccessRequest expectedAccessRequest = inputAccessRequest.DeepClone();

            // when
            AccessRequest actualAccessRequest =
                await this.apiBroker.PostImpersonationContextRequestAsync(inputAccessRequest);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);
            await this.apiBroker.DeleteImpersonationContextByIdAsync(actualAccessRequest.ImpersonationContext.Id);
        }
    }
}
