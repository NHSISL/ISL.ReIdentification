// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.Loggings;
using Microsoft.Azure.Functions.Worker;

namespace ISL.ReIdentification.Functions
{
    public class ResolvedAddressLoaderFunction
    {
        private readonly ILoggingBroker loggingBroker;

        public ResolvedAddressLoaderFunction(
            ILoggingBroker loggingBroker)
        {
            this.loggingBroker = loggingBroker;
        }

        [Function("ResolvedAddressLoaderFunction")]
        public async Task Run(
            [BlobTrigger("addresses/resolve/in/{name}", Connection = "BlobStorage")] Stream myBlob, string name)
        {
            await loggingBroker
                .LogInformationAsync(
                    $"C# Blob trigger function Processing blob\n " +
                    $"Name: address/in/{{name}}");

            try
            {
                // TODO: Implement the logic here
            }
            catch (Exception ex)
            {
                await loggingBroker.LogErrorAsync(ex);
                throw;
            }
        }
    }
}
