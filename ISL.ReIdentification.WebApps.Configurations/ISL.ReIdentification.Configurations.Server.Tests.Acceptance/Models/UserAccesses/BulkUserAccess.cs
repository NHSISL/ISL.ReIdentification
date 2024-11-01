﻿namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Models.UserAccesses
{
    public class BulkUserAccess
    {
        public Guid EntraUserId { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string DisplayName { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public string UserPrincipalName { get; set; }
        public List<string> OrgCodes { get; set; }
    }
}
