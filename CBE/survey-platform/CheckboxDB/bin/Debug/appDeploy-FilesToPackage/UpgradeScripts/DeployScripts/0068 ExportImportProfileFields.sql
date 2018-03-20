--Export
-- add a new text for new page that will be displyed in the dialog
EXEC ckbx_sp_Text_Set '/pageText/settings/customFieldsExport.aspx/title', 'en-US', N'Export Custom Fields'

-- add header for select custom fields
EXEC ckbx_sp_Text_Set '/pageText/settings/customUserFields.aspx/selectFields', 'en-US', N'Select Custom Fields'


--Import
EXEC ckbx_sp_Text_Set '/pageText/settings/customFieldsImport.aspx/title', 'en-US', N'Import Custom Fields'


