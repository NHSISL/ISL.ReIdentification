// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Services.Orchestrations.Persists;
using ISL.ReIdentification.Functions.Models;
using Microsoft.Azure.Functions.Worker;

namespace LHDS.Functions.Addresses
{
    public class ResolvedAddressExporterTimerFunction
    {
        private readonly ILoggingBroker loggingBroker;
        private readonly IPersistanceOrchestrationService persistanceOrchestrationService;

        public ResolvedAddressExporterTimerFunction(
            ILoggingBroker loggingBroker,
            IPersistanceOrchestrationService persistanceOrchestrationService)
        {
            this.loggingBroker = loggingBroker;
            this.persistanceOrchestrationService = persistanceOrchestrationService;
        }

        [Function("ResolvedAddressExporterTimerFunction")]
        public async Task Run([TimerTrigger("0 */15 * * * *")] MyInformation myTimer)
        {
            await loggingBroker.LogInformationAsync($"C# Timer trigger function executed at: {DateTime.Now}");

            try
            {
                await persistanceOrchestrationService.PurgeCsvReIdentificationRecordsThatExpired();
            }
            catch (Exception ex)
            {
                await loggingBroker.LogErrorAsync(ex);
                throw;
            }

            await loggingBroker.LogInformationAsync($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }
    }
}
