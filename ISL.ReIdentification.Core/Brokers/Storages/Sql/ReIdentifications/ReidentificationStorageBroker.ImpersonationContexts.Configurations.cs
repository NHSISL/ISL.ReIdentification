// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Core.Models.Foundations.ImpersonationContexts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial class ReIdentificationStorageBroker
    {
        private void AddImpersonationContextConfigurations(EntityTypeBuilder<ImpersonationContext> builder)
        {
            builder.Property(impersonationContext => impersonationContext.RequesterEntraUserId)
                .IsRequired();

            builder.Property(impersonationContext => impersonationContext.RequesterEmail)
                .HasMaxLength(320)
                .IsRequired();

            builder.Property(impersonationContext => impersonationContext.ResponsiblePersonEntraUserId)
                .IsRequired();

            builder.Property(impersonationContext => impersonationContext.ResponsiblePersonEmail)
                .HasMaxLength(320)
                .IsRequired();

            builder.Property(impersonationContext => impersonationContext.Reason)
                .IsRequired();

            builder.Property(impersonationContext => impersonationContext.Purpose)
                .IsRequired();

            builder.Property(impersonationContext => impersonationContext.Organisation)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(impersonationContext => impersonationContext.ProjectName)
                .HasMaxLength(255)
                .IsRequired();

            builder.HasIndex(impersonationContext => impersonationContext.ProjectName)
                .IsUnique();

            builder.Property(impersonationContext => impersonationContext.IsApproved)
                .IsRequired();

            builder.Property(impersonationContext => impersonationContext.IdentifierColumn)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(impersonationContext => impersonationContext.CreatedBy)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(impersonationContext => impersonationContext.CreatedDate)
                .IsRequired();

            builder.Property(impersonationContext => impersonationContext.UpdatedBy)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(impersonationContext => impersonationContext.UpdatedDate)
                .IsRequired();
        }
    }
}
