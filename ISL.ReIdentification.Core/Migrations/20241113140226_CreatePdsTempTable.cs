// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISL.ReIdentification.Core.Migrations
{
    /// <inheritdoc />
    public partial class CreatePdsTempTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        { }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var dropIfTableExists = $@"
                    IF OBJECT_ID('dbo.tempSample_PDS_PDS_PATIENT_CARE_PRACTICE', 'U') IS NOT NULL
                        DROP TABLE dbo.tempSample_PDS_PDS_PATIENT_CARE_PRACTICE;
                    GO";

            migrationBuilder.Sql(dropIfTableExists);
        }
    }
}
