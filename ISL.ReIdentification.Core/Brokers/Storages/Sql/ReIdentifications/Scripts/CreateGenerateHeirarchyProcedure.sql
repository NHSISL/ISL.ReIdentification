CREATE PROCEDURE [dbo].[GenerateHeirarchy]
AS
BEGIN
    -- Check if the temporary table exists
    IF OBJECT_ID('dbo.Temp_Dictionary_dbo_OrganisationDescendent') IS NULL
    BEGIN
        THROW 50000, 'Temp Table does not exist.', 1;
    END

    -- Check if the temporary table has data
    IF (SELECT COUNT(*) FROM dbo.Temp_Dictionary_dbo_OrganisationDescendent) = 0
    BEGIN
        RAISERROR(N'Temp Table is empty.', 16, 1);
        RETURN;
    END

    -- Begin transaction for data insertion and updates
    BEGIN TRANSACTION;

    -- Clear existing data in OdsDatas table
    TRUNCATE TABLE dbo.OdsDatas;

    -- Set active status for valid records
    UPDATE dbo.Temp_Dictionary_dbo_OrganisationDescendent
    SET IsActive = 1
    WHERE PathStartDate <= GETDATE()
      AND PathEndDate >= GETDATE()
      AND RelationshipStartDate <= GETDATE()
      AND RelationshipEndDate >= GETDATE();

    -- Attach any 'SELF' relationships to the root node
    UPDATE dbo.Temp_Dictionary_dbo_OrganisationDescendent
    SET ParentId = 1, ParentPath = 'ROOT'
    WHERE RelationshipType = 'SELF';

    -- Generate readable parent path and assign a parent ID
    UPDATE dbo.Temp_Dictionary_dbo_OrganisationDescendent
    SET ParentPath = LEFT(Path, LEN(Path) - LEN(OrganisationCode_Child) - LEN(RelationshipType) - 10)
    WHERE RelationshipType <> 'SELF';

    -- Assign ParentId using derived parent path
    UPDATE od
    SET ParentId = (
        SELECT id 
        FROM dbo.Temp_Dictionary_dbo_OrganisationDescendent od1 
        WHERE od1.path = od.ParentPath
    )
    FROM dbo.Temp_Dictionary_dbo_OrganisationDescendent od
    WHERE ParentId IS NULL;

    -- Assign row numbers for each child
    UPDATE od
    SET RowNumber = rn
    FROM dbo.Temp_Dictionary_dbo_OrganisationDescendent od
    JOIN (
        SELECT id, ROW_NUMBER() OVER (PARTITION BY ParentId ORDER BY ParentId) AS rn 
        FROM dbo.Temp_Dictionary_dbo_OrganisationDescendent
    ) od1 ON od.id = od1.id;

    -- Recursive CTE to calculate hierarchical paths
    WITH paths(path, Id)
    AS (
        -- Handle the root node
        SELECT 
            hierarchyid::GetRoot(),
            id
        FROM dbo.Temp_Dictionary_dbo_OrganisationDescendent
        WHERE ParentId IS NULL

        UNION ALL

        -- Handle all other nodes
        SELECT 
            CAST(p.path.ToString() + CAST(od.RowNumber AS VARCHAR(30)) + '/' AS hierarchyid), 
            od.Id 
        FROM dbo.Temp_Dictionary_dbo_OrganisationDescendent od
        JOIN paths AS p ON od.ParentId = p.Id
    )
    UPDATE od
    SET OdsHierarchy = p.path
    FROM dbo.Temp_Dictionary_dbo_OrganisationDescendent od
    JOIN paths p ON od.Id = p.Id;

    -- Insert data into OdsDatas
    INSERT INTO dbo.OdsDatas (Id, OdsHierarchy, OrganisationCode, OrganisationName)
    SELECT NEWID(), OdsHierarchy, OrganisationCode_Child, ODSOrgName
    FROM dbo.Temp_Dictionary_dbo_OrganisationDescendent
    WHERE IsActive = 1;

    -- Mark records with children in OdsDatas
    UPDATE dbo.OdsDatas
    SET HasChildren = 1
    WHERE Id IN (
        SELECT DISTINCT o1.Id
        FROM dbo.OdsDatas o1
        LEFT JOIN dbo.OdsDatas o2 ON o1.OdsHierarchy.GetDescendant(NULL, NULL) = o2.OdsHierarchy
        WHERE o2.Id IS NOT NULL
    );

    -- Commit transaction after all operations are successful
    COMMIT TRANSACTION;
END;