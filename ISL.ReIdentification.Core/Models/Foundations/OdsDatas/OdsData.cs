// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Microsoft.EntityFrameworkCore;

namespace ISL.ReIdentification.Core.Models.Foundations.OdsDatas
{
    public class OdsData : IKey, IAudit
    {
        public Guid Id { get; set; }
        public HierarchyId OdsHierarchy { get; set; }
        public string OrganisationCode { get; set; }
        public string OrganisationName { get; set; }
        public DateTimeOffset? RelationshipWithParentStartDate { get; set; }
        public DateTimeOffset? RelationshipWithParentEndDate { get; set; }
        public bool HasChildren { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
