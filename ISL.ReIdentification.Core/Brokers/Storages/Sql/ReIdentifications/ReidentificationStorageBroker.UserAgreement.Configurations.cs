// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Core.Models.Foundations.UserAgreements;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial class ReIdentificationStorageBroker
    {
        private void AddUserAgreementConfigurations(EntityTypeBuilder<UserAgreement> builder)
        {
            builder.Property(userAgreement => userAgreement.EntraUserId)
                .IsRequired();

            builder.Property(userAgreement => userAgreement.AgreementType)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(userAgreement => userAgreement.AgreementVersion)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(userAgreement => new
            {
                userAgreement.EntraUserId,
                userAgreement.AgreementType,
                userAgreement.AgreementVersion
            })
            .IsUnique();

            builder.Property(userAgreement => userAgreement.AgreementDate)
                .IsRequired();

            builder
                .Property(userAgreement => userAgreement.CreatedBy)
                .HasMaxLength(255)
                .IsRequired();

            builder
                .Property(userAgreement => userAgreement.CreatedDate)
                .IsRequired();

            builder
                .Property(userAgreement => userAgreement.UpdatedBy)
                .HasMaxLength(255)
                .IsRequired();

            builder
                .Property(userAgreement => userAgreement.UpdatedDate)
                .IsRequired();
        }
    }
}