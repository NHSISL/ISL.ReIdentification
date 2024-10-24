// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Orchestrations.Accesses;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Accesses
{
    public partial class AccessOrchestrationServiceTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ShouldSetHasAccessOnCheckUserAccessToPatientsAsync(bool userHasAccess)
        {
            // given
            var accessOrchestrationServiceMock = new Mock<AccessOrchestrationService>
                (this.userAccessServiceMock.Object,
                this.pdsDataServiceMock.Object,
                this.dateTimeBrokerMock.Object,
                this.loggingBrokerMock.Object)
            { CallBase = true };

            AccessRequest randomRequest = CreateRandomAccessRequest();
            AccessRequest inputAccessRequest = randomRequest.DeepClone();
            AccessRequest outputAccessRequest = inputAccessRequest.DeepClone();
            outputAccessRequest.IdentificationRequest.IdentificationItems.ForEach(item => item.HasAccess = userHasAccess);
            AccessRequest expectedAccessRequest = outputAccessRequest.DeepClone();

            string userOrganisation = GetRandomStringWithLength(5);

            List<string> userOrganisations =
                new List<string> { userOrganisation };

            this.pdsDataServiceMock.Setup(service =>
                service.OrganisationsHaveAccessToThisPatient(It.IsAny<string>(), It.IsAny<List<string>>()))
                    .ReturnsAsync(userHasAccess);

            AccessOrchestrationService service = accessOrchestrationServiceMock.Object;

            // when
            AccessRequest actualAccessRequest =
                await service.CheckUserAccessToPatientsAsync(inputAccessRequest, userOrganisations);

            // then
            actualAccessRequest.Should().BeEquivalentTo(expectedAccessRequest);

            accessOrchestrationServiceMock.Verify(service =>
                service.CheckUserAccessToPatientsAsync(inputAccessRequest, userOrganisations),
                    Times.Once());

            this.pdsDataServiceMock.Verify(service =>
                service.OrganisationsHaveAccessToThisPatient(It.IsAny<string>(), It.IsAny<List<string>>()),
                    Times.Exactly(inputAccessRequest.IdentificationRequest.IdentificationItems.Count));

            accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.pdsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}
