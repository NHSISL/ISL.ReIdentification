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

namespace ISL.ReIdentification.Portals.Server.Tests.Unit.Controllers.ImpersonationContexts
{
    public partial class ImpersonationContextsControllerTests
    {
        [Fact]
        public void GetShouldHaveWithNoRoleAttributeWithRoles()
        {
            // Given

            var controllerType = typeof(ImpersonationContextsController);
            var methodInfo = controllerType.GetMethod("GetImpersonationContextByIdAsync");
            Type attributeType = typeof(AuthorizeAttribute);
            string attributeProperty = "Roles";
            List<string> expectedAttributeValues = new List<string>();

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
        public void GetShouldNotHaveInvisibleApiAttribute()
        {
            // Given

            var controllerType = typeof(ImpersonationContextsController);
            var methodInfo = controllerType.GetMethod("GetImpersonationContextByIdAsync");
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
