// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISL.ReIdentification.Core.Migrations
{
    /// <inheritdoc />
    public partial class RevertTempTablesForPdsAndOds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                $@"IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'LoadPdsData') 
                BEGIN                
                    DROP PROCEDURE LoadPdsData;
                END
                
                GO

                IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GenerateHeirarchy') 
                BEGIN                
                    DROP PROCEDURE GenerateHeirarchy;
                END

                GO
                
                IF OBJECT_ID('dbo.Temp_Dictionary_dbo_OrganisationDescendent', 'U') IS NOT NULL
                BEGIN
                    DROP TABLE dbo.Temp_Dictionary_dbo_OrganisationDescendent;
                END

                GO

                IF OBJECT_ID('dbo.tempSample_PDS_PDS_PATIENT_CARE_PRACTICE', 'U') IS NOT NULL
                BEGIN
                    DROP TABLE dbo.tempSample_PDS_PDS_PATIENT_CARE_PRACTICE;
                END

                GO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                $@"IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'LoadPdsData') 
                BEGIN                
                    DROP PROCEDURE LoadPdsData;
                END
                
                GO

                IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GenerateHeirarchy') 
                BEGIN                
                    DROP PROCEDURE GenerateHeirarchy;
                END

                GO
                
                IF OBJECT_ID('dbo.Temp_Dictionary_dbo_OrganisationDescendent', 'U') IS NOT NULL
                BEGIN
                    DROP TABLE dbo.Temp_Dictionary_dbo_OrganisationDescendent;
                END

                GO

                IF OBJECT_ID('dbo.tempSample_PDS_PDS_PATIENT_CARE_PRACTICE', 'U') IS NOT NULL
                BEGIN
                    DROP TABLE dbo.tempSample_PDS_PDS_PATIENT_CARE_PRACTICE;
                END

                GO");
        }
    }
}
