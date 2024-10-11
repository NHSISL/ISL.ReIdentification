
if(Object_ID('dbo.TempSample_Dictionary_dbo_OrganisationDescendent') is not null)
	drop table dbo.TempSample_Dictionary_dbo_OrganisationDescendent
GO
CREATE TABLE [dbo].[TempSample_Dictionary_dbo_OrganisationDescendent](
	Id int IDENTITY(1,1) NOT NULL,
	[ParentPath] [varchar](500) NULL,
	[ParentId] int NULL,
	RowNumber int NULL,
	OdsHierarchy hierarchyid NULL,
	[SK_OrganisationID_Root] [int]  NULL,
	[OrganisationCode_Root] [varchar](15)  NULL,
	[OrganisationPrimaryRole_Root] [varchar](5) NULL,
	[SK_OrganisationID_Parent] [int]  NULL,
	[OrganisationCode_Parent] [varchar](15)  NULL,
	[OrganisationPrimaryRole_Parent] [varchar](5) NULL,
	[SK_OrganisationID_Child] [int]  NULL,
	[OrganisationCode_Child] [varchar](15)  NULL,
	[OrganisationPrimaryRole_Child] [varchar](5) NULL,
	[RelationshipType] [varchar](5)  NULL,
	[RelationshipStartDate] [date]  NULL,
	[RelationshipEndDate] [date]  NULL,
	[Path] [varchar](500)  NULL,
	[Depth] [tinyint]  NULL,
	[PathStartDate] [date]  NULL,
	[PathEndDate] [date]  NULL,
	[DateAdded] [date]  NULL,
	[DateUpdated] [date]  NULL
) ON [PRIMARY]
GO

GO 
-- Insert the root node
Insert into TempSample_Dictionary_dbo_OrganisationDescendent (OrganisationCode_Child, OdsHierarchy) values ('ROOT',hierarchyid::GetRoot())