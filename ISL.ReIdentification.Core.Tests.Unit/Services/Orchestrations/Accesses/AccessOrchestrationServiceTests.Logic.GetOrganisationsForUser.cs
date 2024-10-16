﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Orchestrations.Accesses
{
    public partial class AccessOrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(GetOrganisationsReturnsOrgs))]
        public async Task ShouldGetOrganisationsForUserReturnsOrganisations(UserAccess returnedUserAccess)
        {
            // given
            Guid inputEntraUserId = returnedUserAccess.EntraUserId;

            IQueryable<UserAccess> returnedUserAccesses =
                new List<UserAccess> { returnedUserAccess }
                    .AsQueryable();

            List<string> expectedOrganisations =
                new List<string> { returnedUserAccess.OrgCode };

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(DateTimeOffset.UtcNow);

            this.userAccessServiceMock.Setup(service =>
                service.RetrieveAllUserAccessesAsync())
                    .ReturnsAsync(returnedUserAccesses);

            // when
            List<string> actualOrganisations = await this.accessOrchestrationService
                .GetOrganisationsForUserAsync(inputEntraUserId);

            // then
            actualOrganisations.Should().BeEquivalentTo(expectedOrganisations);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.userAccessServiceMock.Verify(service =>
                service.RetrieveAllUserAccessesAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.pdsDataServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(GetOrganisationsReturnsNoOrgs))]
        public async Task ShouldGetOrganisationsForUserReturnsNoOrganisations(
            UserAccess returnedUserAccess, bool changeEntraUserId)
        {
            // given
            DateTimeOffset currentDateTimeOffset = DateTimeOffset.UtcNow;
            Guid inputEntraUserId = returnedUserAccess.EntraUserId;
            Guid differentEntraUserId = Guid.NewGuid();

            if (changeEntraUserId)
            {
                returnedUserAccess.EntraUserId = differentEntraUserId;
            }

            IQueryable<UserAccess> returnedUserAccesses =
                new List<UserAccess> { returnedUserAccess }
                    .AsQueryable();

            List<string> expectedOrganisations =
                new List<string>();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(currentDateTimeOffset);

            this.userAccessServiceMock.Setup(service =>
                service.RetrieveAllUserAccessesAsync())
                    .ReturnsAsync(returnedUserAccesses);

            // when
            List<string> actualOrganisations = await this.accessOrchestrationService
                .GetOrganisationsForUserAsync(inputEntraUserId);

            // then
            actualOrganisations.Should().BeEquivalentTo(expectedOrganisations);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.userAccessServiceMock.Verify(service =>
                service.RetrieveAllUserAccessesAsync(),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userAccessServiceMock.VerifyNoOtherCalls();
            this.pdsDataServiceMock.VerifyNoOtherCalls();
        }
    }
}
