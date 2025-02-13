// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;

namespace ISL.ReIdentification.Core.Models.Foundations.PdsDatas
{
    public class PdsData : IKey, IAudit
    {
        public Guid Id { get; set; }
        public string PseudoNhsNumber { get; set; }
        public string OrgCode { get; set; }
        public string OrganisationName { get; set; }
        public DateTimeOffset? RelationshipWithOrganisationEffectiveFromDate { get; set; }
        public DateTimeOffset? RelationshipWithOrganisationEffectiveToDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}