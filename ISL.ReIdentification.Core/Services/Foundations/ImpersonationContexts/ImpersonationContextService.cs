// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;

namespace ISL.ReIdentification.Core.Services.Foundations.ImpersonationContexts
{
    public partial class ImpersonationContextService : IImpersonationContextService
    {
        private readonly IReIdentificationStorageBroker reIdentificationStorageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ImpersonationContextService(
            IReIdentificationStorageBroker reIdentificationStorageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.reIdentificationStorageBroker = reIdentificationStorageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }
        public ValueTask<ImpersonationContext> AddImpersonationContextAsync(ImpersonationContext impersonationContext) =>
            TryCatch(async () =>
            {
                await ValidateImpersonationContextOnAddAsync(impersonationContext);

                return await this.reIdentificationStorageBroker.InsertImpersonationContextAsync(impersonationContext);
            });

        public ValueTask<ImpersonationContext> RetrieveImpersonationContextByIdAsync(Guid impersonationContextId) =>
            TryCatch(async () =>
            {
                ValidateImpersonationContextId(impersonationContextId);

                ImpersonationContext maybeImpersonationContext =
                    await this.reIdentificationStorageBroker.SelectImpersonationContextByIdAsync(impersonationContextId);

                ValidateStorageImpersonationContext(maybeImpersonationContext, impersonationContextId);

                return maybeImpersonationContext;
            });

        public ValueTask<IQueryable<ImpersonationContext>> RetrieveAllImpersonationContextsAsync() =>
            TryCatch(this.reIdentificationStorageBroker.SelectAllImpersonationContextsAsync);

        public ValueTask<ImpersonationContext> ModifyImpersonationContextAsync(ImpersonationContext impersonationContext) =>
            TryCatch(async () =>
            {
                await ValidateImpersonationContextOnModifyAsync(impersonationContext);

                ImpersonationContext maybeImpersonationContext =
                    await this.reIdentificationStorageBroker.SelectImpersonationContextByIdAsync(impersonationContext.Id);

                ValidateStorageImpersonationContext(maybeImpersonationContext, impersonationContext.Id);
                ValidateAgainstStorageImpersonationContextOnModify(impersonationContext, maybeImpersonationContext);

                return await this.reIdentificationStorageBroker.UpdateImpersonationContextAsync(impersonationContext);
            });

        public ValueTask<ImpersonationContext> RemoveImpersonationContextByIdAsync(Guid impersonationContextId) =>
            TryCatch(async () =>
            {
                ValidateImpersonationContextId(impersonationContextId);

                ImpersonationContext maybeImpersonationContext =
                    await this.reIdentificationStorageBroker.SelectImpersonationContextByIdAsync(impersonationContextId);

                ValidateStorageImpersonationContext(maybeImpersonationContext, impersonationContextId);

                return await this.reIdentificationStorageBroker.DeleteImpersonationContextAsync(maybeImpersonationContext);
            });
    }
}
