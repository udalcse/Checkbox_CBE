-- Add chart name to edit page
IF NOT EXISTS (SELECT 1 FROM ckbx_PageTextIds WHERE PageId = 1003 AND TextId = '/itemType/GradientColorDirectorSkillsMatrix/name')
BEGIN
 INSERT INTO ckbx_PageTextIds
 VALUES
 (1003, '/itemType/GradientColorDirectorSkillsMatrix/name')
END