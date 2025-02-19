// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Configuration.Server.Tests.Integration.Models;

namespace ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Brokers
{
    public partial class ApiBroker
    {
        private const string lookupsRelativeUrl = "api/lookups";

        public async ValueTask<Lookup> PostLookupAsync(Lookup lookup) =>
            await this.apiFactoryClient.PostContentAsync(lookupsRelativeUrl, lookup);

        public async ValueTask<List<Lookup>> GetAllLookupsAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<Lookup>>(lookupsRelativeUrl);

        public async ValueTask<Lookup> GetLookupByIdAsync(Guid lookupId) =>
            await this.apiFactoryClient.GetContentAsync<Lookup>($"{lookupsRelativeUrl}/{lookupId}");

        public async ValueTask<Lookup> PutLookupAsync(Lookup lookup) =>
            await this.apiFactoryClient.PutContentAsync(lookupsRelativeUrl, lookup);

        public async ValueTask<Lookup> DeleteLookupByIdAsync(Guid lookupId) =>
            await this.apiFactoryClient.DeleteContentAsync<Lookup>($"{lookupsRelativeUrl}/{lookupId}");
    }
}