// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.ImpersonationContexts;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Apis
{
    public partial class ReIdentificationApiTests
    {
        [Fact]
        public async Task ShouldUpdateApprovalStatusOnImpersonationContextApprovalAsync()
        {
            // given
            ImpersonationContext randomImpersonationContext = CreateRandomImpersonationContext();
            ImpersonationContext existingImpersonationContext = randomImpersonationContext.DeepClone();
            bool isApproved = true;
            existingImpersonationContext.IsApproved = !isApproved;
            existingImpersonationContext.ResponsiblePersonEntraUserId = TestAuthHandler.SecurityOid;
            await this.apiBroker.PostImpersonationContextAsync(existingImpersonationContext);

            ApprovalRequest inputApprovalRequest = new ApprovalRequest
            {
                ImpersonationContextId = existingImpersonationContext.Id,
                IsApproved = isApproved
            };

            // when
            await this.apiBroker.PostImpersonationContextApprovalAsync(inputApprovalRequest);

            // then
            ImpersonationContext retrievedImpersonationContext =
                await this.apiBroker.GetImpersonationContextByIdAsync(existingImpersonationContext.Id);

            retrievedImpersonationContext.IsApproved.Should().Be(isApproved);
            await this.apiBroker.DeleteImpersonationContextByIdAsync(retrievedImpersonationContext.Id);
        }
    }
}
