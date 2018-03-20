exec ckbx_sp_Text_Set '/itemType/GovernancePrioritySummary/name', 'en-US', N'Corporate Governance Priority Graph' 
GO

IF NOT EXISTS(SELECT 1 FROM ckbx_PageTextIds WHERE TextId = '/itemType/GovernancePrioritySummary/name')
BEGIN
	INSERT INTO ckbx_PageTextIds
			   (PageId
			   ,TextId)
		 VALUES
			   (1003, '/itemType/GovernancePrioritySummary/name')
END    
  
GO

