using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISL.ReIdentification.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddingVersionNumberIntoUniqueConstraintOnUserAgreeements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAgreements_EntraUserId_AgreementType",
                table: "UserAgreements");

            migrationBuilder.AlterColumn<string>(
                name: "AgreementVersion",
                table: "UserAgreements",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAgreements_EntraUserId_AgreementType_AgreementVersion",
                table: "UserAgreements",
                columns: new[] { "EntraUserId", "AgreementType", "AgreementVersion" },
                unique: true,
                filter: "[AgreementVersion] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAgreements_EntraUserId_AgreementType_AgreementVersion",
                table: "UserAgreements");

            migrationBuilder.AlterColumn<string>(
                name: "AgreementVersion",
                table: "UserAgreements",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAgreements_EntraUserId_AgreementType",
                table: "UserAgreements",
                columns: new[] { "EntraUserId", "AgreementType" },
                unique: true);
        }
    }
}
