// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Models.Orchestrations.Persists.Exceptions;
using Xeptions;

namespace ISL.ReIdentification.Core.Services.Orchestrations.Persists
{
    public partial class PersistanceOrchestrationService
    {
        private delegate ValueTask<AccessRequest> ReturningAccessRequestFunction();

        private async ValueTask<AccessRequest> TryCatch(ReturningAccessRequestFunction returningAccessRequestFunction)
        {
            try
            {
                return await returningAccessRequestFunction();
            }
            catch (InvalidArgumentPersistanceOrchestrationException invalidArgumentPersistanceOrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidArgumentPersistanceOrchestrationException);
            }
        }

        private async ValueTask<PersistanceOrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var persistanceOrchestrationValidationException =
                new PersistanceOrchestrationValidationException(
                    message: "Persistance orchestration validation error occurred, please fix errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(persistanceOrchestrationValidationException);

            return persistanceOrchestrationValidationException;
        }
    }
}
