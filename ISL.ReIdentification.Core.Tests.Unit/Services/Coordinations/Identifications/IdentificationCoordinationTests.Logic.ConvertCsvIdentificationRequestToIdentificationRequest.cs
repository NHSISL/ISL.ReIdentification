// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Coordinations.Identifications;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Theory]
        [MemberData(nameof(InputCsvIdentificationRequest))]
        public async Task ShouldConvertCsvIdentificationRequestToIdentificationRequest(
            CsvIdentificationRequest csvIdentificationRequest,
            IdentificationRequest identificationRequest)
        {
            // given
            string csvDataString = Encoding.UTF8.GetString(csvIdentificationRequest.Data);

            this.csvHelperBrokerMock.Setup(broker =>
                broker.MapCsvToObjectAsync<CsvIdentificationItem>(csvDataString, true, null))
                    .ReturnsAsync(RandomCsvIdentificationItems);

            // when
            IdentificationRequest actualResult = await this.identificationCoordinationService
                .ConvertCsvIdentificationRequestToIdentificationRequest(csvIdentificationRequest);

            // then
            actualResult.Should().BeEquivalentTo(identificationRequest);

            this.csvHelperBrokerMock.Verify(service =>
                service.MapCsvToObjectAsync<CsvIdentificationItem>(csvDataString, true, null),
                    Times.Once);

            this.csvHelperBrokerMock.VerifyNoOtherCalls();
        }
    }
}
