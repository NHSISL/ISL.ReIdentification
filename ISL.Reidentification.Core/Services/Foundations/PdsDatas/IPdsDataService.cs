// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;

namespace ISL.ReIdentification.Core.Services.Foundations.PdsDatas
{
    public interface IPdsDataService
    {
        ValueTask<PdsData> AddPdsDataAsync(PdsData pdsData);
        ValueTask<IQueryable<PdsData>> RetrieveAllPdsDatasAsync();
        ValueTask<PdsData> RetrievePdsDataByIdAsync(Guid pdsDataId);
        ValueTask<PdsData> ModifyPdsDataAsync(PdsData pdsData);
        ValueTask<PdsData> RemovePdsDataByIdAsync(Guid pdsDataId);
        ValueTask<bool> OrganisationsHaveAccessToThisPatient(string pseudoNhsNumber, List<string> organisationCodes);
    }
}