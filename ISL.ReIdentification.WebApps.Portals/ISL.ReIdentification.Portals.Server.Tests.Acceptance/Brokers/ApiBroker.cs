// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Net.Http;
using RESTFulSense.Clients;

namespace ISL.ReIdentification.Portals.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private readonly TestWebApplicationFactory<Program> webApplicationFactory;
        private readonly HttpClient httpClient;
        private readonly IRESTFulApiFactoryClient apiFactoryClient;

        public ApiBroker()
        {
            this.webApplicationFactory = new TestWebApplicationFactory<Program>();
            this.httpClient = this.webApplicationFactory.CreateClient();
            this.apiFactoryClient = new RESTFulApiFactoryClient(this.httpClient);
        }
    }
}