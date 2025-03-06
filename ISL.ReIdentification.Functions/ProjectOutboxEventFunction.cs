// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Services.Coordinations.Identifications;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json.Linq;

namespace ISL.ReIdentification.Functions
{
    public class ProjectOutboxEventFunction
    {
        private readonly IIdentificationCoordinationService identificationCoordinationService;
        private readonly ILoggingBroker loggingBroker;

        public ProjectOutboxEventFunction(
            IIdentificationCoordinationService identificationCoordinationService,
            ILoggingBroker loggingBroker)
        {
            this.identificationCoordinationService = identificationCoordinationService;
            this.loggingBroker = loggingBroker;
        }

        [Function("ProjectOutboxEventFunction")]
        public async Task Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            await loggingBroker
                .LogInformationAsync(
                    $"C# Blob trigger function Processing blob\n " +
                    $"Name: address/in/{{name}}");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var eventGridEvent = JArray.Parse(requestBody)[0];
                string subject = eventGridEvent["subject"]?.ToString();
                string container = subject?.Split('/')[4];
                string filename = subject?.Split(new[] { "/blobs/" }, StringSplitOptions.None)[1];

                await this.identificationCoordinationService
                    .ProcessImpersonationContextRequestAsync(container, filename);
            }
            catch (Exception ex)
            {
                await loggingBroker.LogErrorAsync(ex);
                throw;
            }
        }
    }
}
