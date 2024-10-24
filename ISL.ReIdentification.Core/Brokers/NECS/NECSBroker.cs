﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Brokers.NECS;
using ISL.ReIdentification.Core.Models.Brokers.NECS.Requests;
using ISL.ReIdentification.Core.Models.Brokers.NECS.Responses;
using RESTFulSense.Clients;

namespace ISL.ReIdentification.Core.Brokers.NECS
{
    public class NECSBroker : INECSBroker
    {
        private readonly NECSConfiguration necsConfiguration;
        private readonly IRESTFulApiFactoryClient apiClient;
        private readonly HttpClient httpClient;

        public NECSBroker(NECSConfiguration necsConfiguration)
        {
            this.necsConfiguration = necsConfiguration;
            httpClient = SetupHttpClient();
            apiClient = SetupApiClient();
        }

        public async ValueTask<NecsReIdentificationResponse> ReIdAsync(
            NecsReIdentificationRequest necsReIdentificationRequest)
        {
            var returnedAddress =
                await apiClient.PostContentAsync<NecsReIdentificationRequest, NecsReIdentificationResponse>
                    ($"api/Reid/Process", necsReIdentificationRequest);

            return returnedAddress;
        }

        private HttpClient SetupHttpClient()
        {
            var httpClient = new HttpClient()
            {
                BaseAddress =
                    new Uri(uriString: necsConfiguration.ApiUrl),
            };

            httpClient.DefaultRequestHeaders.Add("X-API-KEY", necsConfiguration.ApiKey);

            return httpClient;
        }

        private IRESTFulApiFactoryClient SetupApiClient() =>
            new RESTFulApiFactoryClient(httpClient);
    }
}
