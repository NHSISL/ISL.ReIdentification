// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;

namespace ISL.ReIdentification.Core.Services.Foundations.OdsDatas
{
    public partial class OdsDataService : IOdsDataService
    {
        private readonly IReIdentificationStorageBroker reIdentificationStorageBroker;
        private readonly ILoggingBroker loggingBroker;

        public OdsDataService(
            IReIdentificationStorageBroker reIdentificationStorageBroker,
            ILoggingBroker loggingBroker)
        {
            this.reIdentificationStorageBroker = reIdentificationStorageBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<OdsData> AddOdsDataAsync(OdsData odsData) =>
            TryCatch(async () =>
            {
                await ValidateOdsDataOnAddAsync(odsData);

                return await this.reIdentificationStorageBroker.InsertOdsDataAsync(odsData);
            });

        public ValueTask<IQueryable<OdsData>> RetrieveAllOdsDatasAsync() =>
            TryCatch(this.reIdentificationStorageBroker.SelectAllOdsDatasAsync);

        public ValueTask<OdsData> RetrieveOdsDataByIdAsync(Guid odsDataId) =>
            TryCatch(async () =>
            {
                ValidateOdsDataId(odsDataId);

                OdsData maybeOdsData = await this.reIdentificationStorageBroker
                    .SelectOdsDataByIdAsync(odsDataId);

                ValidateStorageOdsData(maybeOdsData, odsDataId);

                return maybeOdsData;
            });

        public ValueTask<OdsData> ModifyOdsDataAsync(OdsData odsData) =>
            TryCatch(async () =>
            {
                await ValidateOdsDataOnModifyAsync(odsData);

                OdsData maybeOdsData =
                    await this.reIdentificationStorageBroker.SelectOdsDataByIdAsync(odsData.Id);

                ValidateStorageOdsData(maybeOdsData, odsData.Id);
                ValidateAgainstStorageOdsDataOnModify(inputOdsData: odsData, storageOdsData: maybeOdsData);

                return await this.reIdentificationStorageBroker.UpdateOdsDataAsync(odsData);
            });

        public ValueTask<OdsData> RemoveOdsDataByIdAsync(Guid odsDataId) =>
            TryCatch(async () =>
            {
                ValidateOdsDataId(odsDataId);

                OdsData maybeOdsData = await this.reIdentificationStorageBroker
                    .SelectOdsDataByIdAsync(odsDataId);

                ValidateStorageOdsData(maybeOdsData, odsDataId);

                return await this.reIdentificationStorageBroker.DeleteOdsDataAsync(maybeOdsData);
            });
    }
}