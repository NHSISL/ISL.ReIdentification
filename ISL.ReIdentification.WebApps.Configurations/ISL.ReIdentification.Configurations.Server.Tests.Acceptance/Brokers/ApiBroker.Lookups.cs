// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Models.Lookups;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string LookupsRelativeUrl = "api/lookups";

        public async ValueTask<Lookup> PostLookupAsync(Lookup lookup) =>
            await this.apiFactoryClient.PostContentAsync(LookupsRelativeUrl, lookup);

        public async ValueTask<Lookup> GetLookupByIdAsync(Guid lookupId) =>
            await this.apiFactoryClient.GetContentAsync<Lookup>($"{LookupsRelativeUrl}/{lookupId}");

        public async ValueTask<List<Lookup>> GetAllLookupsAsync() =>
          await this.apiFactoryClient.GetContentAsync<List<Lookup>>($"{LookupsRelativeUrl}/");

        public async ValueTask<Lookup> PutLookupAsync(Lookup lookup) =>
            await this.apiFactoryClient.PutContentAsync(LookupsRelativeUrl, lookup);

        public async ValueTask<Lookup> DeleteLookupByIdAsync(Guid lookupId) =>
            await this.apiFactoryClient.DeleteContentAsync<Lookup>($"{LookupsRelativeUrl}/{lookupId}");
    }
}
