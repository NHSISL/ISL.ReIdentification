namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Models.Accesses
{
    public class AccessRequest
    {
        public IdentificationRequest IdentificationRequest { get; set; }
        public CsvIdentificationRequest CsvIdentificationRequest { get; set; }
        public ImpersonationContext ImpersonationContext { get; set; }
    }
}
