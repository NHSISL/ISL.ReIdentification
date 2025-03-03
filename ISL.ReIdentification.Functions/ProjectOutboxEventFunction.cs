// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Services.Coordinations.Identifications;

namespace ISL.ReIdentification.Functions
{
    public class ResolvedAddressLoaderFunction
    {
        private readonly IIdentificationCoordinationService identificationCoordinationService;
        private readonly ILoggingBroker loggingBroker;

        public ResolvedAddressLoaderFunction(
            IIdentificationCoordinationService identificationCoordinationService,
            ILoggingBroker loggingBroker)
        {
            this.identificationCoordinationService = identificationCoordinationService;
            this.loggingBroker = loggingBroker;
        }

        //[Function("ProjectOutboxEventFunction")]
        //public async Task Run(
        //    [BlobTrigger("addresses/resolve/in/{name}", Connection = "BlobStorage")] Stream myBlob, string name)
        //{
        //    await loggingBroker
        //        .LogInformationAsync(
        //            $"C# Blob trigger function Processing blob\n " +
        //            $"Name: address/in/{{name}}");

        //    try
        //    {
        //        // TODO:  Refactor the identification service to take in the stream and name
        //        // await this.identificationCoordinationService.ProcessImpersonationContextRequestAsync(name, myBlob);
        //    }
        //    catch (Exception ex)
        //    {
        //        await loggingBroker.LogErrorAsync(ex);
        //        throw;
        //    }
        //}
    }
}
