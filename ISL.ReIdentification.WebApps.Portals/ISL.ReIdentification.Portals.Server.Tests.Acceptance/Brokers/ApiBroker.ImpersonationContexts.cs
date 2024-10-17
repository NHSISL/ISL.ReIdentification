// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISL.ReIdentification.Portals.Server.Tests.Acceptance.Models.ImpersonationContexts;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string ImpersonationContextsRelativeUrl = "api/impersonationContexts";

        public async ValueTask<ImpersonationContext> PostImpersonationContextAsync(
            ImpersonationContext impersonationContext) =>
                await this.apiFactoryClient.PostContentAsync(ImpersonationContextsRelativeUrl, impersonationContext);

        public async ValueTask<List<ImpersonationContext>> GetAllImpersonationContextsAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<ImpersonationContext>>(
                $"{ImpersonationContextsRelativeUrl}/");

        public async ValueTask<ImpersonationContext> GetImpersonationContextByIdAsync(Guid impersonationContextId) =>
            await this.apiFactoryClient
                .GetContentAsync<ImpersonationContext>($"{ImpersonationContextsRelativeUrl}/{impersonationContextId}");

        public async ValueTask<ImpersonationContext> DeleteImpersonationContextByIdAsync(Guid impersonationContextId) =>
            await this.apiFactoryClient.DeleteContentAsync<ImpersonationContext>(
                $"{ImpersonationContextsRelativeUrl}/{impersonationContextId}");

        public async ValueTask<ImpersonationContext> PutImpersonationContextAsync(
            ImpersonationContext impersonationContext) =>
                await this.apiFactoryClient.PutContentAsync(ImpersonationContextsRelativeUrl, impersonationContext);
    }
}
