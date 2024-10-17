using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.SqlServer.Types;

#nullable disable

namespace ISL.ReIdentification.Core.Migrations
{
    /// <inheritdoc />
    public partial class OdsHierarchyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OdsDatas_PdsDatas_PdsDataRowId",
                table: "OdsDatas");

            migrationBuilder.DropIndex(
                name: "IX_OdsDatas_PdsDataRowId",
                table: "OdsDatas");

            migrationBuilder.DropColumn(
                name: "Depth",
                table: "OdsDatas");

            migrationBuilder.DropColumn(
                name: "OrganisationCode_Parent",
                table: "OdsDatas");

            migrationBuilder.DropColumn(
                name: "OrganisationCode_Root",
                table: "OdsDatas");

            migrationBuilder.DropColumn(
                name: "OrganisationPrimaryRole_Parent",
                table: "OdsDatas");

            migrationBuilder.DropColumn(
                name: "OrganisationPrimaryRole_Root",
                table: "OdsDatas");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "OdsDatas");

            migrationBuilder.DropColumn(
                name: "PdsDataRowId",
                table: "OdsDatas");

            migrationBuilder.AddColumn<SqlHierarchyId>(
                name: "OdsHierarchy",
                table: "OdsDatas",
                type: "hierarchyid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganisationCode",
                table: "OdsDatas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrganisationName",
                table: "OdsDatas",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OdsHierarchy",
                table: "OdsDatas");

            migrationBuilder.DropColumn(
                name: "OrganisationCode",
                table: "OdsDatas");

            migrationBuilder.DropColumn(
                name: "OrganisationName",
                table: "OdsDatas");

            migrationBuilder.AddColumn<int>(
                name: "Depth",
                table: "OdsDatas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OrganisationCode_Parent",
                table: "OdsDatas",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrganisationCode_Root",
                table: "OdsDatas",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrganisationPrimaryRole_Parent",
                table: "OdsDatas",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrganisationPrimaryRole_Root",
                table: "OdsDatas",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "OdsDatas",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "PdsDataRowId",
                table: "OdsDatas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_OdsDatas_PdsDataRowId",
                table: "OdsDatas",
                column: "PdsDataRowId");

            migrationBuilder.AddForeignKey(
                name: "FK_OdsDatas_PdsDatas_PdsDataRowId",
                table: "OdsDatas",
                column: "PdsDataRowId",
                principalTable: "PdsDatas",
                principalColumn: "RowId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
