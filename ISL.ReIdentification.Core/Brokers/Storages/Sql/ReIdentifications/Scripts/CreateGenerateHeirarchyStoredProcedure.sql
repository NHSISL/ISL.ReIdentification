SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GenerateHeirarchy]
AS
BEGIN
    DECLARE @CorrelationId UNIQUEIDENTIFIER = NEWID();
	BEGIN TRY
        -- Check if the inbound tables exists
        IF OBJECT_ID('Dictionary.OrganisationDescendent') IS NULL
        BEGIN
            THROW 50000, 'Inbound Dictionary.OrganisationDescendent Table Missing.', 1;
        END

        IF OBJECT_ID('Dictionary.Organisation') IS NULL
        BEGIN
            THROW 50000, 'Inbound Dictionary.Organisation Table Missing.', 1;
        END

        -- Check if the inbound table has data
        IF (SELECT COUNT(*) FROM Dictionary.OrganisationDescendent) = 0
        BEGIN
            THROW 50000, N'Inbound Dictionary.OrganisationDescendent Table is empty.', 1;
        END

        IF (SELECT COUNT(*) FROM Dictionary.Organisation) = 0
        BEGIN
            RAISERROR(N'Inbound Dictionary.Organisation Table is empty.', 16, 1);
            RETURN;
        END

        RAISERROR('Load Commenced',0,1) WITH NOWAIT;
        
        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'ODSLoad', 'Load Commenced', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE())

        RAISERROR('Create Temp Table',0,1) WITH NOWAIT;
        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'ODSLoad', 'Create Temp Table', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE())

        -- Clear existing data in OdsDatas table
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
            [ODSOrgName] [varchar](255) NULL,
            [IsActive] [bit] NULL Default(1),
        ) ON [PRIMARY]

        INSERT INTO TempOdsInbound (OrganisationCode_Child, IsActive) VALUES ('ROOT',1)

        INSERT INTO TempOdsInbound 
        (
            [SK_OrganisationID_Root]        ,
            [OrganisationCode_Root]         ,
            [OrganisationPrimaryRole_Root]  ,
            [SK_OrganisationID_Parent]      ,
            [OrganisationCode_Parent]       ,
            [OrganisationPrimaryRole_Parent],
            [SK_OrganisationID_Child]       ,
            [OrganisationCode_Child]        ,
            [OrganisationPrimaryRole_Child] ,
            [RelationshipType]              ,
            [RelationshipStartDate]         ,
            [RelationshipEndDate]           ,
            [Path]                          ,
            [Depth]                         ,
            [PathStartDate]                 ,
            [PathEndDate]                   ,
            [DateAdded]                     ,
            [DateUpdated]                   ,
            [ODSOrgName]                    ,
            [IsActive]              
        )
        SELECT      
            [SK_OrganisationID_Root]        ,
            [OrganisationCode_Root]         ,
            [OrganisationPrimaryRole_Root]  ,
            [SK_OrganisationID_Parent]      ,
            [OrganisationCode_Parent]       ,
            [OrganisationPrimaryRole_Parent],
            [SK_OrganisationID_Child]       ,
            [OrganisationCode_Child]        ,
            [OrganisationPrimaryRole_Child] ,
            [RelationshipType]              ,
            [RelationshipStartDate]         ,
            [RelationshipEndDate]           ,
            [Path]                          ,
            [Depth]                         ,
            [PathStartDate]                 ,
            [PathEndDate]                   ,
            [DateAdded]                     ,
            [DateUpdated]                   ,
            o.Organisation_Name             ,
            1 as isActive  
        FROM
            Dictionary.OrganisationDescendent OD JOIN
            Dictionary.Organisation o on od.SK_OrganisationID_Child = o.SK_OrganisationID
        WHERE
            OrganisationCode_Root= 'Y56' 
            AND OrganisationPrimaryRole_Child in ('RO209','RO172','RO177','RO107','RO99','RO108','RO272','RO261','RO176','RO98','RO96')
            AND PathStartDate <= GETDATE()
            AND PathEndDate >= GETDATE()
            AND RelationshipStartDate <= GETDATE()
            AND RelationshipEndDate >= GETDATE();

        RAISERROR('Prepare Orgs for Loading',0,1) WITH NOWAIT;
        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'ODSLoad', 'Prepare Orgs for Loading', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE())

        -- Attach any 'SELF' relationships to the root node
        UPDATE [dbo].[TempOdsInbound]
        SET ParentId = 1, ParentPath = 'ROOT'
        WHERE RelationshipType = 'SELF';

        -- Generate readable parent path and assign a parent ID
        UPDATE [dbo].[TempOdsInbound]
        SET ParentPath = LEFT(Path, LEN(Path) - LEN(OrganisationCode_Child) - LEN(RelationshipType) - 10)
        WHERE RelationshipType <> 'SELF';

        -- Assign ParentId using derived parent path
        UPDATE od
        SET ParentId = (
            SELECT id 
            FROM [dbo].[TempOdsInbound] od1 
            WHERE od1.path = od.ParentPath
        )
        FROM [dbo].[TempOdsInbound] od
        WHERE ParentId IS NULL;


        RAISERROR('Add in Test Organisation',0,1) WITH NOWAIT;
        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'ODSLoad', 'Add Test Organisation', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE())

        INSERT INTO [dbo].[TempOdsInbound] (ParentPath, ParentId,  OrganisationCode_Child, ODSOrgName)
        VALUES ('ROOT', 1, 'ZZZ', 'TEST ORG' ) ;

        RAISERROR('Generate hierarchical path',0,1) WITH NOWAIT;
        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'ODSLoad', 'Generate hierarchical path', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE());

        -- Assign row numbers for each child
        UPDATE od
        SET RowNumber = rn
        FROM [dbo].[TempOdsInbound] od
        JOIN (
            SELECT id, ROW_NUMBER() OVER (PARTITION BY ParentId ORDER BY ParentId) AS rn 
            FROM [dbo].[TempOdsInbound]
        ) od1 ON od.id = od1.id;

        -- Recursive CTE to calculate hierarchical paths
        WITH paths(path, Id)
        AS (
            -- Handle the root node
            SELECT 
                hierarchyid::GetRoot(),
                id
            FROM [dbo].[TempOdsInbound]
            WHERE ParentId IS NULL

            UNION ALL

            -- Handle all other nodes
            SELECT 
                CAST(p.path.ToString() + CAST(od.RowNumber AS VARCHAR(30)) + '/' AS hierarchyid), 
                od.Id 
            FROM [dbo].[TempOdsInbound] od
            JOIN paths AS p ON od.ParentId = p.Id
        )
        UPDATE od
        SET OdsHierarchy = p.path
        FROM [dbo].[TempOdsInbound] od
        JOIN paths p ON od.Id = p.Id;


        RAISERROR('Create Temp OdsTable',0,1) WITH NOWAIT;
        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'ODSLoad', 'Create Temp OdsTable', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE())

        CREATE TABLE [dbo].[TempOdsDatas](
            [Id] [uniqueidentifier] NOT NULL,
            [RelationshipWithParentStartDate] [datetimeoffset](7) NULL,
            [RelationshipWithParentEndDate] [datetimeoffset](7) NULL,
            [OdsHierarchy] [hierarchyid] NULL,
            [OrganisationCode] [nvarchar](15) NOT NULL,
            [OrganisationName] [nvarchar](255) NULL,
            [HasChildren] [bit] NOT NULL
        ) ON [PRIMARY]
        ALTER TABLE [dbo].[TempOdsDatas] ADD  DEFAULT (CONVERT([bit],(0))) FOR [HasChildren]

        RAISERROR('Load OdsTable',0,1) WITH NOWAIT;
        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'ODSLoad', 'Load OdsTable', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE())

        -- Insert data into OdsDatas
        INSERT INTO dbo.TempOdsDatas (Id, OdsHierarchy, OrganisationCode, OrganisationName)
        SELECT NEWID(), OdsHierarchy, OrganisationCode_Child, ODSOrgName
        FROM [dbo].[TempOdsInbound]
        WHERE IsActive = 1;

        -- Mark records with children in OdsDatas
        UPDATE dbo.TempOdsDatas
        SET HasChildren = 1
        WHERE Id IN (
            SELECT DISTINCT o1.Id
            FROM dbo.TempOdsDatas o1
            LEFT JOIN dbo.TempOdsDatas o2 ON o1.OdsHierarchy.GetDescendant(NULL, NULL) = o2.OdsHierarchy
            WHERE o2.Id IS NOT NULL
        );

        RAISERROR('Create Primary Key',0,1) WITH NOWAIT;
        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'ODSLoad', 'Create Primary Key', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE())

        DECLARE @SQL NVARCHAR(MAX) = 'ALTER TABLE [dbo].[TempOdsDatas] ADD  CONSTRAINT [PK_OdsDatas_' + CAST(NEWID() as nvarchar(36)) + '] PRIMARY KEY CLUSTERED ' +
                            '([Id] ASC)' + 
                            'WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ' +
                            'ON [PRIMARY]'
        PRINT @SQL
        EXEC sp_executesql @SQL

        RAISERROR('Swapping Live and Temp Table',0,1) WITH NOWAIT;
        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'ODSLoad', 'Swapping Live and Temp Table', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE())

        DECLARE @BackupSqlName NVARCHAR(MAX) = 'OdsDatasBak_' + FORMAT(GETUTCDATE(),'yyyyMMddHHmmss')
        exec sp_rename 'dbo.OdsDatas',  @BackupSqlName
        exec sp_rename 'dbo.TempOdsDatas', 'OdsDatas'

        IF OBJECT_ID('dbo.TempOdsInbound') IS NOT NULL
        BEGIN
            RAISERROR('Dropping Temp Table',0,1) WITH NOWAIT;
            
            INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
            VALUES (NEWID(), @CorrelationId, 'ODSLoad', 'Dropping Temp Table', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE());

            DROP TABLE [dbo].[TempOdsInbound]
        END;

        RAISERROR('Load Complete',0,1) WITH NOWAIT;
        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'ODSLoad', 'Load Complete', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE())

    END TRY
    BEGIN CATCH

        DECLARE @ErrorMessage NVARCHAR(4000);
        SET @ErrorMessage = 'ODSLoad Load Failed: ' + ERROR_MESSAGE();

        RAISERROR(@ErrorMessage,0,1) WITH NOWAIT;

        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'ODSLoad', @ErrorMessage, 'Error', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE());

        IF OBJECT_ID('dbo.TempOdsDatas') IS NOT NULL
        BEGIN
            RAISERROR('Dropping Temp Table',0,1) WITH NOWAIT;
            
            INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
            VALUES (NEWID(), @CorrelationId, 'ODSLoad', 'Dropping Temp Table', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE());

            DROP TABLE [dbo].[TempOdsDatas]
        END;

        IF OBJECT_ID('dbo.TempOdsInbound') IS NOT NULL
        BEGIN
            RAISERROR('Dropping Temp Table',0,1) WITH NOWAIT;
            
            INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
            VALUES (NEWID(), @CorrelationId, 'ODSLoad', 'Dropping Temp Table', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE());

            DROP TABLE [dbo].[TempOdsInbound]
        END;

        THROW;
    END CATCH
END;
GO
