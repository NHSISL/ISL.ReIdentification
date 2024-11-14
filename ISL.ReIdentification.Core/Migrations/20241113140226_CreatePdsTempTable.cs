// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISL.ReIdentification.Core.Migrations
{
    /// <inheritdoc />
    public partial class CreatePdsTempTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourceName =
                $"ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications.Scripts.CreatePdsTempTable.sql";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                var sqlContent = reader.ReadToEnd();

                var createSpIfNotExists = $@"
                    IF OBJECT_ID('dbo.tempSample_PDS_PDS_PATIENT_CARE_PRACTICE', 'U') IS NULL
                    BEGIN
                        EXEC('{sqlContent.Replace("'", "''")}');
                    END";

                migrationBuilder.Sql(createSpIfNotExists);
            }
        }

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
