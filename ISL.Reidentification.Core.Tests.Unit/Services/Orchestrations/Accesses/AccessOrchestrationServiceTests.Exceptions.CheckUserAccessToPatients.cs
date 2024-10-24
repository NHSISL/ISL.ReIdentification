// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Orchestrations.Accesses;
using ISL.ReIdentification.Core.Services.Orchestrations.Accesses;
using Moq;
using Xeptions;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Accesses
{
    public partial class AccessOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowAggregateOnCheckAccessToPatientAndLogItAsync()
        {
            // given
            List<Exception> exceptions = new List<Exception>();
            Xeption someException = new Xeption();

            var accessOrchestrationServiceMock = new Mock<AccessOrchestrationService>
                (this.userAccessServiceMock.Object,
                this.pdsDataServiceMock.Object,
                this.dateTimeBrokerMock.Object,
                this.loggingBrokerMock.Object)
            { CallBase = true };

            AccessRequest someAccessRequest = CreateRandomAccessRequest();
            string userOrganisation = GetRandomStringWithLength(5);

            List<string> userOrganisations =
                new List<string> { userOrganisation };

            this.pdsDataServiceMock.Setup(service =>
                 service.OrganisationsHaveAccessToThisPatient(It.IsAny<string>(), It.IsAny<List<string>>()))
                     .ThrowsAsync(someException);

            foreach (var identificationItem in someAccessRequest.IdentificationRequest.IdentificationItems)
            {
                var itemException = someException.DeepClone();
                itemException.AddData("IdentificationItemError", identificationItem.RowNumber);
                exceptions.Add(itemException);
            }

            var expectedAggregateException =
                new AggregateException(
                    message: $"Unable to validate access for {exceptions.Count} identification requests.",
                    innerExceptions: exceptions);

            AccessOrchestrationService service = accessOrchestrationServiceMock.Object;

            // when
            ValueTask<AccessRequest> checkAccessToPatientTask =
                service.CheckUserAccessToPatientsAsync(someAccessRequest, userOrganisations);

            AggregateException actualAggregateException =
                await Assert.ThrowsAsync<AggregateException>(
                    testCode: checkAccessToPatientTask.AsTask);

            // then
            actualAggregateException
                .Should().BeEquivalentTo(expectedAggregateException);

            accessOrchestrationServiceMock.Verify(service =>
                 service.CheckUserAccessToPatientsAsync(someAccessRequest, userOrganisations),
                    Times.Once);

            this.pdsDataServiceMock.Verify(service =>
                 service.OrganisationsHaveAccessToThisPatient(It.IsAny<string>(), It.IsAny<List<string>>()),
                    Times.Exactly(someAccessRequest.IdentificationRequest.IdentificationItems.Count));

            accessOrchestrationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.pdsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}
