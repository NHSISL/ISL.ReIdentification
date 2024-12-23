// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace ISL.ReIdentification.Core.Models.Coordinations.Identifications
{
    public class ProjectStorageConfiguration
    {
        public string Container { get; set; }
        public string LandingFolder { get; set; }
        public string PickupFolder { get; set; }
        public string ErrorFolder { get; set; }
        public string ServiceUri { get; set; }
        public string StorageAccountName { get; set; }
        public string StorageAccountAccessKey { get; set; }
        public int TokenLifetimeMinutes { get; set; }
    }
}
