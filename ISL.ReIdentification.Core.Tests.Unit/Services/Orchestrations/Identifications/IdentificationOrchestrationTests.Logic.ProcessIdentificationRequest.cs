// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using Moq;

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

            IdentificationRequest randomIdentificationRequest =
               CreateRandomIdentificationRequest(hasAccess: false, itemCount: itemCount);

            IdentificationRequest inputIdentificationRequest = randomIdentificationRequest.DeepClone();
            IdentificationRequest outputIdentificationRequest = inputIdentificationRequest.DeepClone();

            outputIdentificationRequest.IdentificationItems.ForEach(item =>
            {
                item.Identifier = "0000000000";
                item.Message = "Failed to Re-Identify. User do not have access to the organisation(s) " + 
                    "associated with patient.";
            });

            IdentificationRequest expectedIdentificationRequest = outputIdentificationRequest.DeepClone();

            this.identifierBrokerMock.Setup(broker =>
                broker.GetIdentifierAsync())
                    .ReturnsAsync(randomGuid);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            var actualIdentificationRequest =
                await this.identificationOrchestrationService
                    .ProcessIdentificationRequestAsync(inputIdentificationRequest);

            // then
            actualIdentificationRequest.Should().BeEquivalentTo(expectedIdentificationRequest);

            this.identifierBrokerMock.Verify(broker =>
                broker.GetIdentifierAsync(),
                    Times.Exactly(itemCount));

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Exactly(itemCount));

            foreach (IdentificationItem item in randomIdentificationRequest.IdentificationItems)
            {
                AccessAudit inputAccessAudit = new AccessAudit
                {
                    Id = randomGuid,
                    PseudoIdentifier = item.Identifier,
                    EntraUserId = randomIdentificationRequest.EntraUserId,
                    GivenName = randomIdentificationRequest.GivenName,
                    Surname = randomIdentificationRequest.Surname,
                    Email = randomIdentificationRequest.Email,
                    Purpose = randomIdentificationRequest.Purpose,
                    Reason = randomIdentificationRequest.Reason,
                    Organisation = randomIdentificationRequest.Organisation,
                    HasAccess = (bool)item.HasAccess,
                    Message = item.Message,
                    CreatedBy = "System",
                    CreatedDate = randomDateTimeOffset,
                    UpdatedBy = "System",
                    UpdatedDate = randomDateTimeOffset
                };

                this.accessAuditServiceMock.Verify(service =>
                    service.AddAccessAuditAsync(It.Is(SameAccessAuditAs(inputAccessAudit))),
                        Times.Once);
            }

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

            IdentificationRequest randomIdentificationRequest =
                CreateRandomIdentificationRequest(hasAccess: true, itemCount: itemCount);

            IdentificationRequest inputIdentificationRequest = randomIdentificationRequest.DeepClone();
            IdentificationRequest outputIdentificationRequest = inputIdentificationRequest.DeepClone();

            outputIdentificationRequest.IdentificationItems.ForEach(item =>
            {
                item.Identifier = $"{item.Identifier}I";
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
                Purpose = inputIdentificationRequest.Purpose,
                Organisation = inputIdentificationRequest.Organisation,
                Reason = inputIdentificationRequest.Reason
            };

            IdentificationRequest outputHasAccessIdentificationRequest =
                inputHasAccessIdentificationRequest.DeepClone();

            outputHasAccessIdentificationRequest.IdentificationItems.ForEach(item =>
            {
                item.Identifier = $"{item.Identifier}I";
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
                    Times.Exactly(itemCount));

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Exactly(itemCount));

            foreach (IdentificationItem item in randomIdentificationRequest.IdentificationItems)
            {
                AccessAudit inputAccessAudit = new AccessAudit
                {
                    Id = randomGuid,
                    PseudoIdentifier = item.Identifier,
                    EntraUserId = randomIdentificationRequest.EntraUserId,
                    GivenName = randomIdentificationRequest.GivenName,
                    Surname = randomIdentificationRequest.Surname,
                    Email = randomIdentificationRequest.Email,
                    Purpose = randomIdentificationRequest.Purpose,
                    Reason = randomIdentificationRequest.Reason,
                    Organisation = randomIdentificationRequest.Organisation,
                    HasAccess = (bool)item.HasAccess,
                    Message = item.Message,
                    CreatedBy = "System",
                    CreatedDate = randomDateTimeOffset,
                    UpdatedBy = "System",
                    UpdatedDate = randomDateTimeOffset
                };

                this.accessAuditServiceMock.Verify(service =>
                    service.AddAccessAuditAsync(It.Is(SameAccessAuditAs(inputAccessAudit))),
                        Times.Once);
            }

            this.reIdentificationServiceMock.Verify(service =>
                service.ProcessReIdentificationRequest(It.Is(
                    SameIdentificationRequestAs(inputHasAccessIdentificationRequest))),
                        Times.Once);

            this.accessAuditServiceMock.VerifyNoOtherCalls();
            this.reIdentificationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.identifierBrokerMock.VerifyNoOtherCalls();
        }
    }
}
