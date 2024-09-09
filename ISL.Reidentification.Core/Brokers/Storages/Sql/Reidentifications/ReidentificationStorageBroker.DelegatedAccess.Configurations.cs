﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.Reidentification.Core.Models.Foundations.DelegatedAccesses;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISL.Reidentification.Core.Brokers.Storages.Sql.Reidentifications
{
    public partial class ReidentificationStorageBroker
    {
        private void AddDelegatedAccessConfigurations(EntityTypeBuilder<DelegatedAccess> builder)
        {
            builder.Property(delegatedAccess => delegatedAccess.RequesterEmail)
                .IsRequired();

            builder.Property(delegatedAccess => delegatedAccess.RecipientEmail)
                .IsRequired();

            builder.Property(delegatedAccess => delegatedAccess.IsDelegatedAccess)
                .IsRequired();

            builder.Property(delegatedAccess => delegatedAccess.IsApproved)
                .IsRequired();

            builder.Property(delegatedAccess => delegatedAccess.Data)
                .IsRequired(false);

            builder.Property(delegatedAccess => delegatedAccess.IdentifierColumn)
                .IsRequired();

            builder.Property(delegatedAccess => delegatedAccess.CreatedBy)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(delegatedAccess => delegatedAccess.CreatedDate)
                .IsRequired();

            builder.Property(delegatedAccess => delegatedAccess.UpdatedBy)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(delegatedAccess => delegatedAccess.UpdatedDate)
                .IsRequired();
        }
    }
}