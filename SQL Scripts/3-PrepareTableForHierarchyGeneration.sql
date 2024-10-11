IF(OBJECT_ID('dbo.PrepareTableForHierarchyGeneration') is not null)
	DROP PROCEDURE dbo.PrepareTableForHierarchyGeneration
GO
CREATE PROCEDURE [dbo].[PrepareTableForHierarchyGeneration]
AS

	-- Attach any 'SELF' relationships to the root node
	UPDATE TempSample_Dictionary_dbo_OrganisationDescendent 
	SET ParentId = 1, ParentPath = 'ROOT'
	WHERE RelationshipType = 'SELF'

    -- Generate easier to read parent path then use that to assign a parent id
	UPDATE TempSample_Dictionary_dbo_OrganisationDescendent 
	SET ParentPath = LEFT(Path, LEN(Path) - LEN(OrganisationCode_Child) - LEN(RelationshipType)-10) 
	WHERE RelationshipType <> 'SELF'

	-- Use the derived parent path to assign a parent id
	UPDATE od
	SET ParentId = (
		SELECT id 
		FROM TempSample_Dictionary_dbo_OrganisationDescendent od1 
		WHERE od1.path = od.parentPath
		)
	FROM TempSample_Dictionary_dbo_OrganisationDescendent od
	Where ParentId is NULL

	-- Assign Row Numbers for each child
	UPDATE od
	SET RowNumber = rn
	FROM TempSample_Dictionary_dbo_OrganisationDescendent od
	JOIN (
		SELECT id, ROW_NUMBER() OVER (PARTITION BY ParentId ORDER BY ParentId) AS rn 
		FROM TempSample_Dictionary_dbo_OrganisationDescendent) od1
			ON od.id = od1.id
GO
EXEC [dbo].[PrepareTableForHierarchyGeneration]

select * from TempSample_Dictionary_dbo_OrganisationDescendent order by ParentId, RowNumber