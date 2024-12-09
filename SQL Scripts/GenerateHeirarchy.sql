
if(Object_ID('dbo.TempOdsInbound') is not null)
	drop table dbo.TempOdsInbound
GO
CREATE TABLE [dbo].[TempOdsInbound](
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
	[DateUpdated] [date]  NULL,
	[ODSOrgName] [varchar](25) NULL,
) ON [PRIMARY]
GO

Insert into TempOdsInbound (OrganisationCode_Child, OdsHierarchy) values ('ROOT',hierarchyid::GetRoot())

INSERT [dbo].[TempOdsInbound] ([SK_OrganisationID_Root], [OrganisationCode_Root], [OrganisationPrimaryRole_Root], [SK_OrganisationID_Parent], [OrganisationCode_Parent], [OrganisationPrimaryRole_Parent], [SK_OrganisationID_Child], [OrganisationCode_Child], [OrganisationPrimaryRole_Child], [RelationshipType], [RelationshipStartDate], [RelationshipEndDate], [Path], [Depth], [PathStartDate], [PathEndDate], [DateAdded], [DateUpdated], [ODSOrgName]) VALUES (440024, N'NHS', N'RO261', 440024, N'NHS', N'RO261', 440024, N'NHS', N'RO261', N'SELF', CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), N'[NHS]', 0, CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), CAST(N'2024-07-13' AS Date), CAST(N'2024-07-13' AS Date),'NHS')
INSERT [dbo].[TempOdsInbound] ([SK_OrganisationID_Root], [OrganisationCode_Root], [OrganisationPrimaryRole_Root], [SK_OrganisationID_Parent], [OrganisationCode_Parent], [OrganisationPrimaryRole_Parent], [SK_OrganisationID_Child], [OrganisationCode_Child], [OrganisationPrimaryRole_Child], [RelationshipType], [RelationshipStartDate], [RelationshipEndDate], [Path], [Depth], [PathStartDate], [PathEndDate], [DateAdded], [DateUpdated], [ODSOrgName]) VALUES (440024, N'TESTICB1', N'RO261', 440024, N'TESTICB1', N'RO261', 440024, N'TESTICB1', N'RO261', N'RE4', CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), N'[NHS]-<-{RE4}-<-[TESTICB1]', 0, CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), CAST(N'2024-07-13' AS Date), CAST(N'2024-07-13' AS Date), 'Integrated Care Board 1')
INSERT [dbo].[TempOdsInbound] ([SK_OrganisationID_Root], [OrganisationCode_Root], [OrganisationPrimaryRole_Root], [SK_OrganisationID_Parent], [OrganisationCode_Parent], [OrganisationPrimaryRole_Parent], [SK_OrganisationID_Child], [OrganisationCode_Child], [OrganisationPrimaryRole_Child], [RelationshipType], [RelationshipStartDate], [RelationshipEndDate], [Path], [Depth], [PathStartDate], [PathEndDate], [DateAdded], [DateUpdated], [ODSOrgName]) VALUES (440024, N'TESTICB2', N'RO261', 440024, N'TESTICB2', N'RO261', 440024, N'TESTICB2', N'RO261', N'RE4', CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), N'[NHS]-<-{RE4}-<-[TESTICB2]', 0, CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), CAST(N'2024-07-13' AS Date), CAST(N'2024-07-13' AS Date), 'Integrated Care Board 2')
INSERT [dbo].[TempOdsInbound] ([SK_OrganisationID_Root], [OrganisationCode_Root], [OrganisationPrimaryRole_Root], [SK_OrganisationID_Parent], [OrganisationCode_Parent], [OrganisationPrimaryRole_Parent], [SK_OrganisationID_Child], [OrganisationCode_Child], [OrganisationPrimaryRole_Child], [RelationshipType], [RelationshipStartDate], [RelationshipEndDate], [Path], [Depth], [PathStartDate], [PathEndDate], [DateAdded], [DateUpdated], [ODSOrgName]) VALUES (440024, N'PCN11', N'RO261', 440024, N'PCN11', N'RO261', 440024, N'PCN11', N'RO261', N'RE4', CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), N'[NHS]-<-{RE4}-<-[TESTICB1]-<-{RE4}-<-[PCN11]', 0, CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), CAST(N'2024-07-13' AS Date), CAST(N'2024-07-13' AS Date), 'Primary Care Board 11')
INSERT [dbo].[TempOdsInbound] ([SK_OrganisationID_Root], [OrganisationCode_Root], [OrganisationPrimaryRole_Root], [SK_OrganisationID_Parent], [OrganisationCode_Parent], [OrganisationPrimaryRole_Parent], [SK_OrganisationID_Child], [OrganisationCode_Child], [OrganisationPrimaryRole_Child], [RelationshipType], [RelationshipStartDate], [RelationshipEndDate], [Path], [Depth], [PathStartDate], [PathEndDate], [DateAdded], [DateUpdated], [ODSOrgName]) VALUES (440024, N'PCN12', N'RO261', 440024, N'PCN12', N'RO261', 440024, N'PCN12', N'RO261', N'RE4', CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), N'[NHS]-<-{RE4}-<-[TESTICB1]-<-{RE4}-<-[PCN12]', 0, CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), CAST(N'2024-07-13' AS Date), CAST(N'2024-07-13' AS Date), 'Primary Care Board 12')
INSERT [dbo].[TempOdsInbound] ([SK_OrganisationID_Root], [OrganisationCode_Root], [OrganisationPrimaryRole_Root], [SK_OrganisationID_Parent], [OrganisationCode_Parent], [OrganisationPrimaryRole_Parent], [SK_OrganisationID_Child], [OrganisationCode_Child], [OrganisationPrimaryRole_Child], [RelationshipType], [RelationshipStartDate], [RelationshipEndDate], [Path], [Depth], [PathStartDate], [PathEndDate], [DateAdded], [DateUpdated], [ODSOrgName]) VALUES (440024, N'PCN21', N'RO261', 440024, N'PCN21', N'RO261', 440024, N'PCN21', N'RO261', N'RE4', CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), N'[NHS]-<-{RE4}-<-[TESTICB2]-<-{RE4}-<-[PCN21]', 0, CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), CAST(N'2024-07-13' AS Date), CAST(N'2024-07-13' AS Date), 'Primary Care Board 21')
INSERT [dbo].[TempOdsInbound] ([SK_OrganisationID_Root], [OrganisationCode_Root], [OrganisationPrimaryRole_Root], [SK_OrganisationID_Parent], [OrganisationCode_Parent], [OrganisationPrimaryRole_Parent], [SK_OrganisationID_Child], [OrganisationCode_Child], [OrganisationPrimaryRole_Child], [RelationshipType], [RelationshipStartDate], [RelationshipEndDate], [Path], [Depth], [PathStartDate], [PathEndDate], [DateAdded], [DateUpdated], [ODSOrgName]) VALUES (440024, N'PCN22', N'RO261', 440024, N'PCN22', N'RO261', 440024, N'PCN22', N'RO261', N'RE4', CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), N'[NHS]-<-{RE4}-<-[TESTICB2]-<-{RE4}-<-[PCN22]', 0, CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), CAST(N'2024-07-13' AS Date), CAST(N'2024-07-13' AS Date), 'Primary Care Board 22')
INSERT [dbo].[TempOdsInbound] ([SK_OrganisationID_Root], [OrganisationCode_Root], [OrganisationPrimaryRole_Root], [SK_OrganisationID_Parent], [OrganisationCode_Parent], [OrganisationPrimaryRole_Parent], [SK_OrganisationID_Child], [OrganisationCode_Child], [OrganisationPrimaryRole_Child], [RelationshipType], [RelationshipStartDate], [RelationshipEndDate], [Path], [Depth], [PathStartDate], [PathEndDate], [DateAdded], [DateUpdated], [ODSOrgName]) VALUES (440024, N'GP111', N'RO261', 440024, N'GP111', N'RO261', 440024, N'GP111', N'RO261', N'RE4', CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), N'[NHS]-<-{RE4}-<-[TESTICB1]-<-{RE4}-<-[PCN11]-<-{RE4}-<-[GP111]', 0, CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), CAST(N'2024-07-13' AS Date), CAST(N'2024-07-13' AS Date), 'General Practice 111')
INSERT [dbo].[TempOdsInbound] ([SK_OrganisationID_Root], [OrganisationCode_Root], [OrganisationPrimaryRole_Root], [SK_OrganisationID_Parent], [OrganisationCode_Parent], [OrganisationPrimaryRole_Parent], [SK_OrganisationID_Child], [OrganisationCode_Child], [OrganisationPrimaryRole_Child], [RelationshipType], [RelationshipStartDate], [RelationshipEndDate], [Path], [Depth], [PathStartDate], [PathEndDate], [DateAdded], [DateUpdated], [ODSOrgName]) VALUES (440024, N'GP112', N'RO261', 440024, N'GP112', N'RO261', 440024, N'GP112', N'RO261', N'RE4', CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), N'[NHS]-<-{RE4}-<-[TESTICB1]-<-{RE4}-<-[PCN11]-<-{RE4}-<-[GP112]', 0, CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), CAST(N'2024-07-13' AS Date), CAST(N'2024-07-13' AS Date), 'General Practice 112')
INSERT [dbo].[TempOdsInbound] ([SK_OrganisationID_Root], [OrganisationCode_Root], [OrganisationPrimaryRole_Root], [SK_OrganisationID_Parent], [OrganisationCode_Parent], [OrganisationPrimaryRole_Parent], [SK_OrganisationID_Child], [OrganisationCode_Child], [OrganisationPrimaryRole_Child], [RelationshipType], [RelationshipStartDate], [RelationshipEndDate], [Path], [Depth], [PathStartDate], [PathEndDate], [DateAdded], [DateUpdated], [ODSOrgName]) VALUES (440024, N'GP121', N'RO261', 440024, N'GP121', N'RO261', 440024, N'GP121', N'RO261', N'RE4', CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), N'[NHS]-<-{RE4}-<-[TESTICB1]-<-{RE4}-<-[PCN12]-<-{RE4}-<-[GP121]', 0, CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), CAST(N'2024-07-13' AS Date), CAST(N'2024-07-13' AS Date), 'General Practice 121')
INSERT [dbo].[TempOdsInbound] ([SK_OrganisationID_Root], [OrganisationCode_Root], [OrganisationPrimaryRole_Root], [SK_OrganisationID_Parent], [OrganisationCode_Parent], [OrganisationPrimaryRole_Parent], [SK_OrganisationID_Child], [OrganisationCode_Child], [OrganisationPrimaryRole_Child], [RelationshipType], [RelationshipStartDate], [RelationshipEndDate], [Path], [Depth], [PathStartDate], [PathEndDate], [DateAdded], [DateUpdated], [ODSOrgName]) VALUES (440024, N'GP122', N'RO261', 440024, N'GP122', N'RO261', 440024, N'GP122', N'RO261', N'RE4', CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), N'[NHS]-<-{RE4}-<-[TESTICB1]-<-{RE4}-<-[PCN12]-<-{RE4}-<-[GP122]', 0, CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), CAST(N'2024-07-13' AS Date), CAST(N'2024-07-13' AS Date), 'General Practice 122')
INSERT [dbo].[TempOdsInbound] ([SK_OrganisationID_Root], [OrganisationCode_Root], [OrganisationPrimaryRole_Root], [SK_OrganisationID_Parent], [OrganisationCode_Parent], [OrganisationPrimaryRole_Parent], [SK_OrganisationID_Child], [OrganisationCode_Child], [OrganisationPrimaryRole_Child], [RelationshipType], [RelationshipStartDate], [RelationshipEndDate], [Path], [Depth], [PathStartDate], [PathEndDate], [DateAdded], [DateUpdated], [ODSOrgName]) VALUES (440024, N'GP211', N'RO261', 440024, N'GP211', N'RO261', 440024, N'GP211', N'RO261', N'RE4', CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), N'[NHS]-<-{RE4}-<-[TESTICB2]-<-{RE4}-<-[PCN21]-<-{RE4}-<-[GP211]', 0, CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), CAST(N'2024-07-13' AS Date), CAST(N'2024-07-13' AS Date), 'General Practice 211')
INSERT [dbo].[TempOdsInbound] ([SK_OrganisationID_Root], [OrganisationCode_Root], [OrganisationPrimaryRole_Root], [SK_OrganisationID_Parent], [OrganisationCode_Parent], [OrganisationPrimaryRole_Parent], [SK_OrganisationID_Child], [OrganisationCode_Child], [OrganisationPrimaryRole_Child], [RelationshipType], [RelationshipStartDate], [RelationshipEndDate], [Path], [Depth], [PathStartDate], [PathEndDate], [DateAdded], [DateUpdated], [ODSOrgName]) VALUES (440024, N'GP212', N'RO261', 440024, N'GP212', N'RO261', 440024, N'GP212', N'RO261', N'RE4', CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), N'[NHS]-<-{RE4}-<-[TESTICB2]-<-{RE4}-<-[PCN21]-<-{RE4}-<-[GP212]', 0, CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), CAST(N'2024-07-13' AS Date), CAST(N'2024-07-13' AS Date), 'General Practice 212')
INSERT [dbo].[TempOdsInbound] ([SK_OrganisationID_Root], [OrganisationCode_Root], [OrganisationPrimaryRole_Root], [SK_OrganisationID_Parent], [OrganisationCode_Parent], [OrganisationPrimaryRole_Parent], [SK_OrganisationID_Child], [OrganisationCode_Child], [OrganisationPrimaryRole_Child], [RelationshipType], [RelationshipStartDate], [RelationshipEndDate], [Path], [Depth], [PathStartDate], [PathEndDate], [DateAdded], [DateUpdated], [ODSOrgName]) VALUES (440024, N'GP221', N'RO261', 440024, N'GP221', N'RO261', 440024, N'GP221', N'RO261', N'RE4', CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), N'[NHS]-<-{RE4}-<-[TESTICB2]-<-{RE4}-<-[PCN22]-<-{RE4}-<-[GP221]', 0, CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), CAST(N'2024-07-13' AS Date), CAST(N'2024-07-13' AS Date), 'General Practice 221')
INSERT [dbo].[TempOdsInbound] ([SK_OrganisationID_Root], [OrganisationCode_Root], [OrganisationPrimaryRole_Root], [SK_OrganisationID_Parent], [OrganisationCode_Parent], [OrganisationPrimaryRole_Parent], [SK_OrganisationID_Child], [OrganisationCode_Child], [OrganisationPrimaryRole_Child], [RelationshipType], [RelationshipStartDate], [RelationshipEndDate], [Path], [Depth], [PathStartDate], [PathEndDate], [DateAdded], [DateUpdated], [ODSOrgName]) VALUES (440024, N'GP222', N'RO261', 440024, N'GP222', N'RO261', 440024, N'GP222', N'RO261', N'RE4', CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), N'[NHS]-<-{RE4}-<-[TESTICB2]-<-{RE4}-<-[PCN22]-<-{RE4}-<-[GP222]', 0, CAST(N'1900-01-01' AS Date), CAST(N'9999-12-31' AS Date), CAST(N'2024-07-13' AS Date), CAST(N'2024-07-13' AS Date), 'General Practice 222')
GO
-- Attach any 'SELF' relationships to the root node
UPDATE TempOdsInbound 
SET ParentId = 1, ParentPath = 'ROOT'
WHERE RelationshipType = 'SELF'

-- Generate easier to read parent path then use that to assign a parent id
UPDATE TempOdsInbound 
SET ParentPath = LEFT(Path, LEN(Path) - LEN(OrganisationCode_Child) - LEN(RelationshipType)-10) 
WHERE RelationshipType <> 'SELF'

-- Use the derived parent path to assign a parent id
UPDATE od
SET ParentId = (
	SELECT id 
	FROM TempOdsInbound od1 
	WHERE od1.path = od.parentPath
	)
FROM TempOdsInbound od
Where ParentId is NULL

-- Assign Row Numbers for each child
UPDATE od
SET RowNumber = rn
FROM TempOdsInbound od
JOIN (
	SELECT id, ROW_NUMBER() OVER (PARTITION BY ParentId ORDER BY ParentId) AS rn 
	FROM TempOdsInbound) od1
		ON od.id = od1.id


GO 
-- Insert the root node
-- Insert into TempOdsInbound (OrganisationCode_Child, OdsHierarchy) values ('ROOT',hierarchyid::GetRoot())

GO

WITH paths(path, Id)   
AS (  
	-- Handle the root node
	SELECT 
		hierarchyid::GetRoot(),
		id
	FROM TempOdsInbound
	WHERE ParentId is NULL

	UNION ALL
	
	-- Handle all other nodes
	SELECT 
		CAST(p.path.ToString() + CAST(od.RowNumber AS varchar(30)) + '/' AS hierarchyid), 
		od.Id 
	FROM TempOdsInbound od
	JOIN paths AS p ON od.parentId = p.id
)
UPDATE od
SET OdsHierarchy = p.path
FROM TempOdsInbound od
JOIN paths p ON od.id = p.id

GO
Insert Into OdsDatas (Id,OdsHierarchy, OrganisationCode, OrganisationName)
select NEWID(),OdsHierarchy, OrganisationCode_Child, ODSOrgName from TempOdsInbound


UPDATE OdsDatas set HasChildren=1 where id in(

select distinct o1.id
from OdsDatas o1
Left join OdsDatas o2 on o1.OdsHierarchy.GetDescendant(null,null) = o2.OdsHierarchy
where o2.id is not null)


