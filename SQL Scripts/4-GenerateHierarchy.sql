IF OBJECT_ID('dbo.GenerateHierarchy') IS NOT NULL
	DROP PROCEDURE dbo.GenerateHierarchy
GO
CREATE PROCEDURE [dbo].[GenerateHierarchy]
AS

WITH paths(path, Id)   
AS (  
	-- Handle the root node
	SELECT 
		hierarchyid::GetRoot(),
		id
	FROM TempSample_Dictionary_dbo_OrganisationDescendent
	WHERE ParentId is NULL

	UNION ALL
	
	-- Handle all other nodes
	SELECT 
		CAST(p.path.ToString() + CAST(od.RowNumber AS varchar(30)) + '/' AS hierarchyid), 
		od.Id 
	FROM TempSample_Dictionary_dbo_OrganisationDescendent od
	JOIN paths AS p ON od.parentId = p.id
)
UPDATE od
SET OdsHierarchy = p.path
FROM TempSample_Dictionary_dbo_OrganisationDescendent od
JOIN paths p ON od.id = p.id

GO
EXEC [dbo].[GenerateHierarchy]
