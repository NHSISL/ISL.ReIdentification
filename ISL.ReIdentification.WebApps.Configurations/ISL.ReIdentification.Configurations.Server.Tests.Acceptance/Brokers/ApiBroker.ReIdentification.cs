namespace ISL.ReIdentification.Configurations.Server.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string reIdentificationRelativeUrl = "api/reidentification";

        public async ValueTask<AccessRequest> PostImpersonationContextRequestAsync(AccessRequest accessRequest) =>
            await this.apiFactoryClient.PostContentAsync($"{reIdentificationRelativeUrl}/impersonation", accessRequest);
    }
}
