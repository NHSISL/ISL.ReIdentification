// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using Newtonsoft.Json.Linq;

namespace ISL.ReIdentification.Core.Services.Foundations.OdsDatas
{
    public interface IOdsDataService
    {
        ValueTask<OdsData> AddOdsDataAsync(OdsData odsData);
        ValueTask<IQueryable<OdsData>> RetrieveAllOdsDatasAsync();
        ValueTask<OdsData> RetrieveOdsDataByIdAsync(Guid odsDataId);
        ValueTask<OdsData> ModifyOdsDataAsync(OdsData odsData);
        ValueTask<OdsData> RemoveOdsDataByIdAsync(Guid odsDataId);
        ValueTask<IQueryable<OdsData>> GetChildren(Guid odsDataId);
    }
}