// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;
using Moq;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Theory]
        [MemberData(nameof(InputIdentificationRequest))]
        public async Task ShouldConvertIdentificationRequestToCsvIdentificationRequest(
            IdentificationRequest identificationRequest,
            CsvIdentificationRequest csvIdentificationRequest)
        {
            // given
            string csvDataString = Encoding.UTF8.GetString(csvIdentificationRequest.Data);

            this.csvHelperBrokerMock.Setup(broker =>
                broker.MapObjectToCsvAsync(RandomCsvIdentificationItems(), true, null, false))
                    .ReturnsAsync(CsvDataString);

            // when
            CsvIdentificationRequest actualResult = await this.identificationCoordinationService
                .ConvertIdentificationRequestToCsvIdentificationRequest(identificationRequest);

            // then
            actualResult.Should().BeEquivalentTo(csvIdentificationRequest);

            this.csvHelperBrokerMock.Verify(service =>
                service.MapObjectToCsvAsync(RandomCsvIdentificationItems(), true, null, false),
                    Times.Once);

            this.csvHelperBrokerMock.VerifyNoOtherCalls();
        }
    }
}
