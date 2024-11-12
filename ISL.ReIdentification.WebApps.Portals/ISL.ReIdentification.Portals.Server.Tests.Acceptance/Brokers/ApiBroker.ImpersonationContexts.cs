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
        private const string impersonationContextsRelativeUrl = "api/impersonationContexts";

        public async ValueTask<ImpersonationContext> PostImpersonationContextAsync(
            ImpersonationContext impersonationContext) =>
                await this.apiFactoryClient.PostContentAsync(impersonationContextsRelativeUrl, impersonationContext);

        public async ValueTask<List<ImpersonationContext>> GetAllImpersonationContextsAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<ImpersonationContext>>(
                $"{impersonationContextsRelativeUrl}/");

        public async ValueTask<List<ImpersonationContext>> GetSpecificImpersonationContextByIdAsync(Guid impersonationContextId) =>
            await this.apiFactoryClient.GetContentAsync<List<ImpersonationContext>>(
                $"{impersonationContextsRelativeUrl}?$filter=Id eq {impersonationContextId}");

        public async ValueTask<ImpersonationContext> GetImpersonationContextByIdAsync(Guid impersonationContextId) =>
            await this.apiFactoryClient
                .GetContentAsync<ImpersonationContext>($"{impersonationContextsRelativeUrl}/{impersonationContextId}");

        public async ValueTask<ImpersonationContext> DeleteImpersonationContextByIdAsync(Guid impersonationContextId) =>
            await this.apiFactoryClient.DeleteContentAsync<ImpersonationContext>(
                $"{impersonationContextsRelativeUrl}/{impersonationContextId}");

        public async ValueTask<ImpersonationContext> PutImpersonationContextAsync(
            ImpersonationContext impersonationContext) =>
                await this.apiFactoryClient.PutContentAsync(impersonationContextsRelativeUrl, impersonationContext);
    }
}
