IF NOT EXISTS (SELECT 1 FROM ckbx_ItemType WHERE ItemName = 'MatrixMessage')
BEGIN
INSERT INTO ckbx_ItemType
(
       ItemDataAssemblyName ,
       ItemDataClassName ,
       DefaultAppearanceCode ,
       ItemName ,
       MobileCompatible ,
       CategoryId ,
       RTCompatible ,
       LibraryCompatible ,
       ReportCompatible ,
       Position ,
       TextIdPrefix ,
       IsAnswerable
)
VALUES
(
       'Checkbox',
       'Checkbox.Forms.Items.Configuration.MatrixMessage' ,
       'MATRIX_MESSAGE',
       'MatrixMessage',
       0 ,
       1001 ,
       0 ,
       0 ,
       0 ,
       0 ,
       null,
       0
)
END
