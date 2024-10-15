// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;

namespace ISL.ReIdentification.Core.Services.Foundations.Notifications
{
    public interface INotificationService
    {
        ValueTask SendPendingApprovalNotificationAsync(AccessRequest accessRequest);
        ValueTask SendApprovedNotificationAsync(AccessRequest accessRequest);
        ValueTask SendDeniedNotificationAsync(AccessRequest accessRequest);
    }
}
