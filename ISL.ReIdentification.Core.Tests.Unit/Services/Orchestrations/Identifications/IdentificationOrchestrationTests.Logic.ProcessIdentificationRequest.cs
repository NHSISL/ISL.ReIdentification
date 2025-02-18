// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using Moq;
using Valid8R;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Identifications
{
    public partial class IdentificationOrchestrationTests
    {
        [Fact]
        public async Task ShouldCreateAuditAccessRecordIfRequestItemDoesNotHaveAccessAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Guid randomGuid = Guid.NewGuid();
            int itemCount = GetRandomNumber();
            bool hasAccess = false;
            var noAccessMessage = "User does not have access to the organisation(s) " +
                "associated with patient.  Re-identification blocked.";

            var accessMessage = "User does have access to the organisation(s) " +
                "associated with patient.  Item will be submitted for re-identification.";

            var auditType = "PDS Access";

            IdentificationRequest randomIdentificationRequest =
               CreateRandomIdentificationRequest(hasAccess, itemCount: itemCount);

            IdentificationRequest inputIdentificationRequest = randomIdentificationRequest.DeepClone();
            IdentificationRequest outputIdentificationRequest = inputIdentificationRequest.DeepClone();

            outputIdentificationRequest.IdentificationItems.ForEach(item =>
            {
                item.Identifier = "0000000000";
                item.Message = item.HasAccess ? accessMessage : noAccessMessage;
            });

            IdentificationRequest expectedIdentificationRequest = outputIdentificationRequest.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
               broker.GetCurrentDateTimeOffsetAsync())
                   .ReturnsAsync(randomDateTimeOffset);

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ReturnsAsync(randomGuid);

            // when
            var actualIdentificationRequest =
                await this.identificationOrchestrationService
                    .ProcessIdentificationRequestAsync(inputIdentificationRequest);

            // then
            actualIdentificationRequest.Should().BeEquivalentTo(expectedIdentificationRequest);

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Exactly(itemCount + 1));

            this.dateTimeBrokerMock.Verify(broker =>
               broker.GetCurrentDateTimeOffsetAsync(),
                   Times.Exactly(3));

            this.loggingBrokerMock.Verify(broker =>
                broker.LogInformationAsync($"Start ReId Check {randomDateTimeOffset}, TransactionId {randomGuid}"),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogInformationAsync($"Start PDS Check {randomDateTimeOffset}, TransactionId {randomGuid}"),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogInformationAsync($"Completed PDS Request {randomDateTimeOffset}, TransactionId {randomGuid}"),
                    Times.Once);

            List<AccessAudit> pdsAccessAudits = new List<AccessAudit>();

            foreach (IdentificationItem item in randomIdentificationRequest.IdentificationItems)
            {
                AccessAudit inputAccessAudit = new AccessAudit
                {
                    Id = randomGuid,
                    RequestId = randomIdentificationRequest.Id,
                    TransactionId = randomGuid,
                    PseudoIdentifier = item.Identifier,
                    EntraUserId = randomIdentificationRequest.EntraUserId,
                    GivenName = randomIdentificationRequest.GivenName,
                    Surname = randomIdentificationRequest.Surname,
                    Email = randomIdentificationRequest.Email,
                    Reason = randomIdentificationRequest.Reason,
                    Organisation = randomIdentificationRequest.Organisation,
                    HasAccess = hasAccess,
                    Message = item.HasAccess ? accessMessage : noAccessMessage,
                    AuditType = auditType
                };

                pdsAccessAudits.Add(inputAccessAudit);
            }

            this.accessAuditServiceMock.Verify(service =>
                service.BulkAddAccessAuditAsync(
                    It.Is(Valid8.SameObjectAs<List<AccessAudit>>(pdsAccessAudits, testOutputHelper, ""))),
                        Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.reIdentificationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldCreateAuditAccessRecordAndPerformReIdentificationIfRequestItemHasAccessAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Guid randomGuid = Guid.NewGuid();
            int itemCount = GetRandomNumber();
            string randomString = GetRandomStringWithLength(10);
            string reIdentifiedIdentifier = randomString;
            bool hasAccess = true;

            var noAccessMessage = "User does not have access to the organisation(s) " +
                "associated with patient.  Re-identification blocked.";

            var accessMessage = "User does have access to the organisation(s) " +
                "associated with patient.  Item will be submitted for re-identification.";

            IdentificationRequest randomIdentificationRequest =
                CreateRandomIdentificationRequest(hasAccess, itemCount: itemCount);

            IdentificationRequest inputIdentificationRequest = randomIdentificationRequest.DeepClone();
            IdentificationRequest outputIdentificationRequest = inputIdentificationRequest.DeepClone();

            outputIdentificationRequest.IdentificationItems.ForEach(item =>
            {
                item.Identifier = string.IsNullOrEmpty(item.Identifier)
                    ? item.Identifier
                    : $"{item.Identifier.PadLeft(10, '0')}I";

                item.IsReidentified = true;
            });

            IdentificationRequest expectedIdentificationRequest = outputIdentificationRequest.DeepClone();

            IdentificationRequest inputHasAccessIdentificationRequest = new IdentificationRequest
            {
                Id = inputIdentificationRequest.Id,
                IdentificationItems = inputIdentificationRequest.IdentificationItems,
                EntraUserId = inputIdentificationRequest.EntraUserId,
                GivenName = inputIdentificationRequest.GivenName,
                Surname = inputIdentificationRequest.Surname,
                DisplayName = inputIdentificationRequest.DisplayName,
                JobTitle = inputIdentificationRequest.JobTitle,
                Email = inputIdentificationRequest.Email,
                Organisation = inputIdentificationRequest.Organisation,
                Reason = inputIdentificationRequest.Reason
            };

            IdentificationRequest outputHasAccessIdentificationRequest =
                inputHasAccessIdentificationRequest.DeepClone();

            outputHasAccessIdentificationRequest.IdentificationItems.ForEach(item =>
            {
                item.Identifier = string.IsNullOrEmpty(item.Identifier)
                    ? item.Identifier
                    : $"{item.Identifier.PadLeft(10, '0')}I";

                item.IsReidentified = true;
            });

            this.identifierBrokerMock.Setup(broker =>
               broker.GetIdentifierAsync())
                   .ReturnsAsync(randomGuid);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.reIdentificationServiceMock.Setup(service =>
                service.ProcessReIdentificationRequest(
                    It.Is(SameIdentificationRequestAs(inputHasAccessIdentificationRequest))))
                        .ReturnsAsync(outputHasAccessIdentificationRequest);

            // when
            var actualIdentificationRequest =
                await this.identificationOrchestrationService
                    .ProcessIdentificationRequestAsync(inputIdentificationRequest);

            // then
            actualIdentificationRequest.Should().BeEquivalentTo(expectedIdentificationRequest);

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Exactly((itemCount * 2) + 1));

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Exactly(itemCount + 6));

            this.loggingBrokerMock.Verify(broker =>
                broker.LogInformationAsync($"Start ReId Check {randomDateTimeOffset}, TransactionId {randomGuid}"),
                   Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogInformationAsync($"Start PDS Check {randomDateTimeOffset}, TransactionId {randomGuid}"),
                  Times.Once);

            this.loggingBrokerMock.Verify(broker =>
              broker.LogInformationAsync($"Completed PDS Request {randomDateTimeOffset}, TransactionId {randomGuid}"),
                 Times.Once);

            this.loggingBrokerMock.Verify(broker =>
              broker.LogInformationAsync($"Start NECS Check {randomDateTimeOffset}, TransactionId {randomGuid}"),
                 Times.Once);

            this.loggingBrokerMock.Verify(broker =>
              broker.LogInformationAsync($"Completed NECS Request {randomDateTimeOffset}, TransactionId {randomGuid}"),
                 Times.Once);

            this.loggingBrokerMock.Verify(broker =>
              broker.LogInformationAsync($"Completed ReID {randomDateTimeOffset}, TransactionId {randomGuid}"),
                 Times.Once);

            List<AccessAudit> pdsAccessAudits = new List<AccessAudit>();

            foreach (IdentificationItem item in randomIdentificationRequest.IdentificationItems)
            {
                AccessAudit inputAccessAudit = new AccessAudit
                {
                    Id = randomGuid,
                    RequestId = randomIdentificationRequest.Id,
                    TransactionId = randomGuid,
                    PseudoIdentifier = item.Identifier,
                    EntraUserId = randomIdentificationRequest.EntraUserId,
                    GivenName = randomIdentificationRequest.GivenName,
                    Surname = randomIdentificationRequest.Surname,
                    Email = randomIdentificationRequest.Email,
                    Reason = randomIdentificationRequest.Reason,
                    Organisation = randomIdentificationRequest.Organisation,
                    HasAccess = item.HasAccess,
                    Message = item.HasAccess ? accessMessage : noAccessMessage,
                    AuditType = "PDS Access",
                };

                pdsAccessAudits.Add(inputAccessAudit);
            }

            this.accessAuditServiceMock.Verify(service =>
                service.BulkAddAccessAuditAsync(
                    It.Is(Valid8.SameObjectAs<List<AccessAudit>>(pdsAccessAudits, testOutputHelper, ""))),
                        Times.Once);

            this.reIdentificationServiceMock.Verify(service =>
                service.ProcessReIdentificationRequest(It.Is(
                    SameIdentificationRequestAs(inputHasAccessIdentificationRequest))),
                        Times.Once);

            List<AccessAudit> necsAccessAudits = new List<AccessAudit>();

            foreach (IdentificationItem item in randomIdentificationRequest.IdentificationItems)
            {
                var pseudoIdentifier = randomIdentificationRequest.IdentificationItems
                    .FirstOrDefault(identificationItem => identificationItem.RowNumber == item.RowNumber)
                        .Identifier;

                AccessAudit successAccessAudit = new AccessAudit
                {
                    Id = randomGuid,
                    RequestId = randomIdentificationRequest.Id,
                    TransactionId = randomGuid,

                    PseudoIdentifier = string.IsNullOrEmpty(pseudoIdentifier)
                        ? pseudoIdentifier
                        : pseudoIdentifier.PadLeft(10, '0'),

                    EntraUserId = randomIdentificationRequest.EntraUserId,
                    GivenName = randomIdentificationRequest.GivenName,
                    Surname = randomIdentificationRequest.Surname,
                    Email = randomIdentificationRequest.Email,
                    Reason = randomIdentificationRequest.Reason,
                    Organisation = randomIdentificationRequest.Organisation,
                    HasAccess = item.HasAccess,
                    AuditType = "NECS Access",
                    Message = $"Re-identification outcome: {item.Message}"
                };

                necsAccessAudits.Add(successAccessAudit);
            }

            this.accessAuditServiceMock.Verify(service =>
                service.BulkAddAccessAuditAsync(
                    It.Is(Valid8.SameObjectAs<List<AccessAudit>>(necsAccessAudits, testOutputHelper, ""))),
                        Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.reIdentificationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
