// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace ISL.ReIdentification.Configurations.Server.Tests.Integration.ReIdentification.Brokers
{
    public partial class ApiBroker
    {
        private readonly WebApplicationFactory<Program> webApplicationFactory;
        private readonly HttpClient httpClient;
        private readonly IRESTFulApiFactoryClient apiFactoryClient;

        public ApiBroker()
        {
            webApplicationFactory = new WebApplicationFactory<Program>();
            httpClient = webApplicationFactory.CreateClient();
            apiFactoryClient = new RESTFulApiFactoryClient(httpClient);
        }
    }
}