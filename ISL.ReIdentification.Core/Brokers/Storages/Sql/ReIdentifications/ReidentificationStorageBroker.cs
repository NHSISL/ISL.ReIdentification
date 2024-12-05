// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions;
using ISL.ReIdentification.Core.Models.Foundations.AccessAudits;
using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using ISL.ReIdentification.Core.Models.Foundations.Lookups;
using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using STX.EFCore.Client.Clients;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial class ReIdentificationStorageBroker : EFxceptionsContext, IReIdentificationStorageBroker
    {
        private readonly IConfiguration configuration;
        private readonly IEFCoreClient efCoreClient;

        public ReIdentificationStorageBroker(IConfiguration configuration)
        {
            this.configuration = configuration;
            Database.Migrate();
            efCoreClient = new EFCoreClient(this);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            string connectionString = configuration
                .GetConnectionString(name: "ReIdentificationConnection") ?? string.Empty;

            optionsBuilder.UseSqlServer(connectionString, config => config.UseHierarchyId());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            AddImpersonationContextConfigurations(modelBuilder.Entity<ImpersonationContext>());
            AddUserAccessConfigurations(modelBuilder.Entity<UserAccess>());
            AddAccessAuditConfigurations(modelBuilder.Entity<AccessAudit>());
            AddLookupConfigurations(modelBuilder.Entity<Lookup>());
            AddCsvIdentificationRequestConfigurations(modelBuilder.Entity<CsvIdentificationRequest>());
            AddOdsDataConfigurations(modelBuilder.Entity<OdsData>());
            AddPdsDataConfigurations(modelBuilder.Entity<PdsData>());
        }

        private async ValueTask<T> InsertAsync<T>(T @object) where T : class =>
            await efCoreClient.InsertAsync(@object);

        private async ValueTask<IQueryable<T>> SelectAllAsync<T>() where T : class =>
            await efCoreClient.SelectAllAsync<T>();

        private async ValueTask<T> SelectAsync<T>(params object[] @objectIds) where T : class =>
            await efCoreClient.SelectAsync<T>(@objectIds);

        private async ValueTask<T> UpdateAsync<T>(T @object) where T : class =>
            await efCoreClient.UpdateAsync(@object);

        private async ValueTask<T> DeleteAsync<T>(T @object) where T : class =>
            await efCoreClient.DeleteAsync(@object);

        private async ValueTask BulkInsertAsync<T>(IEnumerable<T> objects) where T : class =>
            await efCoreClient.BulkInsertAsync<T>(objects);

        private async ValueTask BulkUpdateAsync<T>(IEnumerable<T> objects) where T : class =>
            await efCoreClient.BulkUpdateAsync<T>(objects);

        private async ValueTask BulkDeleteAsync<T>(IEnumerable<T> objects) where T : class =>
            await efCoreClient.BulkDeleteAsync<T>(objects);
    }
}