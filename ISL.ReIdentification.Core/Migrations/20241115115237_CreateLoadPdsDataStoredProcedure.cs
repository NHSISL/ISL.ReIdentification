﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISL.ReIdentification.Core.Migrations
{
    /// <inheritdoc />
    public partial class CreateLoadPdsDataStoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourceName =
                $"ISL.ReIdentification.Core.Brokers.Storages.Sql.ReIdentifications.Scripts.CreateLoadPdsDataStoredProcedure.sql";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                var sqlContent = reader.ReadToEnd();

                var createSpIfNotExists = $@"
                    
                    IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'LoadPdsData') 
                    BEGIN                
                        DROP PROCEDURE LoadPdsData;
                    END

                    GO  

                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'LoadPdsData')
                    BEGIN
                        EXEC('{sqlContent.Replace("'", "''")}');
                    END";

                migrationBuilder.Sql(createSpIfNotExists);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                $@"IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'LoadPdsData') 
                BEGIN                
                    DROP PROCEDURE LoadPdsData;
                END
                ");
        }
    }
}
