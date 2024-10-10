// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Core.Models.Foundations.CsvIdentificationRequests;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial class ReIdentificationStorageBroker
    {
        private void AddCsvIdentificationRequestConfigurations(EntityTypeBuilder<CsvIdentificationRequest> builder)
        {
            builder.Property(csvIdentificationRequest => csvIdentificationRequest.RequesterEntraUserId)
                .IsRequired();

            builder.Property(csvIdentificationRequest => csvIdentificationRequest.RequesterEmail)
                .HasMaxLength(320)
                .IsRequired();

            builder.Property(csvIdentificationRequest => csvIdentificationRequest.RecipientEntraUserId)
                .IsRequired();

            builder.Property(csvIdentificationRequest => csvIdentificationRequest.RecipientEmail)
                .HasMaxLength(320)
                .IsRequired();

            builder.Property(csvIdentificationRequest => csvIdentificationRequest.Reason)
                .IsRequired();

            builder.Property(csvIdentificationRequest => csvIdentificationRequest.Purpose)
                .IsRequired();

            builder.Property(csvIdentificationRequest => csvIdentificationRequest.Organisation)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(csvIdentificationRequest => csvIdentificationRequest.Data)
                .IsRequired();

            builder.Property(csvIdentificationRequest => csvIdentificationRequest.Sha256Hash)
                .IsRequired();

            builder.Property(csvIdentificationRequest => csvIdentificationRequest.IdentifierColumn)
                .IsRequired();

            builder
                .Property(csvIdentificationRequest => csvIdentificationRequest.CreatedBy)
                .HasMaxLength(255)
                .IsRequired();

            builder
                .Property(csvIdentificationRequest => csvIdentificationRequest.CreatedDate)
                .IsRequired();

            builder
                .Property(csvIdentificationRequest => csvIdentificationRequest.UpdatedBy)
                .HasMaxLength(255)
                .IsRequired();

            builder
                .Property(csvIdentificationRequest => csvIdentificationRequest.UpdatedDate)
                .IsRequired();
        }
    }
}