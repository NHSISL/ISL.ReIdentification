// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using ISL.ReIdentification.Core.Models.Brokers.Notifications;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.Notifications.Exceptions;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;

namespace ISL.ReIdentification.Core.Services.Foundations.Notifications
{
    public partial class NotificationService
    {
        private static void ValidateOnSendCsvPendingApprovalNotificationAsync(
            AccessRequest accessRequest,
            NotificationConfigurations notificationConfigurations)
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

        private static void ValidateOnSendCsvApprovedNotificationAsync(
            AccessRequest accessRequest,
            NotificationConfigurations notificationConfigurations)
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

        private static void ValidateOnSendCsvDeniedNotificationAsync(
            AccessRequest accessRequest,
            NotificationConfigurations notificationConfigurations)
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

        private static void ValidateOnSendImpersonationPendingApprovalNotificationAsync(
            AccessRequest accessRequest,
            NotificationConfigurations notificationConfigurations)
        {
            Validate(
                (Rule: IsInvalid(accessRequest), Parameter: nameof(AccessRequest)),
                (Rule: IsInvalid(notificationConfigurations), Parameter: nameof(NotificationConfigurations)));

            Validate(
                (Rule: IsInvalid(accessRequest.ImpersonationContext),
                Parameter: $"{nameof(AccessRequest)}.{nameof(AccessRequest.ImpersonationContext)}"),

                (Rule: IsInvalid(notificationConfigurations.ImpersonationPendingApprovalRequestTemplateId),
                Parameter: $"{nameof(NotificationConfigurations)}." +
                    $"{nameof(NotificationConfigurations.ImpersonationPendingApprovalRequestTemplateId)}"),

                (Rule: IsInvalid(notificationConfigurations.ConfigurationBaseUrl),
                Parameter: $"{nameof(NotificationConfigurations)}." +
                    $"{nameof(NotificationConfigurations.ConfigurationBaseUrl)}"),

                (Rule: IsInvalid(notificationConfigurations.PortalBaseUrl),
                Parameter: $"{nameof(NotificationConfigurations)}." +
                    $"{nameof(NotificationConfigurations.PortalBaseUrl)}"));
        }

        private static void ValidateOnSendImpersonationApprovedNotificationAsync(
            AccessRequest accessRequest,
            NotificationConfigurations notificationConfigurations)
        {
            Validate(
                (Rule: IsInvalid(accessRequest), Parameter: nameof(AccessRequest)),
                (Rule: IsInvalid(notificationConfigurations), Parameter: nameof(NotificationConfigurations)));

            Validate(
                (Rule: IsInvalid(accessRequest.ImpersonationContext),
                Parameter: $"{nameof(AccessRequest)}.{nameof(AccessRequest.ImpersonationContext)}"),

                (Rule: IsInvalid(notificationConfigurations.ImpersonationApprovedRequestTemplateId),
                Parameter: $"{nameof(NotificationConfigurations)}." +
                    $"{nameof(NotificationConfigurations.ImpersonationApprovedRequestTemplateId)}"),

                (Rule: IsInvalid(notificationConfigurations.ConfigurationBaseUrl),
                Parameter: $"{nameof(NotificationConfigurations)}." +
                    $"{nameof(NotificationConfigurations.ConfigurationBaseUrl)}"),

                (Rule: IsInvalid(notificationConfigurations.PortalBaseUrl),
                Parameter: $"{nameof(NotificationConfigurations)}." +
                    $"{nameof(NotificationConfigurations.PortalBaseUrl)}"));
        }

        private static void ValidateOnSendImpersonationDeniedNotificationAsync(
            AccessRequest accessRequest,
            NotificationConfigurations notificationConfigurations)
        {
            Validate(
                (Rule: IsInvalid(accessRequest), Parameter: nameof(AccessRequest)),
                (Rule: IsInvalid(notificationConfigurations), Parameter: nameof(NotificationConfigurations)));

            Validate(
                (Rule: IsInvalid(accessRequest.ImpersonationContext),
                Parameter: $"{nameof(AccessRequest)}.{nameof(AccessRequest.ImpersonationContext)}"),

                (Rule: IsInvalid(notificationConfigurations.ImpersonationDeniedRequestTemplateId),
                Parameter: $"{nameof(NotificationConfigurations)}." +
                    $"{nameof(NotificationConfigurations.ImpersonationDeniedRequestTemplateId)}"),

                (Rule: IsInvalid(notificationConfigurations.ConfigurationBaseUrl),
                Parameter: $"{nameof(NotificationConfigurations)}." +
                    $"{nameof(NotificationConfigurations.ConfigurationBaseUrl)}"),

                (Rule: IsInvalid(notificationConfigurations.PortalBaseUrl),
                Parameter: $"{nameof(NotificationConfigurations)}." +
                    $"{nameof(NotificationConfigurations.PortalBaseUrl)}"));
        }

        private static void ValidateOnSendImpersonationTokensGeneratedNotificationAsync(
            AccessRequest accessRequest,
            NotificationConfigurations notificationConfigurations)
        {
            Validate(
                (Rule: IsInvalid(accessRequest), Parameter: nameof(AccessRequest)),
                (Rule: IsInvalid(notificationConfigurations), Parameter: nameof(NotificationConfigurations)));

            Validate(
                (Rule: IsInvalid(accessRequest.ImpersonationContext),
                Parameter: $"{nameof(AccessRequest)}.{nameof(AccessRequest.ImpersonationContext)}"),

                (Rule: IsInvalid(notificationConfigurations.ImpersonationTokensGeneratedTemplateId),
                Parameter: $"{nameof(NotificationConfigurations)}." +
                    $"{nameof(NotificationConfigurations.ImpersonationTokensGeneratedTemplateId)}"),

                (Rule: IsInvalid(notificationConfigurations.ConfigurationBaseUrl),
                Parameter: $"{nameof(NotificationConfigurations)}." +
                    $"{nameof(NotificationConfigurations.ConfigurationBaseUrl)}"),

                (Rule: IsInvalid(notificationConfigurations.PortalBaseUrl),
                Parameter: $"{nameof(NotificationConfigurations)}." +
                    $"{nameof(NotificationConfigurations.PortalBaseUrl)}"));
        }

        private static void ValidateInputsOnSendCsvPendingApprovalNotificationAsync(
            string toEmail,
            string subject,
            string body,
            Dictionary<string, dynamic> personalisation)
        {
            Validate(
                (Rule: IsInvalid(toEmail), Parameter: nameof(toEmail)),
                (Rule: IsInvalid(subject), Parameter: nameof(subject)),
                (Rule: IsInvalid(body), Parameter: nameof(body)),
                (Rule: IsInvalid(personalisation), Parameter: nameof(personalisation)));
        }

        private static void ValidateInputsOnSendCsvApprovedNotificationAsync(
            string toEmail,
            string subject,
            string body,
            Dictionary<string, dynamic> personalisation)
        {
            Validate(
                (Rule: IsInvalid(toEmail), Parameter: nameof(toEmail)),
                (Rule: IsInvalid(subject), Parameter: nameof(subject)),
                (Rule: IsInvalid(body), Parameter: nameof(body)),
                (Rule: IsInvalid(personalisation), Parameter: nameof(personalisation)));
        }

        private static void ValidateInputsOnSendCsvDeniedNotificationAsync(
            string toEmail,
            string subject,
            string body,
            Dictionary<string, dynamic> personalisation)
        {
            Validate(
                (Rule: IsInvalid(toEmail), Parameter: nameof(toEmail)),
                (Rule: IsInvalid(subject), Parameter: nameof(subject)),
                (Rule: IsInvalid(body), Parameter: nameof(body)),
                (Rule: IsInvalid(personalisation), Parameter: nameof(personalisation)));
        }

        private static void ValidateInputsOnSendImpersonationPendingApprovalNotificationAsync(
            string toEmail,
            string subject,
            string body,
            Dictionary<string, dynamic> personalisation)
        {
            Validate(
                (Rule: IsInvalid(toEmail), Parameter: nameof(toEmail)),
                (Rule: IsInvalid(subject), Parameter: nameof(subject)),
                (Rule: IsInvalid(body), Parameter: nameof(body)),
                (Rule: IsInvalid(personalisation), Parameter: nameof(personalisation)));
        }

        private static void ValidateInputsOnSendImpersonationApprovedNotificationAsync(
            string toEmail,
            string subject,
            string body,
            Dictionary<string, dynamic> personalisation)
        {
            Validate(
                (Rule: IsInvalid(toEmail), Parameter: nameof(toEmail)),
                (Rule: IsInvalid(subject), Parameter: nameof(subject)),
                (Rule: IsInvalid(body), Parameter: nameof(body)),
                (Rule: IsInvalid(personalisation), Parameter: nameof(personalisation)));
        }

        private static void ValidateInputsOnSendImpersonationDeniedNotificationAsync(
            string toEmail,
            string subject,
            string body,
            Dictionary<string, dynamic> personalisation)
        {
            Validate(
                (Rule: IsInvalid(toEmail), Parameter: nameof(toEmail)),
                (Rule: IsInvalid(subject), Parameter: nameof(subject)),
                (Rule: IsInvalid(body), Parameter: nameof(body)),
                (Rule: IsInvalid(personalisation), Parameter: nameof(personalisation)));
        }

        private static dynamic IsInvalid(AccessRequest accessRequest) => new
        {
            Condition = accessRequest is null,
            Message = $"{nameof(AccessRequest)} is invalid"
        };

        private static dynamic IsInvalid(NotificationConfigurations notificationConfigurations) => new
        {
            Condition = notificationConfigurations is null,
            Message = $"{nameof(NotificationConfigurations)} is invalid"
        };

        private static dynamic IsInvalid(CsvIdentificationRequest csvIdentificationRequest) => new
        {
            Condition = csvIdentificationRequest is null,
            Message = $"{nameof(CsvIdentificationRequest)} is invalid"
        };

        private static dynamic IsInvalid(ImpersonationContext impersonationContext) => new
        {
            Condition = impersonationContext is null,
            Message = $"{nameof(ImpersonationContext)} is invalid"
        };

        private static dynamic IsInvalid(Dictionary<string, dynamic> personalisation) => new
        {
            Condition = personalisation is null,
            Message = $"Dictionary is invalid"
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
