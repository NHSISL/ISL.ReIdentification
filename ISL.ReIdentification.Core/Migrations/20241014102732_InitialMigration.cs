using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISL.ReIdentification.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PseudoIdentifier = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EntraUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GivenName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Organisation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HasAccess = table.Column<bool>(type: "bit", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessAudits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CsvIdentificationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequesterEntraUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequesterFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequesterLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequesterDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequesterEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    RequesterJobTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecipientEntraUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipientFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecipientLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecipientDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecipientEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    RecipientJobTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Organisation = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Sha256Hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentifierColumn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CsvIdentificationRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImpersonationContexts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequesterEntraUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequesterFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequesterLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequesterDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequesterEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    RequesterJobTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecipientEntraUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipientFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecipientLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecipientDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecipientEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    RecipientJobTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Organisation = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PipelineName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ManagedIdentityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IdentifierColumn = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImpersonationContexts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lookups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lookups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PdsDatas",
                columns: table => new
                {
                    RowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PseudoNhsNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    PrimaryCareProvider = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    PrimaryCareProviderBusinessEffectiveFromDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PrimaryCareProviderBusinessEffectiveToDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CcgOfRegistration = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    CurrentCcgOfRegistration = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    IcbOfRegistration = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    CurrentIcbOfRegistration = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PdsDatas", x => x.RowId);
                });

            migrationBuilder.CreateTable(
                name: "UserAccesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntraUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GivenName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    UserPrincipalName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrgCode = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OdsDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganisationCode_Root = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    OrganisationPrimaryRole_Root = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    OrganisationCode_Parent = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    OrganisationPrimaryRole_Parent = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    RelationshipStartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RelationshipEndDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Depth = table.Column<int>(type: "int", nullable: false),
                    PdsDataRowId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OdsDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OdsDatas_PdsDatas_PdsDataRowId",
                        column: x => x.PdsDataRowId,
                        principalTable: "PdsDatas",
                        principalColumn: "RowId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessAudits_CreatedDate",
                table: "AccessAudits",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_AccessAudits_Email",
                table: "AccessAudits",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_AccessAudits_EntraUserId",
                table: "AccessAudits",
                column: "EntraUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessAudits_HasAccess",
                table: "AccessAudits",
                column: "HasAccess");

            migrationBuilder.CreateIndex(
                name: "IX_AccessAudits_PseudoIdentifier",
                table: "AccessAudits",
                column: "PseudoIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_ImpersonationContexts_PipelineName",
                table: "ImpersonationContexts",
                column: "PipelineName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lookups_Name",
                table: "Lookups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OdsDatas_PdsDataRowId",
                table: "OdsDatas",
                column: "PdsDataRowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessAudits");

            migrationBuilder.DropTable(
                name: "CsvIdentificationRequests");

            migrationBuilder.DropTable(
                name: "ImpersonationContexts");

            migrationBuilder.DropTable(
                name: "Lookups");

            migrationBuilder.DropTable(
                name: "OdsDatas");

            migrationBuilder.DropTable(
                name: "UserAccesses");

            migrationBuilder.DropTable(
                name: "PdsDatas");
        }
    }
}
