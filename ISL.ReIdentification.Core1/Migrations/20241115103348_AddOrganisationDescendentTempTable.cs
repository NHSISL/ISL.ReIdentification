// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISL.ReIdentification.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganisationDescendentTempTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        { }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var dropIfTableExists = $@"
                    IF OBJECT_ID('dbo.Temp_Dictionary_dbo_OrganisationDescendent', 'U') IS NOT NULL
                        DROP TABLE dbo.Temp_Dictionary_dbo_OrganisationDescendent;
                    GO";

            migrationBuilder.Sql(dropIfTableExists);
        }
    }
}
