using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISL.ReIdentification.Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedLookupsToHaveGroupName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lookups_Name",
                table: "Lookups");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Lookups",
                type: "nvarchar(220)",
                maxLength: 220,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);

            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "Lookups",
                type: "nvarchar(220)",
                maxLength: 220,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Lookups_GroupName",
                table: "Lookups",
                column: "GroupName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lookups_GroupName_Name",
                table: "Lookups",
                columns: new[] { "GroupName", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lookups_GroupName",
                table: "Lookups");

            migrationBuilder.DropIndex(
                name: "IX_Lookups_GroupName_Name",
                table: "Lookups");

            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "Lookups");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Lookups",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(220)",
                oldMaxLength: 220);

            migrationBuilder.CreateIndex(
                name: "IX_Lookups_Name",
                table: "Lookups",
                column: "Name",
                unique: true);
        }
    }
}
