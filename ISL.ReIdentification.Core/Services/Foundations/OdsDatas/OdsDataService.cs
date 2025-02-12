// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Securities;
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;

namespace ISL.ReIdentification.Core.Services.Foundations.OdsDatas
{
    public partial class OdsDataService : IOdsDataService
    {
        private readonly IReIdentificationStorageBroker reIdentificationStorageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ISecurityBroker securityBroker;
        private readonly ILoggingBroker loggingBroker;

        public OdsDataService(
            IReIdentificationStorageBroker reIdentificationStorageBroker,
            IDateTimeBroker dateTimeBroker,
            ISecurityBroker securityBroker,
            ILoggingBroker loggingBroker)
        {
            this.reIdentificationStorageBroker = reIdentificationStorageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.securityBroker = securityBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<OdsData> AddOdsDataAsync(OdsData odsData) =>
        TryCatch(async () =>
        {
            OdsData odsDataWithAddAuditApplied = await ApplyAddAuditAsync(odsData);
            await ValidateOdsDataOnAddAsync(odsDataWithAddAuditApplied);

            return await this.reIdentificationStorageBroker.InsertOdsDataAsync(odsDataWithAddAuditApplied);
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
            OdsData odsDataWithAddAuditApplied = await ApplyModifyAuditAsync(odsData);
            await ValidateOdsDataOnModifyAsync(odsDataWithAddAuditApplied);

            OdsData maybeOdsData =
                await this.reIdentificationStorageBroker.SelectOdsDataByIdAsync(odsDataWithAddAuditApplied.Id);

            ValidateStorageOdsData(maybeOdsData, odsDataWithAddAuditApplied.Id);
            
            ValidateAgainstStorageOdsDataOnModify(
                inputOdsData: odsDataWithAddAuditApplied, 
                storageOdsData: maybeOdsData);

            return await this.reIdentificationStorageBroker.UpdateOdsDataAsync(odsDataWithAddAuditApplied);
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

        public ValueTask<List<OdsData>> RetrieveAllAncestorsByChildId(Guid odsDataChildId) =>
        TryCatch(async () =>
        {
            ValidateOdsDataId(odsDataChildId);

            OdsData childRecord = await this.reIdentificationStorageBroker
                 .SelectOdsDataByIdAsync(odsDataChildId);

            ValidateStorageOdsData(childRecord, odsDataChildId);
            OdsData currentNode = childRecord;
            List<OdsData> ancestors = new List<OdsData>();
            ancestors.Add(currentNode);
            IQueryable<OdsData> odsDatas = await this.reIdentificationStorageBroker.SelectAllOdsDatasAsync();

            while (currentNode.OdsHierarchy.GetLevel() >= 1)
            {
                int level = currentNode.OdsHierarchy.GetLevel();

                OdsData ancestor = odsDatas.FirstOrDefault(odsData =>
                    odsData.OdsHierarchy == currentNode.OdsHierarchy.GetAncestor(1));

                if (ancestor is not null)
                {
                    ancestors.Add(ancestor);
                    currentNode = ancestor;
                }
                else
                {
                    break;
                }
            }

            return ancestors;
        });

        virtual internal async ValueTask<OdsData> ApplyAddAuditAsync(OdsData odsData)
        {
            ValidateOdsDataIsNotNull(odsData);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            odsData.CreatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            odsData.CreatedDate = auditDateTimeOffset;
            odsData.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            odsData.UpdatedDate = auditDateTimeOffset;

            return odsData;
        }

        virtual internal async ValueTask<OdsData> ApplyModifyAuditAsync(OdsData odsData)
        {
            ValidateOdsDataIsNotNull(odsData);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            odsData.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            odsData.UpdatedDate = auditDateTimeOffset;

            return odsData;
        }
    }
}