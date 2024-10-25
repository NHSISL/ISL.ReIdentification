// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Foundations.PdsDatas
{
    public partial class PdsDataServiceTests
    {
        [Fact]
        public async Task ShouldCheckIfOrganisationsHaveAccessToThisPatientAsync()
        {
            // given
            string randomPseudoNhsNumber = GetRandomString();
            string inputPseudoNhsNumber = randomPseudoNhsNumber;
            List<PdsData> randomPdsDatas = CreateRandomPdsDatas();
            randomPdsDatas.ForEach(pdsData => pdsData.PseudoNhsNumber = inputPseudoNhsNumber);
            List<PdsData> storagePdsDatas = randomPdsDatas;
            List<string> inputOrganisationCodes = randomPdsDatas.Select(pdsData => pdsData.OrgCode).ToList();
            bool expectedResult = true;

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllPdsDatasAsync())
                    .ReturnsAsync(storagePdsDatas.AsQueryable());

            this.dateTimeBroker.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(DateTimeOffset.UtcNow);

            // when
            bool actualResult =
                await this.pdsDataService.OrganisationsHaveAccessToThisPatient(
                    pseudoNhsNumber: inputPseudoNhsNumber, organisationCodes: inputOrganisationCodes);

            // then
            actualResult.Should().Be(expectedResult);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllPdsDatasAsync(),
                    Times.Once);

            this.dateTimeBroker.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldNotHaveAccessOnCheckIfOrganisationsHaveAccessToThisPatientWithInvalidPseudonumberAsync()
        {
            // given
            string randomPseudoNhsNumber = GetRandomString();
            string inputPseudoNhsNumber = randomPseudoNhsNumber;
            List<PdsData> randomPdsDatas = CreateRandomPdsDatas();
            List<PdsData> storagePdsDatas = randomPdsDatas;
            List<string> inputOrganisationCodes = randomPdsDatas.Select(pdsData => pdsData.OrgCode).ToList();
            bool expectedResult = false;

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllPdsDatasAsync())
                    .ReturnsAsync(storagePdsDatas.AsQueryable());

            this.dateTimeBroker.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(DateTimeOffset.UtcNow);

            // when
            bool actualResult =
                await this.pdsDataService.OrganisationsHaveAccessToThisPatient(
                    pseudoNhsNumber: inputPseudoNhsNumber, organisationCodes: inputOrganisationCodes);

            // then
            actualResult.Should().Be(expectedResult);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllPdsDatasAsync(),
                    Times.Once);

            this.dateTimeBroker.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldNotHaveAccessOnCheckIfOrganisationsHaveAccessToThisPatientWithInvalidOrganisationsAsync()
        {
            // given
            string randomPseudoNhsNumber = GetRandomString();
            string inputPseudoNhsNumber = randomPseudoNhsNumber;
            List<PdsData> randomPdsDatas = CreateRandomPdsDatas();
            randomPdsDatas.ForEach(pdsData => pdsData.PseudoNhsNumber = inputPseudoNhsNumber);
            List<PdsData> storagePdsDatas = randomPdsDatas;
            List<string> inputOrganisationCodes = GetRandomStringsWithLengthOf(10);
            bool expectedResult = false;

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllPdsDatasAsync())
                    .ReturnsAsync(storagePdsDatas.AsQueryable());

            this.dateTimeBroker.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(DateTimeOffset.UtcNow);

            // when
            bool actualResult =
                await this.pdsDataService.OrganisationsHaveAccessToThisPatient(
                    pseudoNhsNumber: inputPseudoNhsNumber, organisationCodes: inputOrganisationCodes);

            // then
            actualResult.Should().Be(expectedResult);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllPdsDatasAsync(),
                    Times.Once);

            this.dateTimeBroker.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldNotHaveAccessOnCheckIfOrganisationsHaveAccessToThisPatientWithInvalidInputsAsync()
        {
            // given
            string randomPseudoNhsNumber = GetRandomString();
            string inputPseudoNhsNumber = randomPseudoNhsNumber;
            List<PdsData> randomPdsDatas = CreateRandomPdsDatas();
            List<PdsData> storagePdsDatas = randomPdsDatas;
            List<string> inputOrganisationCodes = GetRandomStringsWithLengthOf(10);
            bool expectedResult = false;

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllPdsDatasAsync())
                    .ReturnsAsync(storagePdsDatas.AsQueryable());

            this.dateTimeBroker.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(DateTimeOffset.UtcNow);

            // when
            bool actualResult =
                await this.pdsDataService.OrganisationsHaveAccessToThisPatient(
                    pseudoNhsNumber: inputPseudoNhsNumber, organisationCodes: inputOrganisationCodes);

            // then
            actualResult.Should().Be(expectedResult);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllPdsDatasAsync(),
                    Times.Once);

            this.dateTimeBroker.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldNotHaveAccessToThisPatientIfRelationshipIsInactiveAsync()
        {
            // given
            string randomPseudoNhsNumber = GetRandomString();
            string inputPseudoNhsNumber = randomPseudoNhsNumber;
            List<PdsData> randomPdsDatas = CreateRandomPdsDatas();
            randomPdsDatas.ForEach(pdsData =>
            {
                pdsData.PseudoNhsNumber = inputPseudoNhsNumber;
                pdsData.RelationshipWithOrganisationEffectiveFromDate = GetRandomFutureDateTimeOffset();
            });
            List<PdsData> storagePdsDatas = randomPdsDatas;
            List<string> inputOrganisationCodes = randomPdsDatas.Select(pdsData => pdsData.OrgCode).ToList();
            bool expectedResult = false;

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllPdsDatasAsync())
                    .ReturnsAsync(storagePdsDatas.AsQueryable());

            this.dateTimeBroker.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(DateTimeOffset.UtcNow);

            // when
            bool actualResult =
                await this.pdsDataService.OrganisationsHaveAccessToThisPatient(
                    pseudoNhsNumber: inputPseudoNhsNumber, organisationCodes: inputOrganisationCodes);

            // then
            actualResult.Should().Be(expectedResult);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllPdsDatasAsync(),
                    Times.Once);

            this.dateTimeBroker.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.dateTimeBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}