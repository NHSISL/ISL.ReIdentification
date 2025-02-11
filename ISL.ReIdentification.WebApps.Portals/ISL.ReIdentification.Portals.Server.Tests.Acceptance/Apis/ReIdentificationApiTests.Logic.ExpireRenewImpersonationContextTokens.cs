// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
        public async Task ShouldExpireRenewImpersonationContextTokensAsync()
        {
            // given
            ImpersonationContext randomImpersonationContext = CreateRandomImpersonationContext();
            ImpersonationContext existingImpersonationContext = randomImpersonationContext.DeepClone();
            existingImpersonationContext.IsApproved = false;
            await this.apiBroker.PostImpersonationContextAsync(existingImpersonationContext);
            Guid inputImpersonationContextId = existingImpersonationContext.Id;

            // when
            AccessRequest actualAccessRequest =
                await this.apiBroker.PostImpersonationContextGenerateTokensAsync(inputImpersonationContextId);

            // then
            actualAccessRequest.ImpersonationContext.InboxSasToken.Should().NotBeNullOrEmpty();
            actualAccessRequest.ImpersonationContext.OutboxSasToken.Should().NotBeNullOrEmpty();
            actualAccessRequest.ImpersonationContext.ErrorsSasToken.Should().NotBeNullOrEmpty();
            await this.apiBroker.DeleteImpersonationContextByIdAsync(actualAccessRequest.ImpersonationContext.Id);
        }
    }
}
