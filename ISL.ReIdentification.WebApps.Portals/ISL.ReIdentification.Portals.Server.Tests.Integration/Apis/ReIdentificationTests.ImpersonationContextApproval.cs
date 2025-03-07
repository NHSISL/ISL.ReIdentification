﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Models.ImpersonationContexts;
using ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Models.ReIdentifications;

namespace ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Apis
{
    public partial class ReIdentificationTests
    {
        [ReleaseCandidateFact]
        public async Task ShouldUpdateApprovalStatusOnImpersonationContextApprovalAsync()
        {
            // given
            ImpersonationContext randomImpersonationContext = CreateRandomImpersonationContext();
            ImpersonationContext existingImpersonationContext = randomImpersonationContext;
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
