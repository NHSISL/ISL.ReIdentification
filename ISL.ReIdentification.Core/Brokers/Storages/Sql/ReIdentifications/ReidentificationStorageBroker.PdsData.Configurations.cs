// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Core.Models.Foundations.PdsDatas;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial class ReIdentificationStorageBroker
    {
        private void AddPdsDataConfigurations(EntityTypeBuilder<PdsData> builder)
        {
            builder.HasKey(pdsData => pdsData.Id);

            builder.Property(pdsData => pdsData.Id)
                .IsRequired();

            builder.Property(pdsData => pdsData.PseudoNhsNumber)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(pdsData => pdsData.OrgCode)
                .HasMaxLength(15)
                .IsRequired();
        }
    }
}
