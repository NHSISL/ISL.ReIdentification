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
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;

namespace ISL.ReIdentification.Core.Services.Foundations.PdsDatas
{
    public partial class PdsDataService : IPdsDataService
    {
        private readonly IReIdentificationStorageBroker reIdentificationStorageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ISecurityBroker securityBroker;
        private readonly ILoggingBroker loggingBroker;

        public PdsDataService(
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

        public ValueTask<PdsData> AddPdsDataAsync(PdsData pdsData) =>
            TryCatch(async () =>
            {
                PdsData pdsDataWithAddAuditApplied = await ApplyAddAuditAsync(pdsData);
                await ValidatePdsDataOnAddAsync(pdsDataWithAddAuditApplied);

                return await this.reIdentificationStorageBroker.InsertPdsDataAsync(pdsData);
            });

        public ValueTask<IQueryable<PdsData>> RetrieveAllPdsDatasAsync() =>
            TryCatch(this.reIdentificationStorageBroker.SelectAllPdsDatasAsync);

        public ValueTask<PdsData> RetrievePdsDataByIdAsync(Guid pdsDataId) =>
            TryCatch(async () =>
            {
                ValidatePdsDataId(pdsDataId);

                PdsData maybePdsData = await this.reIdentificationStorageBroker
                    .SelectPdsDataByIdAsync(pdsDataId);

                ValidateStoragePdsData(maybePdsData, pdsDataId);

                return maybePdsData;
            });

        public ValueTask<PdsData> ModifyPdsDataAsync(PdsData pdsData) =>
            TryCatch(async () =>
            {
                PdsData pdsDataWithAddAuditApplied = await ApplyModifyAuditAsync(pdsData);
                await ValidatePdsDataOnModifyAsync(pdsDataWithAddAuditApplied);

                PdsData maybePdsData =
                    await this.reIdentificationStorageBroker.SelectPdsDataByIdAsync(pdsDataWithAddAuditApplied.Id);

                ValidateStoragePdsData(maybePdsData, pdsDataWithAddAuditApplied.Id);

                ValidateAgainstStoragePdsDataOnModify(
                    inputPdsData: pdsDataWithAddAuditApplied, 
                    storagePdsData: maybePdsData);

                return await this.reIdentificationStorageBroker.UpdatePdsDataAsync(pdsDataWithAddAuditApplied);
            });

        public ValueTask<PdsData> RemovePdsDataByIdAsync(Guid pdsDataId) =>
            TryCatch(async () =>
            {
                ValidatePdsDataId(pdsDataId);

                PdsData maybePdsData = await this.reIdentificationStorageBroker
                    .SelectPdsDataByIdAsync(pdsDataId);

                ValidateStoragePdsData(maybePdsData, pdsDataId);

                return await this.reIdentificationStorageBroker.DeletePdsDataAsync(maybePdsData);
            });

        public ValueTask<bool> OrganisationsHaveAccessToThisPatient(
            string pseudoNhsNumber,
            List<string> organisationCodes) =>
            TryCatch(async () =>
            {
                ValidateOnOrganisationsHaveAccessToThisPatient(pseudoNhsNumber, organisationCodes);

                var query = await this.reIdentificationStorageBroker.SelectAllPdsDatasAsync();
                DateTimeOffset currentDateTime = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

                bool hasAccess = query.Any(
                    pdsData => pdsData.PseudoNhsNumber == pseudoNhsNumber
                    && organisationCodes.Contains(pdsData.OrgCode)
                    && (pdsData.RelationshipWithOrganisationEffectiveFromDate == null
                        || pdsData.RelationshipWithOrganisationEffectiveFromDate <= currentDateTime)
                    && (pdsData.RelationshipWithOrganisationEffectiveToDate == null ||
                        pdsData.RelationshipWithOrganisationEffectiveToDate > currentDateTime));

                return hasAccess;
            });

        virtual internal async ValueTask<PdsData> ApplyAddAuditAsync(PdsData pdsData)
        {
            ValidatePdsDataIsNotNull(pdsData);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            pdsData.CreatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            pdsData.CreatedDate = auditDateTimeOffset;
            pdsData.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            pdsData.UpdatedDate = auditDateTimeOffset;

            return pdsData;
        }

        virtual internal async ValueTask<PdsData> ApplyModifyAuditAsync(PdsData pdsData)
        {
            ValidatePdsDataIsNotNull(pdsData);
            var auditDateTimeOffset = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            var auditUser = await this.securityBroker.GetCurrentUserAsync();
            pdsData.UpdatedBy = auditUser?.EntraUserId.ToString() ?? string.Empty;
            pdsData.UpdatedDate = auditDateTimeOffset;

            return pdsData;
        }
    }
}