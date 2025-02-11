// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Core.Models.Foundations.Audits;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial class ReIdentificationStorageBroker
    {
        private void AddAuditConfigurations(EntityTypeBuilder<Audit> builder)
        {

            builder.Property(audit => audit.Id)
                .IsRequired();

            builder.Property(audit => audit.CorrelationId)
                .IsRequired(false);

            builder.Property(audit => audit.AuditType)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(audit => audit.AuditDetail)
                .IsRequired();

            builder.Property(audit => audit.LogLevel)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(audit => audit.CreatedBy)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(audit => audit.CreatedDate)
                .IsRequired();

            builder.Property(audit => audit.UpdatedBy)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(audit => audit.UpdatedDate)
                .IsRequired();

            builder.HasIndex(audit => audit.CorrelationId);
            builder.HasIndex(audit => audit.AuditType);
            builder.HasIndex(audit => audit.LogLevel);
            builder.HasIndex(audit => audit.CreatedDate);
        }
    }
}
