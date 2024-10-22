// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading.Tasks;
using ISL.ReIdentification.Core.Brokers.Loggings;
using ISL.ReIdentification.Core.Brokers.Notifications;
using ISL.ReIdentification.Core.Models.Brokers.Notifications;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;

namespace ISL.ReIdentification.Core.Services.Foundations.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationConfigurations notificationConfigurations;
        private readonly INotificationBroker notificationBroker;
        private readonly ILoggingBroker loggingBroker;

        public NotificationService(
            NotificationConfigurations notificationConfigurations,
            INotificationBroker notificationBroker,
            ILoggingBroker loggingBroker)
        {
            this.notificationConfigurations = notificationConfigurations;
            this.notificationBroker = notificationBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask SendCsvPendingApprovalNotificationAsync(AccessRequest accessRequest) =>
            throw new System.NotImplementedException();

        public ValueTask SendCsvApprovedNotificationAsync(AccessRequest accessRequest) =>
            throw new System.NotImplementedException();

        public ValueTask SendCsvDeniedNotificationAsync(AccessRequest accessRequest) =>
            throw new System.NotImplementedException();

        public ValueTask SendImpersonationPendingApprovalNotificationAsync(AccessRequest accessRequest) =>
            throw new System.NotImplementedException();

        public ValueTask SendImpersonationApprovedNotificationAsync(AccessRequest accessRequest) =>
            throw new System.NotImplementedException();

        public ValueTask SendImpersonationDeniedNotificationAsync(AccessRequest accessRequest) =>
            throw new System.NotImplementedException();
    }
}
