if(Object_ID('[dbo].[LoadPDSData]') is not null)
	DROP PROCEDURE [dbo].[LoadPDSData]
GO
CREATE PROCEDURE [dbo].[LoadPDSData] AS
BEGIN

if(Object_ID('dbo.TempSample_PDS_PDS_PATIENT_CARE_PRACTICE') is null)
	THROW 50000, 'Temp Table does not exist.', 1;

if((select count(*) from TempSample_PDS_PDS_PATIENT_CARE_PRACTICE) = 0)
	THROW 50000, 'Temp Table is empty.', 1;

BEGIN TRANSACTION
TRUNCATE TABLE PDSDatas


INSERT INTO PDSDatas (PseudoNhsNumber, OrgCode, ID) 
select [Pseudo NHS Number], ODSCode, NEWID() from 
(
select [Pseudo NHS Number],[Primary Care Provider],cast ([derCurrentCcgOfRegistration] as varchar(8)) [derCurrentCcgOfRegistration],cast([derCurrentIcbOfRegistration] as varchar(8)) [derCurrentIcbOfRegistration] from tempSample_PDS_PDS_PATIENT_CARE_PRACTICE
) d
unpivot(OdsCode for ODSRel in ([Primary Care Provider], [derCurrentCcgOfRegistration], [derCurrentIcbOfRegistration])) unpiv

COMMIT TRANSACTION
END 