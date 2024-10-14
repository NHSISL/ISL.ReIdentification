// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;

namespace ISL.ReIdentification.Core.Services.Foundations.OdsDatas
{
    public interface IOdsDataService
    {
        ValueTask<OdsData> AddOdsDataAsync(OdsData odsData);
        ValueTask<IQueryable<OdsData>> RetrieveAllOdsDatasAsync();
        ValueTask<OdsData> RetrieveOdsDataByIdAsync(Guid odsDataId);
        ValueTask<OdsData> ModifyOdsDataAsync(OdsData odsData);
        ValueTask<OdsData> RemoveOdsDataByIdAsync(Guid odsDataId);
    }
}