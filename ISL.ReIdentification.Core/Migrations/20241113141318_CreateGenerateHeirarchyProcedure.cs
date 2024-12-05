// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISL.ReIdentification.Core.Migrations
{
    /// <inheritdoc />
    public partial class CreateGenerateHeirarchyProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        { }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                $@"IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GenerateHeirarchy') 
                BEGIN                
                    DROP PROCEDURE GenerateHeirarchy;
                END
                ");
        }
    }
}
