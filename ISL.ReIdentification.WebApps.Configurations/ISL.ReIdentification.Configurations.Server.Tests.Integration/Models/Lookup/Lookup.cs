// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace ISL.ReIdentification.Configurations.Server.Tests.Integration.Models.Lookup
{
    public class Lookup : IKey, IAudit
    {
        public Guid Id { get; set; }
        public string GroupName { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int SortOrder { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
