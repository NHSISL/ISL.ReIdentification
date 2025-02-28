using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISL.ReIdentification.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Audits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AuditType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AuditDetail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogLevel = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audits", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Audits_AuditType",
                table: "Audits",
                column: "AuditType");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_CorrelationId",
                table: "Audits",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_CreatedDate",
                table: "Audits",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_LogLevel",
                table: "Audits",
                column: "LogLevel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Audits");
        }
    }
}
