// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Microsoft.EntityFrameworkCore;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Models.OdsDatas
{
    public class OdsData
    {
        public Guid Id { get; set; }
        public HierarchyId OdsHierarchy { get; set; }
        public string OrganisationCode { get; set; }
        public string OrganisationName { get; set; }
        public DateTimeOffset? RelationshipWithParentStartDate { get; set; }
        public DateTimeOffset? RelationshipWithParentEndDate { get; set; }
        public bool HasChildren { get; set; }
    }
}
