// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Force.DeepCloner;
using ISL.ReIdentification.Core.Models.Foundations.ReIdentifications;

namespace ISL.ReIdentification.Core.Tests.Unit.Services.Coordinations.Identifications
{
    public partial class IdentificationCoordinationTests
    {
        [Fact]
        public void ShouldUpdateIdentifierColumnValues()
        {
            // given
            int numberOfObjects = GetRandomNumber();
            int identifierColumnIndex = 2;
            List<dynamic> originalRecords = CreateRandomDynamicObjects(numberOfObjects);
            List<dynamic> expectedRecords = originalRecords.DeepClone();

            List<IdentificationItem> identificationItems =
                CreateRandomIdentificationItems(numberOfObjects).ToList();

            for (int i = 0; i < expectedRecords.Count; i++)
            {
                expectedRecords[i].Identifier = identificationItems[i].Identifier;
            }

            // when
            List<dynamic> actualResult = this.identificationCoordinationService
                .UpdateIdentifierColumnValues(originalRecords, identificationItems, identifierColumnIndex);

            // then
            actualResult.Should().BeEquivalentTo(expectedRecords);
        }
    }
}
