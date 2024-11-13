--select Object_ID('dbo.Temp_Dictionary_dbo_OrganisationDescendent')
--exec [dbo].[GenerateHeirarchy]


if(Object_ID('[dbo].[GenerateHeirarchy]') is not null)
	drop PROCEDURE [dbo].[GenerateHeirarchy]
GO
CREATE PROCEDURE [dbo].[GenerateHeirarchy] AS
BEGIN

if(Object_ID('dbo.Temp_Dictionary_dbo_OrganisationDescendent') is null)
BEGIN
	THROW 50000, 'Temp Table does not exist.', 1;
END

if((select count(*) from Temp_Dictionary_dbo_OrganisationDescendent) = 0)
BEGIN
	RAISERROR(N'Temp Table is empty.', 16, 1);
	return;
END

BEGIN TRANSACTION
TRUNCATE TABLE OdsDatas
update Temp_Dictionary_dbo_OrganisationDescendent set IsActive=1 where PathStartDate<=GETDATE() and PathEndDate>=GETDATE() and RelationshipStartDate<=GETDATE() and RelationshipEndDate>=GETDATE()

-- Attach any 'SELF' relationships to the root node
UPDATE Temp_Dictionary_dbo_OrganisationDescendent 
SET ParentId = 1, ParentPath = 'ROOT'
WHERE RelationshipType = 'SELF'

-- Generate easier to read parent path then use that to assign a parent id
UPDATE Temp_Dictionary_dbo_OrganisationDescendent 
SET ParentPath = LEFT(Path, LEN(Path) - LEN(OrganisationCode_Child) - LEN(RelationshipType)-10) 
WHERE RelationshipType <> 'SELF'

-- Use the derived parent path to assign a parent id
UPDATE od
SET ParentId = (
	SELECT id 
	FROM Temp_Dictionary_dbo_OrganisationDescendent od1 
	WHERE od1.path = od.parentPath
	)
FROM Temp_Dictionary_dbo_OrganisationDescendent od
Where ParentId is NULL

-- Assign Row Numbers for each child
UPDATE od
SET RowNumber = rn
FROM Temp_Dictionary_dbo_OrganisationDescendent od
JOIN (
	SELECT id, ROW_NUMBER() OVER (PARTITION BY ParentId ORDER BY ParentId) AS rn 
	FROM Temp_Dictionary_dbo_OrganisationDescendent) od1
		ON od.id = od1.id;


WITH paths(path, Id)   
AS (  
	-- Handle the root node
	SELECT 
		hierarchyid::GetRoot(),
		id
	FROM Temp_Dictionary_dbo_OrganisationDescendent
	WHERE ParentId is NULL

	UNION ALL
	
	-- Handle all other nodes
	SELECT 
		CAST(p.path.ToString() + CAST(od.RowNumber AS varchar(30)) + '/' AS hierarchyid), 
		od.Id 
	FROM Temp_Dictionary_dbo_OrganisationDescendent od
	JOIN paths AS p ON od.parentId = p.id
)
UPDATE od
SET OdsHierarchy = p.path
FROM Temp_Dictionary_dbo_OrganisationDescendent od
JOIN paths p ON od.id = p.id

Insert Into OdsDatas (Id,OdsHierarchy, OrganisationCode, OrganisationName)
select NEWID(),OdsHierarchy, OrganisationCode_Child, ODSOrgName from Temp_Dictionary_dbo_OrganisationDescendent where IsActive=1


UPDATE OdsDatas set HasChildren=1 where id in(

select distinct o1.id
from OdsDatas o1
Left join OdsDatas o2 on o1.OdsHierarchy.GetDescendant(null,null) = o2.OdsHierarchy
where o2.id is not null)

COMMIT TRANSACTION

END