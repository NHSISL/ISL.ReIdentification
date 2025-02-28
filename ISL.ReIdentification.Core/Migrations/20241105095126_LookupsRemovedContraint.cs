using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISL.ReIdentification.Core.Migrations
{
    /// <inheritdoc />
    public partial class LookupsRemovedContraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lookups_GroupName",
                table: "Lookups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Lookups_GroupName",
                table: "Lookups",
                column: "GroupName",
                unique: true);
        }
    }
}
