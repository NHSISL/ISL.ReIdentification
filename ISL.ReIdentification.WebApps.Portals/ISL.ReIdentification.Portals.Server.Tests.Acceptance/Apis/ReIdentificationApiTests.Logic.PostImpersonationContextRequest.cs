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
            AccessRequest actualAccessRequest =
                await this.apiBroker.PostImpersonationContextRequestAsync(inputAccessRequest);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest, options => options
                .Excluding(req => req.ImpersonationContext.CreatedBy)
                .Excluding(req => req.ImpersonationContext.CreatedDate)
                .Excluding(req => req.ImpersonationContext.UpdatedBy)
                .Excluding(req => req.ImpersonationContext.UpdatedDate));
            await this.apiBroker.DeleteImpersonationContextByIdAsync(actualAccessRequest.ImpersonationContext.Id);
        }

        [Fact]
        public async Task ShouldPostImpersonationContextSendApprovedAsync()
        {
            // given

            ImpersonationContext originalContext = CreateRandomImpersonationContext();
            originalContext.IsApproved = false;
            await this.apiBroker.PostImpersonationContextAsync(originalContext);

            ImpersonationContext postedContext = 
                await this.apiBroker.GetImpersonationContextByIdAsync(originalContext.Id);

            postedContext.IsApproved = true;

            AccessRequest inputAccessRequest = new AccessRequest
            {
                ImpersonationContext = postedContext,
            };

            AccessRequest expectedAccessRequest = inputAccessRequest.DeepClone();

            // when
            AccessRequest actualAccessRequest =
                await this.apiBroker.PostImpersonationContextRequestAsync(inputAccessRequest);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest, options => options
                .Excluding(req => req.ImpersonationContext.CreatedBy)
                .Excluding(req => req.ImpersonationContext.CreatedDate)
                .Excluding(req => req.ImpersonationContext.UpdatedBy)
                .Excluding(req => req.ImpersonationContext.UpdatedDate));

            await this.apiBroker.DeleteImpersonationContextByIdAsync(actualAccessRequest.ImpersonationContext.Id);
        }

        [Fact]
        public async Task ShouldPostImpersonationContextSendDeniedAsync()
        {
            // given
            ImpersonationContext originalContext = CreateRandomImpersonationContext();
            originalContext.IsApproved = true;
            await this.apiBroker.PostImpersonationContextAsync(originalContext);

            ImpersonationContext postedContext = 
                await this.apiBroker.GetImpersonationContextByIdAsync(originalContext.Id);

            postedContext.IsApproved = false;

            AccessRequest inputAccessRequest = new AccessRequest
            {
                ImpersonationContext = postedContext,
            };

            AccessRequest expectedAccessRequest = inputAccessRequest.DeepClone();

            // when
            AccessRequest actualAccessRequest =
                await this.apiBroker.PostImpersonationContextRequestAsync(inputAccessRequest);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest, options => options
                .Excluding(req => req.ImpersonationContext.CreatedBy)
                .Excluding(req => req.ImpersonationContext.CreatedDate)
                .Excluding(req => req.ImpersonationContext.UpdatedBy)
                .Excluding(req => req.ImpersonationContext.UpdatedDate));

            await this.apiBroker.DeleteImpersonationContextByIdAsync(actualAccessRequest.ImpersonationContext.Id);
        }
    }
}
