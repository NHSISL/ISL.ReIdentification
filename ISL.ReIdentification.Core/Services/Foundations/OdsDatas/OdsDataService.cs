// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
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

        public ValueTask<List<OdsData>> RetrieveChildrenByParentId(Guid odsDataParentId) =>
        TryCatch(async () =>
        {
            ValidateOdsDataId(odsDataParentId);

            OdsData parentRecord = await this.reIdentificationStorageBroker
                .SelectOdsDataByIdAsync(odsDataParentId);

            ValidateStorageOdsData(parentRecord, odsDataParentId);

            IQueryable<OdsData> query = await this.reIdentificationStorageBroker.SelectAllOdsDatasAsync();
            query = query.Where(ods => ods.OdsHierarchy.GetAncestor(1) == parentRecord.OdsHierarchy);
            List<OdsData> children = query.ToList();

            return children;
        });

        public ValueTask<List<OdsData>> RetrieveAllDecendentsByParentId(Guid odsDataParentId) =>
        TryCatch(async () =>
        {
            ValidateOdsDataId(odsDataParentId);

            OdsData parentRecord = await this.reIdentificationStorageBroker
                .SelectOdsDataByIdAsync(odsDataParentId);

            ValidateStorageOdsData(parentRecord, odsDataParentId);

            IQueryable<OdsData> query = await this.reIdentificationStorageBroker.SelectAllOdsDatasAsync();
            query = query.Where(ods => ods.OdsHierarchy.IsDescendantOf(parentRecord.OdsHierarchy));
            List<OdsData> descendants = query.ToList();

            return descendants;
        });
    }
}