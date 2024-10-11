select * from TempSample_Dictionary_dbo_OrganisationDescendent 

-- Gets All Decendants (children, grandchildren, etc) of a given Organisation
DECLARE @CurrentOrg HIERARCHYID
select @CurrentOrg =OdsHierarchy from TempSample_Dictionary_dbo_OrganisationDescendent where OrganisationCode_Child  = '72Q'
select OrganisationCode_Child, ODSHierarchy.ToString() from TempSample_Dictionary_dbo_OrganisationDescendent where OdsHierarchy.IsDescendantOf(@CurrentOrg) =1

-- Gets All direct children (not grandchildren etc) of a given Organisation
DECLARE @CurrentOrg HIERARCHYID
select @CurrentOrg = OdsHierarchy from TempSample_Dictionary_dbo_OrganisationDescendent where OrganisationCode_Child = '72Q'
select OrganisationCode_Child, ODSHierarchy.ToString() from TempSample_Dictionary_dbo_OrganisationDescendent where OdsHierarchy.GetAncestor(1) = @CurrentOrg