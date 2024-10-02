﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Accesses
{
    public partial class AccessOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldGetOrganisationsForUser()
        {
            // given
            string userEmail = GetRandomStringWithLength(50);
            string inputUserEmail = userEmail;
            UserAccess randomUserAccess = CreateRandomUserAccess();
            randomUserAccess.UserEmail = inputUserEmail;

            IQueryable<UserAccess> randomUserAccesses =
                new List<UserAccess> { randomUserAccess }
                    .AsQueryable();

            IQueryable<UserAccess> storageUserAccesses = randomUserAccesses.DeepClone();
            OdsData randomOdsData = CreateRandomOdsData();
            randomOdsData.OrganisationCode_Root = randomUserAccess.OrgCode;
            randomOdsData.RelationshipStartDate = GetRandomPastDateTimeOffset();
            randomOdsData.RelationshipEndDate = GetRandomFutureDateTimeOffset();

            IQueryable<OdsData> randomOdsDatas =
                new List<OdsData> { randomOdsData }
                    .AsQueryable();

            List<string> expectedOrganisations =
                new List<string> { randomOdsData.OrganisationCode_Root };

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(DateTimeOffset.UtcNow);

            this.reIdentificationStorageBrokerMock.Setup(broker =>
                broker.SelectAllUserAccessesAsync())
                    .ReturnsAsync(storageUserAccesses);

            this.patientOrgReferenceStorageBrokerMock.Setup(broker =>
                broker.SelectAllOdsDatasAsync())
                    .ReturnsAsync(randomOdsDatas);

            // when
            List<string> actualOrganisations = await this.accessOrchestrationService
                .GetOrganisationsForUserAsync(inputUserEmail);

            // then
            actualOrganisations.Should().BeEquivalentTo(expectedOrganisations);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBrokerMock.Verify(broker =>
                broker.SelectAllUserAccessesAsync(),
                    Times.Once);

            this.patientOrgReferenceStorageBrokerMock.Verify(broker =>
                broker.SelectAllOdsDatasAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.reIdentificationStorageBrokerMock.VerifyNoOtherCalls();
            this.patientOrgReferenceStorageBrokerMock.VerifyNoOtherCalls();
        }
    }
}