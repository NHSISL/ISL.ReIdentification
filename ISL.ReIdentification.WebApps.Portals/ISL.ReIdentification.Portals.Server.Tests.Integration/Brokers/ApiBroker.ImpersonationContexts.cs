// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Models.ImpersonationContexts;

namespace ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Brokers
{
    public partial class ApiBroker
    {
        private const string impersonationContextsRelativeUrl = "api/impersonationContexts";

        public async ValueTask<ImpersonationContext> PostImpersonationContextAsync(
            ImpersonationContext impersonationContext) =>
                await this.apiFactoryClient.PostContentAsync(impersonationContextsRelativeUrl, impersonationContext);

        public async ValueTask<ImpersonationContext> DeleteImpersonationContextByIdAsync(Guid impersonationContextId) =>
            await this.apiFactoryClient.DeleteContentAsync<ImpersonationContext>(
                $"{impersonationContextsRelativeUrl}/{impersonationContextId}");
    }
}
