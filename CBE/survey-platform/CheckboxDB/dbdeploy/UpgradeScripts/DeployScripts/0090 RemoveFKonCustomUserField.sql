IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_MatrixResponseState_ckbx_CustomUserField]') AND parent_object_id = OBJECT_ID(N'[ckbx_MatrixResponseState]'))
BEGIN
	ALTER TABLE [ckbx_MatrixResponseState] DROP CONSTRAINT [FK_ckbx_MatrixResponseState_ckbx_CustomUserField]
END
GO