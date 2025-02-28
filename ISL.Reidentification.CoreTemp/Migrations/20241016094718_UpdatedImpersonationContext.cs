// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISL.ReIdentification.Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedImpersonationContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PdsDatas",
                table: "PdsDatas");

            migrationBuilder.DropColumn(
                name: "RowId",
                table: "PdsDatas");

            migrationBuilder.DropColumn(
                name: "CcgOfRegistration",
                table: "PdsDatas");

            migrationBuilder.DropColumn(
                name: "CurrentCcgOfRegistration",
                table: "PdsDatas");

            migrationBuilder.DropColumn(
                name: "CurrentIcbOfRegistration",
                table: "PdsDatas");

            migrationBuilder.DropColumn(
                name: "IcbOfRegistration",
                table: "PdsDatas");

            migrationBuilder.DropColumn(
                name: "PrimaryCareProvider",
                table: "PdsDatas");

            migrationBuilder.RenameColumn(
                name: "PrimaryCareProviderBusinessEffectiveToDate",
                table: "PdsDatas",
                newName: "RelationshipWithOrganisationEffectiveToDate");

            migrationBuilder.RenameColumn(
                name: "PrimaryCareProviderBusinessEffectiveFromDate",
                table: "PdsDatas",
                newName: "RelationshipWithOrganisationEffectiveFromDate");

            migrationBuilder.RenameColumn(
                name: "RelationshipStartDate",
                table: "OdsDatas",
                newName: "RelationshipWithParentStartDate");

            migrationBuilder.RenameColumn(
                name: "RelationshipEndDate",
                table: "OdsDatas",
                newName: "RelationshipWithParentEndDate");

            migrationBuilder.RenameColumn(
                name: "RecipientLastName",
                table: "ImpersonationContexts",
                newName: "ResponsiblePersonLastName");

            migrationBuilder.RenameColumn(
                name: "RecipientJobTitle",
                table: "ImpersonationContexts",
                newName: "ResponsiblePersonJobTitle");

            migrationBuilder.RenameColumn(
                name: "RecipientFirstName",
                table: "ImpersonationContexts",
                newName: "ResponsiblePersonFirstName");

            migrationBuilder.RenameColumn(
                name: "RecipientEntraUserId",
                table: "ImpersonationContexts",
                newName: "ResponsiblePersonEntraUserId");

            migrationBuilder.RenameColumn(
                name: "RecipientEmail",
                table: "ImpersonationContexts",
                newName: "ResponsiblePersonEmail");

            migrationBuilder.RenameColumn(
                name: "RecipientDisplayName",
                table: "ImpersonationContexts",
                newName: "ResponsiblePersonDisplayName");

            migrationBuilder.RenameColumn(
                name: "PipelineName",
                table: "ImpersonationContexts",
                newName: "ProjectName");

            migrationBuilder.RenameColumn(
                name: "ManagedIdentityName",
                table: "ImpersonationContexts",
                newName: "ClientSecret");

            migrationBuilder.RenameIndex(
                name: "IX_ImpersonationContexts_PipelineName",
                table: "ImpersonationContexts",
                newName: "IX_ImpersonationContexts_ProjectName");

            migrationBuilder.AlterColumn<string>(
                name: "PseudoNhsNumber",
                table: "PdsDatas",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "PdsDatas",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "OrgCode",
                table: "PdsDatas",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrganisationName",
                table: "PdsDatas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "OrganisationCode",
                table: "OdsDatas",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "ImpersonationContexts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PdsDatas",
                table: "PdsDatas",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PdsDatas",
                table: "PdsDatas");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PdsDatas");

            migrationBuilder.DropColumn(
                name: "OrgCode",
                table: "PdsDatas");

            migrationBuilder.DropColumn(
                name: "OrganisationName",
                table: "PdsDatas");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "ImpersonationContexts");

            migrationBuilder.RenameColumn(
                name: "RelationshipWithOrganisationEffectiveToDate",
                table: "PdsDatas",
                newName: "PrimaryCareProviderBusinessEffectiveToDate");

            migrationBuilder.RenameColumn(
                name: "RelationshipWithOrganisationEffectiveFromDate",
                table: "PdsDatas",
                newName: "PrimaryCareProviderBusinessEffectiveFromDate");

            migrationBuilder.RenameColumn(
                name: "RelationshipWithParentStartDate",
                table: "OdsDatas",
                newName: "RelationshipStartDate");

            migrationBuilder.RenameColumn(
                name: "RelationshipWithParentEndDate",
                table: "OdsDatas",
                newName: "RelationshipEndDate");

            migrationBuilder.RenameColumn(
                name: "ResponsiblePersonLastName",
                table: "ImpersonationContexts",
                newName: "RecipientLastName");

            migrationBuilder.RenameColumn(
                name: "ResponsiblePersonJobTitle",
                table: "ImpersonationContexts",
                newName: "RecipientJobTitle");

            migrationBuilder.RenameColumn(
                name: "ResponsiblePersonFirstName",
                table: "ImpersonationContexts",
                newName: "RecipientFirstName");

            migrationBuilder.RenameColumn(
                name: "ResponsiblePersonEntraUserId",
                table: "ImpersonationContexts",
                newName: "RecipientEntraUserId");

            migrationBuilder.RenameColumn(
                name: "ResponsiblePersonEmail",
                table: "ImpersonationContexts",
                newName: "RecipientEmail");

            migrationBuilder.RenameColumn(
                name: "ResponsiblePersonDisplayName",
                table: "ImpersonationContexts",
                newName: "RecipientDisplayName");

            migrationBuilder.RenameColumn(
                name: "ProjectName",
                table: "ImpersonationContexts",
                newName: "PipelineName");

            migrationBuilder.RenameColumn(
                name: "ClientSecret",
                table: "ImpersonationContexts",
                newName: "ManagedIdentityName");

            migrationBuilder.RenameIndex(
                name: "IX_ImpersonationContexts_ProjectName",
                table: "ImpersonationContexts",
                newName: "IX_ImpersonationContexts_PipelineName");

            migrationBuilder.AlterColumn<string>(
                name: "PseudoNhsNumber",
                table: "PdsDatas",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<long>(
                name: "RowId",
                table: "PdsDatas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "CcgOfRegistration",
                table: "PdsDatas",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCcgOfRegistration",
                table: "PdsDatas",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentIcbOfRegistration",
                table: "PdsDatas",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IcbOfRegistration",
                table: "PdsDatas",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryCareProvider",
                table: "PdsDatas",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrganisationCode",
                table: "OdsDatas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PdsDatas",
                table: "PdsDatas",
                column: "RowId");
        }
    }
}
