// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;

namespace ISL.ReIdentification.Core.Services.Foundations.Notifications
{
    public interface INotificationService
    {
        ValueTask SendCsvPendingApprovalNotificationAsync(AccessRequest accessRequest);
        ValueTask SendCsvApprovedNotificationAsync(AccessRequest accessRequest);
        ValueTask SendCsvDeniedNotificationAsync(AccessRequest accessRequest);
        ValueTask SendImpersonationPendingApprovalNotificationAsync(AccessRequest accessRequest);
        ValueTask SendImpersonationApprovedNotificationAsync(AccessRequest accessRequest);
        ValueTask SendImpersonationDeniedNotificationAsync(AccessRequest accessRequest);
        ValueTask SendImpersonationTokensGeneratedNotificationAsync(AccessRequest accessRequest);
    }
}
