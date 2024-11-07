using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISL.ReIdentification.Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCsvIdentificationRequestToUseIdentifierIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentifierColumn",
                table: "CsvIdentificationRequests");

            migrationBuilder.AddColumn<bool>(
                name: "HasHeaderRecord",
                table: "CsvIdentificationRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "IdentifierColumnIndex",
                table: "CsvIdentificationRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasHeaderRecord",
                table: "CsvIdentificationRequests");

            migrationBuilder.DropColumn(
                name: "IdentifierColumnIndex",
                table: "CsvIdentificationRequests");

            migrationBuilder.AddColumn<string>(
                name: "IdentifierColumn",
                table: "CsvIdentificationRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
