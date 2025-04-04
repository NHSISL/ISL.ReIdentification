﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Attrify.Attributes;
using FluentAssertions;
using ISL.ReIdentification.Portals.Server.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.ReIdentification
{
    public partial class ReIdentificationControllerTests
    {
        [Fact]
        public void PostImpersonationContextGenerateTokensShouldHaveRoleAttributeWithRoles()
        {
            // Given
            var controllerType = typeof(ReIdentificationController);
            var methodInfo = controllerType.GetMethod("PostImpersonationContextGenerateTokensAsync");
            Type attributeType = typeof(AuthorizeAttribute);
            string attributeProperty = "Roles";

            List<string> expectedAttributeValues = new List<string>
            {
                "ISL.Reidentification.Portal.Administrators",
                "ISL.Reidentification.Portal.DataEngineers"
            };

            // When
            var methodAttribute = methodInfo?
                .GetCustomAttributes(attributeType, inherit: true)
                .FirstOrDefault();

            var controllerAttribute = controllerType
                .GetCustomAttributes(attributeType, inherit: true)
                .FirstOrDefault();

            var attribute = methodAttribute ?? controllerAttribute;

            // Then
            attribute.Should().NotBeNull();

            var actualAttributeValue = attributeType
                .GetProperty(attributeProperty)?
                .GetValue(attribute) as string ?? string.Empty;

            var actualAttributeValues = actualAttributeValue?
                .Split(',')
                .Select(role => role.Trim())
                .Where(role => !string.IsNullOrEmpty(role))
                .ToList();

            actualAttributeValues.Should().BeEquivalentTo(expectedAttributeValues);
        }

        [Fact]
        public void PostImpersonationContextGenerateTokensShouldNotHaveInvisibleApiAttribute()
        {
            // Given
            var controllerType = typeof(ReIdentificationController);
            var methodInfo = controllerType.GetMethod("PostImpersonationContextGenerateTokensAsync");
            Type attributeType = typeof(InvisibleApiAttribute);

            // When
            var methodAttribute = methodInfo?
                .GetCustomAttributes(attributeType, inherit: true)
                .FirstOrDefault();

            var controllerAttribute = controllerType
                .GetCustomAttributes(attributeType, inherit: true)
                .FirstOrDefault();

            var attribute = methodAttribute ?? controllerAttribute;

            // Then
            attribute.Should().BeNull();
        }
    }
}
