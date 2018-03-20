-- Insert heat map type name, so it can be found on page load

IF NOT EXISTS (SELECT 1 FROM ckbx_PageTextIds WHERE PageId = 1003 AND TextId = '/itemType/HeatMapSummary/name')
BEGIN
	INSERT INTO ckbx_PageTextIds
	VALUES
	(1003, '/itemType/HeatMapSummary/name')
END



