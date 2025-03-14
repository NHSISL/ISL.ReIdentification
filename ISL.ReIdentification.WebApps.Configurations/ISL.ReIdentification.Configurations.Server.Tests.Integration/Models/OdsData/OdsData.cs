﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.OdsData
{
    public class OdsData
    {
        public Guid Id { get; set; }
        public string OdsHierarchy { get; set; }
        public string OrganisationCode { get; set; }
        public string OrganisationName { get; set; }
        public DateTimeOffset? RelationshipWithParentStartDate { get; set; }
        public DateTimeOffset? RelationshipWithParentEndDate { get; set; }
        public bool HasChildren { get; set; }
    }
}