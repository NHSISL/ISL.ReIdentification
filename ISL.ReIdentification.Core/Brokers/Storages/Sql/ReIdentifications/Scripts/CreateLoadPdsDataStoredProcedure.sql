SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[LoadPDSData]
AS
BEGIN
    DECLARE @CorrelationId UNIQUEIDENTIFIER = NEWID();
    SET NOCOUNT ON
    BEGIN TRY 
        -- Check if the temporary table exists
        IF OBJECT_ID('pds.PDS_PATIENT_CARE_PRACTICE') IS NULL
            THROW 50000, 'Inbound Table does not exist.', 1;

        -- Check if the temporary table has data
        IF (SELECT COUNT(*) FROM pds.PDS_PATIENT_CARE_PRACTICE) = 0
            THROW 50000, 'Inbound Table is empty.', 1;

        IF OBJECT_ID('dbo.TempPdsDatas') IS NOT NULL
            THROW 50000, 'Temp Table already exists.', 1;

        RAISERROR('Load Commenced',0,1) WITH NOWAIT;
    
        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'PDSLoad', 'Load Commenced', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE())

        CREATE TABLE [dbo].[TempPdsDatas](
            [PseudoNhsNumber] [nvarchar](10) NOT NULL,
            [RelationshipWithOrganisationEffectiveFromDate] [datetimeoffset](7) NULL,
            [RelationshipWithOrganisationEffectiveToDate] [datetimeoffset](7) NULL,
            [Id] [uniqueidentifier] NOT NULL,
            [OrgCode] [nvarchar](15) NOT NULL,
            [OrganisationName] [nvarchar](max) NULL
        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

        RAISERROR('Load Temp Table Created',0,1) WITH NOWAIT;
        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'PDSLoad', 'Load Temp Table Created', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE())

        RAISERROR('Load Commenced into Temp Table',0,1) WITH NOWAIT;
        -- Begin transaction for data insertion
        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'PDSLoad', 'Load Commenced into Temp Table', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE())

        -- Insert new data from the temporary table with unpivoted columns
        INSERT INTO dbo.TempPdsDatas (PseudoNhsNumber, OrgCode, ID)
        (SELECT 
            [PseudoNhsNumber],
            [OrgCode],
            NEWID()
        FROM pds.PDS_PATIENT_CARE_PRACTICE
        where OrgCode is not null and PseudoNhsNumber is not null)

        RAISERROR('Temp Table Load Completed',0,1) WITH NOWAIT;
        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'PDSLoad', 'Temp Table Load Completed', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE())

        DECLARE @CompleteMessage NVARCHAR(255) = 'Load Complete - added ' + CAST(@@ROWCOUNT as VARCHAR(15)) + ' PDS Records'

        RAISERROR('Adding Test Records',0,1) WITH NOWAIT;
        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'PDSLoad', 'Adding Test Records', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE())
        EXEC [dbo].[LoadLiveTestPseudos]

        RAISERROR('Creating Primary Keys',0,1) WITH NOWAIT;
        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'PDSLoad', 'Creating Primary Keys', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE())
        
        DECLARE @SQL NVARCHAR(MAX) = 'ALTER TABLE [dbo].[TempPdsDatas] ADD  CONSTRAINT [PK_PdsDatas_' + CAST(NEWID() as nvarchar(36)) + '] PRIMARY KEY CLUSTERED' +
                                    '([Id] ASC)' + 
                                    'WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)' +
                                    'ON [PRIMARY]'        
        EXEC sp_executesql @SQL

        RAISERROR('Creating Pseudo Id Index',0,1) WITH NOWAIT;
        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'PDSLoad', 'Creating Pseudo Id Index', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE())
        
        CREATE NONCLUSTERED INDEX [IX_PdsDatas_PseudoNhsNumber] ON [dbo].[TempPdsDatas]
        (
            [PseudoNhsNumber] ASC
        )WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

        RAISERROR('Creating Ods code Index',0,1) WITH NOWAIT;
        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'PDSLoad', 'Creating Ods code Index', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE())

        CREATE NONCLUSTERED INDEX [IX_PdsDatas_OrgCode] ON [dbo].[TempPdsDatas]
        (
            [OrgCode] ASC
        )WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

        RAISERROR('Adding Defaults',0,1) WITH NOWAIT;
        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'PDSLoad', 'Adding Defaults', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE())

        ALTER TABLE [dbo].[TempPdsDatas] ADD  DEFAULT (N'') FOR [PseudoNhsNumber]

        ALTER TABLE [dbo].[TempPdsDatas] ADD  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [Id]

        ALTER TABLE [dbo].[TempPdsDatas] ADD  DEFAULT (N'') FOR [OrgCode]



        RAISERROR('Swapping Live and Temp Table',0,1) WITH NOWAIT;
        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'PDSLoad', 'Swapping Live and Temp Table', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE())

        DECLARE @BackupSqlName NVARCHAR(MAX) = 'PDSDatasBak_' + FORMAT(GETUTCDATE(),'yyyyMMddHHmmss')
        exec sp_rename 'dbo.PDSDatas',  @BackupSqlName
        exec sp_rename 'dbo.TempPdsDatas', 'PDSDatas'

        RAISERROR(@CompleteMessage,0,1) WITH NOWAIT;
        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'PDSLoad', @CompleteMessage, 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE())
    END TRY
    BEGIN CATCH

        DECLARE @ErrorMessage NVARCHAR(4000);
        SET @ErrorMessage = 'PDS Load Failed: ' + ERROR_MESSAGE();

        RAISERROR(@ErrorMessage,0,1) WITH NOWAIT;

        INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
        VALUES (NEWID(), @CorrelationId, 'PDSLoad', @ErrorMessage, 'Error', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE());

        IF OBJECT_ID('dbo.TempPdsDatas') IS NOT NULL
        BEGIN
            RAISERROR('Dropping Temp Table',0,1) WITH NOWAIT;
            
            INSERT INTO Audits (id, CorrelationId, AuditType, AuditDetail, LogLevel, CreatedBy, CreatedDate,  UpdatedBy, UpdatedDate ) 
            VALUES (NEWID(), @CorrelationId, 'PDSLoad', 'Dropping Temp Table', 'Info', CURRENT_USER, GETUTCDATE(),  CURRENT_USER, GETUTCDATE());

            DROP TABLE [dbo].[TempPdsDatas]
        END;

        THROW;
    END CATCH
END;
GO

