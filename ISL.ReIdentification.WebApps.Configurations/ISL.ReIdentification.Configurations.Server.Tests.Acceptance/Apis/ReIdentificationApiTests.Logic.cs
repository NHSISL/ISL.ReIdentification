// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Models.Accesses;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Apis
{
    public partial class ReIdentificationApiTests
    {
        [Fact]
        public async Task ShouldPostImpersonationContextAsync()
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
    }
}
