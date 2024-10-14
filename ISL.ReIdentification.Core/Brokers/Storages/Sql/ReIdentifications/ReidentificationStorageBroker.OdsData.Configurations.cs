// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ISL.ReIdentification.Core.Models.Foundations.OdsDatas;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications
{
    public partial class ReIdentificationStorageBroker
    {
        private void AddOdsDataConfigurations(EntityTypeBuilder<OdsData> builder)
        {
            builder.Property(OdsData => OdsData.OrganisationName)
                .HasMaxLength(30);
        }
    }
}