// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

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

            // when
            bool actualResult =
                await this.pdsDataService.OrganisationsHaveAccessToThisPatient(
                    pseudoNhsNumber: inputPseudoNhsNumber, organisationCodes: inputOrganisationCodes);

            // then
            actualResult.Should().Be(expectedResult);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllPdsDatasAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
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

            // when
            bool actualResult =
                await this.pdsDataService.OrganisationsHaveAccessToThisPatient(
                    pseudoNhsNumber: inputPseudoNhsNumber, organisationCodes: inputOrganisationCodes);

            // then
            actualResult.Should().Be(expectedResult);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllPdsDatasAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(Organisations))]
        public async Task ShouldNotHaveAccessOnCheckIfOrganisationsHaveAccessToThisPatientWithInvalidOrganisationsAsync(
            List<string> organisations)
        {
            // given
            string randomPseudoNhsNumber = GetRandomString();
            string inputPseudoNhsNumber = randomPseudoNhsNumber;
            List<PdsData> randomPdsDatas = CreateRandomPdsDatas();
            randomPdsDatas.ForEach(pdsData => pdsData.PseudoNhsNumber = inputPseudoNhsNumber);
            List<PdsData> storagePdsDatas = randomPdsDatas;
            List<string> inputOrganisationCodes = organisations;
            bool expectedResult = false;

            this.reIdentificationStorageBroker.Setup(broker =>
                broker.SelectAllPdsDatasAsync())
                    .ReturnsAsync(storagePdsDatas.AsQueryable());

            // when
            bool actualResult =
                await this.pdsDataService.OrganisationsHaveAccessToThisPatient(
                    pseudoNhsNumber: inputPseudoNhsNumber, organisationCodes: inputOrganisationCodes);

            // then
            actualResult.Should().Be(expectedResult);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllPdsDatasAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
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

            // when
            bool actualResult =
                await this.pdsDataService.OrganisationsHaveAccessToThisPatient(
                    pseudoNhsNumber: inputPseudoNhsNumber, organisationCodes: inputOrganisationCodes);

            // then
            actualResult.Should().Be(expectedResult);

            this.reIdentificationStorageBroker.Verify(broker =>
                broker.SelectAllPdsDatasAsync(),
                    Times.Once);

            this.reIdentificationStorageBroker.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}