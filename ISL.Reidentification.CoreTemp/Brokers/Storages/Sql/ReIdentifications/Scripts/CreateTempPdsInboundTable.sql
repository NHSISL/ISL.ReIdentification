-- Check if the table already exists and drop it if so
IF OBJECT_ID('dbo.TempPdsInbound', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.TempPdsInbound;
END

CREATE TABLE dbo.TempPdsInbound
(
	[RowID] [bigint] NOT NULL,
	[Pseudo NHS Number] [varchar](15) NULL,
	[Primary Care Provider] [varchar](8) NULL,
	[Primary Care Provider Business Effective From Date] [date] NULL,
	[Primary Care Provider Business Effective To Date] [date] NULL,
	[Reason for Removal] [varchar](50) NULL,
	[derCcgOfRegistration] [varchar](5) NULL,
	[derCurrentCcgOfRegistration] [varchar](5) NULL,
	[derIcbOfRegistration] [varchar](5) NULL,
	[derCurrentIcbOfRegistration] [varchar](5) NULL
);
