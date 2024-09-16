using System;

namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Models.Lookups
{
    public class Lookup
    {
        public Guid Id { get; set; }

        // TODO:  Add your properties here

        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
