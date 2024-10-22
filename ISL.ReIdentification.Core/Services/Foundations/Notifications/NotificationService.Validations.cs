// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using ISL.ReIdentification.Core.Models.Brokers.Notifications;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.Notifications.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;

namespace ISL.ReIdentification.Core.Services.Foundations.Notifications
{
    public partial class NotificationService
    {
        private static void ValidateOnSendCsvPendingApprovalNotificationAsync(
            AccessRequest? accessRequest,
            NotificationConfigurations? notificationConfigurations)
        {
            Validate(
                (Rule: IsInvalid(accessRequest), Parameter: nameof(AccessRequest)),
                (Rule: IsInvalid(notificationConfigurations), Parameter: nameof(NotificationConfigurations)));

            Validate(
                (Rule: IsInvalid(accessRequest.CsvIdentificationRequest),
                Parameter: $"{nameof(AccessRequest)}.{nameof(AccessRequest.CsvIdentificationRequest)}"),

                (Rule: IsInvalid(notificationConfigurations.CsvPendingApprovalRequestTemplateId),
                Parameter: $"{nameof(NotificationConfigurations)}." +
                    $"{nameof(NotificationConfigurations.CsvPendingApprovalRequestTemplateId)}"),

                (Rule: IsInvalid(notificationConfigurations.ConfigurationBaseUrl),
                Parameter: $"{nameof(NotificationConfigurations)}." +
                    $"{nameof(NotificationConfigurations.ConfigurationBaseUrl)}"),

                (Rule: IsInvalid(notificationConfigurations.PortalBaseUrl),
                Parameter: $"{nameof(NotificationConfigurations)}." +
                    $"{nameof(NotificationConfigurations.PortalBaseUrl)}"));
        }

        private static dynamic IsInvalid(AccessRequest? accessRequest) => new
        {
            Condition = accessRequest is null,
            Message = $"{nameof(AccessRequest)} is invalid"
        };

        private static dynamic IsInvalid(NotificationConfigurations? notificationConfigurations) => new
        {
            Condition = notificationConfigurations is null,
            Message = $"{nameof(NotificationConfigurations)} is invalid"
        };

        private static dynamic IsInvalid(CsvIdentificationRequest? csvIdentificationRequest) => new
        {
            Condition = csvIdentificationRequest is null,
            Message = $"{nameof(CsvIdentificationRequest)} is invalid"
        };

        private static dynamic IsInvalid(string name) => new
        {
            Condition = String.IsNullOrWhiteSpace(name),
            Message = "Text is invalid"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidArgumentsNotificationException =
                new InvalidArgumentsNotificationException(
                    message: "Invalid notification arguments. Please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidArgumentsNotificationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidArgumentsNotificationException.ThrowIfContainsErrors();
        }
    }
}
