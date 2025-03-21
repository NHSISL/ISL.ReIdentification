// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Core.Models.Foundations.UserAccesses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial class ReIdentificationStorageBroker
    {
        private void AddUserAccessConfigurations(EntityTypeBuilder<UserAccess> builder)
        {
            builder.ToTable(userAccess => userAccess.IsTemporal());

            builder.Property(userAccess => userAccess.EntraUserId)
                .IsRequired();

            builder.Property(userAccess => userAccess.Email)
                .HasMaxLength(320)
                .IsRequired();

            builder.Property(userAccess => userAccess.OrgCode)
                .HasMaxLength(15)
                .IsRequired();

            builder.HasIndex(userAccess => new { userAccess.EntraUserId, userAccess.OrgCode })
                .IsUnique();

            builder
                .Property(userAccess => userAccess.CreatedBy)
                .HasMaxLength(255)
                .IsRequired();

            builder
                .Property(userAccess => userAccess.CreatedDate)
                .IsRequired();

            builder
                .Property(userAccess => userAccess.UpdatedBy)
                .HasMaxLength(255)
                .IsRequired();

            builder
                .Property(userAccess => userAccess.UpdatedDate)
                .IsRequired();
        }
    }
}