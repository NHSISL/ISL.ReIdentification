// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Services.Coordinations.Identifications;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

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
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            await loggingBroker
                .LogInformationAsync(
                    $"C# Blob trigger function Processing blob\n " +
                    $"Name: address/in/{{name}}");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                EventGridEvent[] egEvents = EventGridEvent.ParseMany(BinaryData.FromString(requestBody));

                foreach (EventGridEvent egEvent in egEvents)
                {
                    if (egEvent.EventType == "Microsoft.EventGrid.SubscriptionValidationEvent")
                    {
                        var data = egEvent.Data.ToObjectFromJson<SubscriptionValidationEventData>();
                        var response = req.CreateResponse(HttpStatusCode.OK);

                        await response.WriteAsJsonAsync(new
                        {
                            ValidationResponse = data.ValidationCode
                        });

                        return response;
                    }

                    string subject = egEvent.Subject;
                    string container = subject?.Split('/')[4];
                    string filename = subject?.Split(new[] { "/blobs/" }, StringSplitOptions.None)[1];

                    await this.identificationCoordinationService
                        .ProcessImpersonationContextRequestAsync(container, filename);
                }

                return req.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                await loggingBroker.LogErrorAsync(ex);
                throw;
            }
        }
    }
}
