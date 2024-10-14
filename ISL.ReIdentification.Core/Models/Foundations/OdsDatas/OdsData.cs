// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ISL.ReIdentification.Core.Models.Foundations.OdsDatas
{
    public class OdsData : IKey
    {
        public Guid Id { get; set; }
        public HierarchyId? OdsHierarchy { get; set; }
        public string OrganisationCode { get; set; }
        public string OrganisationName { get; set; }
        public DateTimeOffset? RelationshipStartDate { get; set; }
        public DateTimeOffset? RelationshipEndDate { get; set; }
    }
}
