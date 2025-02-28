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
    public partial class AddTempOdsInboundTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourceName =
                $"ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications.Scripts.CreateTempOdsInboundTable.sql";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                var sqlContent = reader.ReadToEnd();

                var createSpIfNotExists = $@"
                    IF OBJECT_ID('[dbo].[TempOdsInbound]', 'U') IS NOT NULL
                    BEGIN       
                        DROP TABLE [dbo].[TempOdsInbound];
                    END

                    GO

                    IF OBJECT_ID('[dbo].[TempOdsInbound]', 'U') IS NULL
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
                IF OBJECT_ID('[dbo].[TempOdsInbound]', 'U') IS NOT NULL
                    DROP TABLE [dbo].[TempOdsInbound];
                GO";

            migrationBuilder.Sql(dropIfTableExists);
        }
    }
}
