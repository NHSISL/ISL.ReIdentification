// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ISL.ReIdentification.Core
{
    internal class ReIdentificationContextFactory : IDesignTimeDbContextFactory<ReIdentificationStorageBroker>
    {
        public ReIdentificationStorageBroker CreateDbContext(string[] args)
        {
            List<KeyValuePair<string, string>> config = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(
                    key: "ConnectionStrings:ReIdentificationConnection",
                    value: "Server=(localdb)\\MSSQLLocalDB;Database=ReIdentification;" +
                        "Trusted_Connection=True;MultipleActiveResultSets=true"),
            };

            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(initialData: config);

            IConfiguration configuration = configurationBuilder.Build();
            return new ReIdentificationStorageBroker(configuration);
        }
    }
}
