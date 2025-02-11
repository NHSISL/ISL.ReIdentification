// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Configurations.Server;
using RESTFulSense.Clients;

namespace ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.Brokers
{
    public partial class ApiBroker
    {
        private readonly TestWebApplicationFactory<Program> webApplicationFactory;
        private readonly HttpClient httpClient;
        private readonly IRESTFulApiFactoryClient apiFactoryClient;

        public ApiBroker()
        {
            webApplicationFactory = new TestWebApplicationFactory<Program>();
            httpClient = webApplicationFactory.CreateClient();
            apiFactoryClient = new RESTFulApiFactoryClient(httpClient);
        }
    }
}