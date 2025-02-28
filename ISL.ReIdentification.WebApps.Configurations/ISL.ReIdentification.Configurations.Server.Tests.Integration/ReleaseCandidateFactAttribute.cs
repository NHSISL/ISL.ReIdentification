// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace ISL.ReIdentification.Configurations.Server.Tests.Integration
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer(
        typeName: "ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification.ReleaseCandidateTestCaseDiscoverer",
        assemblyName: "ISL.ReIdentification.Portals.Server.Tests.Integration.ReIdentification")]
    public class ReleaseCandidateFactAttribute : FactAttribute { }
}
