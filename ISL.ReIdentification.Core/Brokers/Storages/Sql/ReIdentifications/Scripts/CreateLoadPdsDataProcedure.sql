CREATE PROCEDURE [dbo].[LoadPDSData]
AS
BEGIN
    -- Check if the temporary table exists
    IF OBJECT_ID('dbo.TempSample_PDS_PDS_PATIENT_CARE_PRACTICE') IS NULL
        THROW 50000, 'Temp Table does not exist.', 1;

    -- Check if the temporary table has data
    IF (SELECT COUNT(*) FROM dbo.TempSample_PDS_PDS_PATIENT_CARE_PRACTICE) = 0
        THROW 50000, 'Temp Table is empty.', 1;

    -- Begin transaction for data insertion
    BEGIN TRANSACTION;

    -- Clear existing data in PDSDatas table
    TRUNCATE TABLE dbo.PDSDatas;

    -- Insert new data from the temporary table with unpivoted columns
    INSERT INTO dbo.PDSDatas (PseudoNhsNumber, OrgCode, ID)
    SELECT [Pseudo NHS Number], OdsCode, NEWID()
    FROM (
        SELECT 
            [Pseudo NHS Number],
            [Primary Care Provider],
            CAST([derCurrentCcgOfRegistration] AS VARCHAR(8)) AS [derCurrentCcgOfRegistration],
            CAST([derCurrentIcbOfRegistration] AS VARCHAR(8)) AS [derCurrentIcbOfRegistration]
        FROM dbo.TempSample_PDS_PDS_PATIENT_CARE_PRACTICE
    ) AS SourceTable
    UNPIVOT (OdsCode FOR ODSRel IN ([Primary Care Provider], [derCurrentCcgOfRegistration], [derCurrentIcbOfRegistration])) AS Unpiv;

    -- Commit transaction after successful insertion
    COMMIT TRANSACTION;
END;