// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.DateTimes;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;

namespace ISL.ReIdentification.Core.Services.Foundations.PdsDatas
{
    public partial class PdsDataService : IPdsDataService
    {
        private readonly IReIdentificationStorageBroker reIdentificationStorageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public PdsDataService(
            IReIdentificationStorageBroker reIdentificationStorageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.reIdentificationStorageBroker = reIdentificationStorageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<PdsData> AddPdsDataAsync(PdsData pdsData) =>
            TryCatch(async () =>
            {
                ValidatePdsDataOnAdd(pdsData);

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
                ValidatePdsDataOnModify(pdsData);

                PdsData maybePdsData =
                    await this.reIdentificationStorageBroker.SelectPdsDataByIdAsync(pdsData.Id);

                ValidateStoragePdsData(maybePdsData, pdsData.Id);
                ValidateAgainstStoragePdsDataOnModify(inputPdsData: pdsData, storagePdsData: maybePdsData);

                return await this.reIdentificationStorageBroker.UpdatePdsDataAsync(pdsData);
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
                    && organisationCodes.Contains(pdsData.OrgCode));

                return hasAccess;
            });
    }
}