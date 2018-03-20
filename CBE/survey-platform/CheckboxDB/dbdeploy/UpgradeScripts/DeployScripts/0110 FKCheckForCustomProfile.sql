
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldRadioOption_User_ckbx_CustomUserFieldRadioOption]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldRadioOption_User]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldRadioOption_User] DROP CONSTRAINT [FK_ckbx_CustomUserFieldRadioOption_User_ckbx_CustomUserFieldRadioOption]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldRadioOption_Alias_ckbx_CustomUserFieldRadioOption]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldRadioOption_Alias]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldRadioOption_Alias] DROP CONSTRAINT [FK_ckbx_CustomUserFieldRadioOption_Alias_ckbx_CustomUserFieldRadioOption]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldRadioOption_ckbx_CustomUserFieldRadio]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldRadioOption]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldRadioOption] DROP CONSTRAINT [FK_ckbx_CustomUserFieldRadioOption_ckbx_CustomUserFieldRadio]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldRadio_ckbx_CustomUserField]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldRadio]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldRadio] DROP CONSTRAINT [FK_ckbx_CustomUserFieldRadio_ckbx_CustomUserField]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldMatrixCell_User_ckbx_CustomUserFieldMatrix]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMatrixCell_User]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldMatrixCell_User] DROP CONSTRAINT [FK_ckbx_CustomUserFieldMatrixCell_User_ckbx_CustomUserFieldMatrix]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrixCell]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMatrixCell]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldMatrixCell] DROP CONSTRAINT [FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrixCell]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrix]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMatrixCell]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldMatrixCell] DROP CONSTRAINT [FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrix]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldMatrix_ckbx_CustomUserField]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMatrix]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldMatrix] DROP CONSTRAINT [FK_ckbx_CustomUserFieldMatrix_ckbx_CustomUserField]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldMap_ckbx_CustomUserField]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMap]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldMap] DROP CONSTRAINT [FK_ckbx_CustomUserFieldMap_ckbx_CustomUserField]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldItemMap_ckbx_CustomUserField]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldItemMap]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldItemMap] DROP CONSTRAINT [FK_ckbx_CustomUserFieldItemMap_ckbx_CustomUserField]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserField_ckbx_CustomUserFieldType]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserField]'))
ALTER TABLE [dbo].[ckbx_CustomUserField] DROP CONSTRAINT [FK_ckbx_CustomUserField_ckbx_CustomUserFieldType]
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_ckbx_CustomUserFieldMatrixCell_User_Active]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ckbx_CustomUserFieldMatrixCell_User] DROP CONSTRAINT [DF_ckbx_CustomUserFieldMatrixCell_User_Active]
END

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_ckbx_CustomUserFieldMatrixCell_IsActive]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ckbx_CustomUserFieldMatrixCell] DROP CONSTRAINT [DF_ckbx_CustomUserFieldMatrixCell_IsActive]
END

GO
/****** Object:  Table [dbo].[ckbx_CustomUserFieldRadioOption_User]    Script Date: 4/3/2017 6:00:18 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldRadioOption_User]') AND type in (N'U'))
DROP TABLE [dbo].[ckbx_CustomUserFieldRadioOption_User]
GO
/****** Object:  Table [dbo].[ckbx_CustomUserFieldRadioOption_Alias]    Script Date: 4/3/2017 6:00:18 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldRadioOption_Alias]') AND type in (N'U'))
DROP TABLE [dbo].[ckbx_CustomUserFieldRadioOption_Alias]
GO
/****** Object:  Table [dbo].[ckbx_CustomUserFieldRadioOption]    Script Date: 4/3/2017 6:00:18 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldRadioOption]') AND type in (N'U'))
DROP TABLE [dbo].[ckbx_CustomUserFieldRadioOption]
GO
/****** Object:  Table [dbo].[ckbx_CustomUserFieldRadio]    Script Date: 4/3/2017 6:00:18 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldRadio]') AND type in (N'U'))
DROP TABLE [dbo].[ckbx_CustomUserFieldRadio]
GO
/****** Object:  Table [dbo].[ckbx_CustomUserFieldMatrixCell_User]    Script Date: 4/3/2017 6:00:18 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMatrixCell_User]') AND type in (N'U'))
DROP TABLE [dbo].[ckbx_CustomUserFieldMatrixCell_User]
GO
/****** Object:  Table [dbo].[ckbx_CustomUserFieldMatrixCell]    Script Date: 4/3/2017 6:00:18 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMatrixCell]') AND type in (N'U'))
DROP TABLE [dbo].[ckbx_CustomUserFieldMatrixCell]
GO
/****** Object:  Table [dbo].[ckbx_CustomUserFieldMatrix]    Script Date: 4/3/2017 6:00:18 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMatrix]') AND type in (N'U'))
DROP TABLE [dbo].[ckbx_CustomUserFieldMatrix]
GO
/****** Object:  Table [dbo].[ckbx_CustomUserFieldMap]    Script Date: 4/3/2017 6:00:18 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMap]') AND type in (N'U'))
DROP TABLE [dbo].[ckbx_CustomUserFieldMap]
GO
/****** Object:  Table [dbo].[ckbx_CustomUserFieldItemMap]    Script Date: 4/3/2017 6:00:18 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldItemMap]') AND type in (N'U'))
DROP TABLE [dbo].[ckbx_CustomUserFieldItemMap]
GO
/****** Object:  Table [dbo].[ckbx_CustomUserField]    Script Date: 4/3/2017 6:00:18 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserField]') AND type in (N'U'))
DROP TABLE [dbo].[ckbx_CustomUserField]
GO
/****** Object:  Table [dbo].[ckbx_CustomUserField]    Script Date: 4/3/2017 6:00:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserField]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ckbx_CustomUserField](
	[CustomUserFieldID] [int] IDENTITY(1,1) NOT NULL,
	[CustomUserFieldName] [nvarchar](510) NULL,
	[Position] [int] NULL,
	[CustomUserFieldTypeID] [int] NULL,
	[IsDeletable] [bit] NULL,
	[Hidden] [bit] NULL,
	[ShowInUserManager] [bit] NULL,
 CONSTRAINT [PK_ckbx_CustomUserField] PRIMARY KEY NONCLUSTERED 
(
	[CustomUserFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ckbx_CustomUserFieldItemMap]    Script Date: 4/3/2017 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldItemMap]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ckbx_CustomUserFieldItemMap](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomUserFieldId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
 CONSTRAINT [PK__ckbx_Cus__3214EC0764AADDB8] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ckbx_CustomUserFieldMap]    Script Date: 4/3/2017 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMap]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ckbx_CustomUserFieldMap](
	[UniqueIdentifier] [nvarchar](611) NOT NULL,
	[CustomUserFieldID] [int] NOT NULL,
	[Value] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ckbx_CustomUserFieldMatrix]    Script Date: 4/3/2017 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMatrix]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ckbx_CustomUserFieldMatrix](
	[MatrixID] [int] IDENTITY(1,1) NOT NULL,
	[FieldName] [nvarchar](450) NULL,
	[IsRowsFixed] [bit] NULL,
	[IsColumnsFixed] [bit] NULL,
	[GridLines] [varchar](100) NULL,
	[CustomUserFieldID] [int] NULL,
 CONSTRAINT [PK_ckbx_CustomUserFieldMatrix] PRIMARY KEY CLUSTERED 
(
	[MatrixID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_ckbx_CustomUserFieldMatrix] UNIQUE NONCLUSTERED 
(
	[MatrixID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ckbx_CustomUserFieldMatrixCell]    Script Date: 4/3/2017 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMatrixCell]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ckbx_CustomUserFieldMatrixCell](
	[CellID] [int] IDENTITY(1,1) NOT NULL,
	[RowNumber] [int] NOT NULL,
	[ColumnNumber] [int] NOT NULL,
	[Data] [nvarchar](max) NULL,
	[IsHeader] [bit] NOT NULL,
	[IsRowHeader] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[MatrixID] [int] NOT NULL,
 CONSTRAINT [PK_ckbx_CustomUserFieldMatrixCell] PRIMARY KEY CLUSTERED 
(
	[CellID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ckbx_CustomUserFieldMatrixCell_User]    Script Date: 4/3/2017 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMatrixCell_User]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ckbx_CustomUserFieldMatrixCell_User](
	[UserCellID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[RowNumber] [int] NOT NULL,
	[ColumnNumber] [int] NOT NULL,
	[Data] [nvarchar](max) NULL,
	[IsHeader] [bit] NOT NULL,
	[IsRowHeader] [bit] NOT NULL,
	[Active] [bit] NOT NULL,
	[MatrixID] [int] NOT NULL,
 CONSTRAINT [PK_ckbx_CustomUserFieldMatrixCell_User] PRIMARY KEY CLUSTERED 
(
	[UserCellID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ckbx_CustomUserFieldRadio]    Script Date: 4/3/2017 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldRadio]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ckbx_CustomUserFieldRadio](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FieldName] [nvarchar](255) NULL,
	[DefaultSelectedOptionId] [int] NULL,
	[CustomUserFieldID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ckbx_CustomUserFieldRadioOption]    Script Date: 4/3/2017 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldRadioOption]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ckbx_CustomUserFieldRadioOption](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OptionText] [nvarchar](max) NULL,
	[RadioButtonId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ckbx_CustomUserFieldRadioOption_Alias]    Script Date: 4/3/2017 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldRadioOption_Alias]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ckbx_CustomUserFieldRadioOption_Alias](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ItemId] [int] NULL,
	[RadioButtonOptionId] [int] NOT NULL,
	[Alias] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ckbx_CustomUserFieldRadioOption_User]    Script Date: 4/3/2017 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldRadioOption_User]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ckbx_CustomUserFieldRadioOption_User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SelectedOptionId] [int] NULL,
	[UserID] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ckbx_CustomUserFieldType]    Script Date: 4/3/2017 6:00:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_ckbx_CustomUserField_CustomUserFieldTypeID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ckbx_CustomUserField] ADD  CONSTRAINT [DF_ckbx_CustomUserField_CustomUserFieldTypeID]  DEFAULT ((1)) FOR [CustomUserFieldTypeID]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_ckbx_CustomUserFieldMatrixCell_IsActive]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ckbx_CustomUserFieldMatrixCell] ADD  CONSTRAINT [DF_ckbx_CustomUserFieldMatrixCell_IsActive]  DEFAULT ((1)) FOR [IsActive]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_ckbx_CustomUserFieldMatrixCell_User_Active]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ckbx_CustomUserFieldMatrixCell_User] ADD  CONSTRAINT [DF_ckbx_CustomUserFieldMatrixCell_User_Active]  DEFAULT ((1)) FOR [Active]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserField_ckbx_CustomUserFieldType]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserField]'))
ALTER TABLE [dbo].[ckbx_CustomUserField]  WITH CHECK ADD  CONSTRAINT [FK_ckbx_CustomUserField_ckbx_CustomUserFieldType] FOREIGN KEY([CustomUserFieldTypeID])
REFERENCES [dbo].[ckbx_CustomUserFieldType] ([CustomUserFieldTypeID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserField_ckbx_CustomUserFieldType]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserField]'))
ALTER TABLE [dbo].[ckbx_CustomUserField] CHECK CONSTRAINT [FK_ckbx_CustomUserField_ckbx_CustomUserFieldType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldItemMap_ckbx_CustomUserField]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldItemMap]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldItemMap]  WITH CHECK ADD  CONSTRAINT [FK_ckbx_CustomUserFieldItemMap_ckbx_CustomUserField] FOREIGN KEY([CustomUserFieldId])
REFERENCES [dbo].[ckbx_CustomUserField] ([CustomUserFieldID])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldItemMap_ckbx_CustomUserField]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldItemMap]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldItemMap] CHECK CONSTRAINT [FK_ckbx_CustomUserFieldItemMap_ckbx_CustomUserField]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldMap_ckbx_CustomUserField]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMap]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldMap]  WITH CHECK ADD  CONSTRAINT [FK_ckbx_CustomUserFieldMap_ckbx_CustomUserField] FOREIGN KEY([CustomUserFieldID])
REFERENCES [dbo].[ckbx_CustomUserField] ([CustomUserFieldID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldMap_ckbx_CustomUserField]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMap]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldMap] CHECK CONSTRAINT [FK_ckbx_CustomUserFieldMap_ckbx_CustomUserField]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldMatrix_ckbx_CustomUserField]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMatrix]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldMatrix]  WITH CHECK ADD  CONSTRAINT [FK_ckbx_CustomUserFieldMatrix_ckbx_CustomUserField] FOREIGN KEY([CustomUserFieldID])
REFERENCES [dbo].[ckbx_CustomUserField] ([CustomUserFieldID])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldMatrix_ckbx_CustomUserField]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMatrix]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldMatrix] CHECK CONSTRAINT [FK_ckbx_CustomUserFieldMatrix_ckbx_CustomUserField]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrix]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMatrixCell]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldMatrixCell]  WITH CHECK ADD  CONSTRAINT [FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrix] FOREIGN KEY([MatrixID])
REFERENCES [dbo].[ckbx_CustomUserFieldMatrix] ([MatrixID])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrix]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMatrixCell]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldMatrixCell] CHECK CONSTRAINT [FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrix]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrixCell]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMatrixCell]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldMatrixCell]  WITH CHECK ADD  CONSTRAINT [FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrixCell] FOREIGN KEY([CellID])
REFERENCES [dbo].[ckbx_CustomUserFieldMatrixCell] ([CellID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrixCell]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMatrixCell]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldMatrixCell] CHECK CONSTRAINT [FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrixCell]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldMatrixCell_User_ckbx_CustomUserFieldMatrix]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMatrixCell_User]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldMatrixCell_User]  WITH CHECK ADD  CONSTRAINT [FK_ckbx_CustomUserFieldMatrixCell_User_ckbx_CustomUserFieldMatrix] FOREIGN KEY([MatrixID])
REFERENCES [dbo].[ckbx_CustomUserFieldMatrix] ([MatrixID])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldMatrixCell_User_ckbx_CustomUserFieldMatrix]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldMatrixCell_User]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldMatrixCell_User] CHECK CONSTRAINT [FK_ckbx_CustomUserFieldMatrixCell_User_ckbx_CustomUserFieldMatrix]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldRadio_ckbx_CustomUserField]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldRadio]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldRadio]  WITH CHECK ADD  CONSTRAINT [FK_ckbx_CustomUserFieldRadio_ckbx_CustomUserField] FOREIGN KEY([CustomUserFieldID])
REFERENCES [dbo].[ckbx_CustomUserField] ([CustomUserFieldID])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldRadio_ckbx_CustomUserField]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldRadio]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldRadio] CHECK CONSTRAINT [FK_ckbx_CustomUserFieldRadio_ckbx_CustomUserField]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldRadioOption_ckbx_CustomUserFieldRadio]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldRadioOption]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldRadioOption]  WITH CHECK ADD  CONSTRAINT [FK_ckbx_CustomUserFieldRadioOption_ckbx_CustomUserFieldRadio] FOREIGN KEY([RadioButtonId])
REFERENCES [dbo].[ckbx_CustomUserFieldRadio] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldRadioOption_ckbx_CustomUserFieldRadio]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldRadioOption]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldRadioOption] CHECK CONSTRAINT [FK_ckbx_CustomUserFieldRadioOption_ckbx_CustomUserFieldRadio]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldRadioOption_Alias_ckbx_CustomUserFieldRadioOption]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldRadioOption_Alias]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldRadioOption_Alias]  WITH CHECK ADD  CONSTRAINT [FK_ckbx_CustomUserFieldRadioOption_Alias_ckbx_CustomUserFieldRadioOption] FOREIGN KEY([RadioButtonOptionId])
REFERENCES [dbo].[ckbx_CustomUserFieldRadioOption] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldRadioOption_Alias_ckbx_CustomUserFieldRadioOption]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldRadioOption_Alias]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldRadioOption_Alias] CHECK CONSTRAINT [FK_ckbx_CustomUserFieldRadioOption_Alias_ckbx_CustomUserFieldRadioOption]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldRadioOption_User_ckbx_CustomUserFieldRadioOption]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldRadioOption_User]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldRadioOption_User]  WITH CHECK ADD  CONSTRAINT [FK_ckbx_CustomUserFieldRadioOption_User_ckbx_CustomUserFieldRadioOption] FOREIGN KEY([SelectedOptionId])
REFERENCES [dbo].[ckbx_CustomUserFieldRadioOption] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ckbx_CustomUserFieldRadioOption_User_ckbx_CustomUserFieldRadioOption]') AND parent_object_id = OBJECT_ID(N'[dbo].[ckbx_CustomUserFieldRadioOption_User]'))
ALTER TABLE [dbo].[ckbx_CustomUserFieldRadioOption_User] CHECK CONSTRAINT [FK_ckbx_CustomUserFieldRadioOption_User_ckbx_CustomUserFieldRadioOption]
GO


ALTER PROCEDURE [dbo].[ckbx_sp_Profile_AddMatrixField]
	@PropertyName nvarchar(510),
	@IsRowsFixed bit,
	@IsColumnsFixed bit,
	@GridLines varchar(100)
AS

SET NOCOUNT ON
	IF EXISTS (SELECT 1 FROM ckbx_CustomUserFieldMatrix WHERE FieldName = @PropertyName)
	BEGIN		
		UPDATE	ckbx_CustomUserFieldMatrix
		SET		FieldName = @PropertyName,
				IsRowsFixed = @IsRowsFixed,
				IsColumnsFixed = @IsColumnsFixed,
				GridLines = @GridLines
			WHERE	FieldName = @PropertyName
	END
	ELSE
	BEGIN
		DECLARE @CustomUserFieldID int 
	
		SELECT @CustomUserFieldID = CustomUserFieldID from ckbx_CustomUserField where CustomUserFieldName = @PropertyName
		INSERT INTO ckbx_CustomUserFieldMatrix(FieldName, IsRowsFixed, IsColumnsFixed, GridLines, CustomUserFieldID)
		VALUES (@PropertyName, @IsRowsFixed, @IsColumnsFixed, @GridLines, @CustomUserFieldID)

		select @@IDENTITY
	END
GO
  
ALTER PROCEDURE [dbo].[ckbx_sp_Profile_AddRadioField]
	@PropertyName nvarchar(510)
AS

SET NOCOUNT ON
	IF EXISTS (SELECT 1 FROM ckbx_CustomUserFieldRadio WHERE FieldName = @PropertyName)
		BEGIN		
			UPDATE	ckbx_CustomUserFieldRadio
			SET		FieldName = @PropertyName
			WHERE	FieldName = @PropertyName
		END
	ELSE
		BEGIN
			DECLARE @CustomUserFieldID int 

			SELECT @CustomUserFieldID = CustomUserFieldID from ckbx_CustomUserField where CustomUserFieldName = @PropertyName
			INSERT INTO ckbx_CustomUserFieldRadio(FieldName, CustomUserFieldID)
			VALUES (@PropertyName, @CustomUserFieldID)

			select @@IDENTITY
		END

GO
		
ALTER PROCEDURE [dbo].[ckbx_sp_Profile_CleanUpRadioButtonField]
 @FieldName [nvarchar] (450)
As

SET NOCOUNT ON

  BEGIN TRANSACTION; 

   DECLARE @RadioFieldId int
   SELECT @RadioFieldId = Id FROM ckbx_CustomUserFieldRadio
   WHERE FieldName = @FieldName

   DELETE FROM ckbx_CustomUserFieldRadioOption WHERE RadioButtonId = @RadioFieldId
  
  COMMIT;

GO

ALTER PROCEDURE [dbo].[ckbx_sp_Profile_AddSelectedRadioFieldOption]
	@FieldName nvarchar(450),
	@UserID uniqueidentifier,
	@SelectedOptionText nvarchar(450)
AS

SET NOCOUNT ON

DECLARE @RadioButtonId int
SELECT @RadioButtonId = Id from ckbx_CustomUserFieldRadio
WHERE FieldName = @FieldName

DECLARE @SelectedOptionId int
SELECT @SelectedOptionId = Id from ckbx_CustomUserFieldRadioOption
WHERE OptionText = @SelectedOptionText AND RadioButtonId = @RadioButtonId

IF @UserID IS NULL
BEGIN
	UPDATE ckbx_CustomUserFieldRadio
	SET DefaultSelectedOptionId = @SelectedOptionId
	WHERE ID = @RadioButtonId
END

ELSE
BEGIN
	INSERT INTO ckbx_CustomUserFieldRadioOption_User
	(SelectedOptionId, UserId)
	VALUES
	(@SelectedOptionId, @UserID)
END

GO

ALTER PROCEDURE [dbo].[ckbx_sp_Profile_GetRadioField]
	@FieldName nvarchar(450),
	@UserID uniqueidentifier
AS

SET NOCOUNT ON

DECLARE @FieldId int
SELECT @FieldId = Id from ckbx_CustomUserFieldRadio where FieldName = @FieldName

DECLARE @SelectedOptionId int
SELECT @SelectedOptionId = SelectedOptionId from ckbx_CustomUserFieldRadioOption_User cf_user
	LEFT JOIN ckbx_CustomUserFieldRadioOption cf_option on 
		cf_user.SelectedOptionId = cf_option.Id
WHERE cf_user.UserID = @UserID AND cf_option.RadioButtonId = @FieldId

IF @SelectedOptionId IS NULL
BEGIN
	SELECT @SelectedOptionId = DefaultSelectedOptionId FROM ckbx_CustomUserFieldRadio
	WHERE Id = @FieldId
END

SELECT o.Id, OptionText, CASE WHEN (@SelectedOptionId = o.Id) THEN 'true' ELSE 'false' END AS IsSelected
FROM ckbx_CustomUserFieldRadioOption o
WHERE RadioButtonId = @FieldId

GO


ALTER PROCEDURE [dbo].[ckbx_sp_CustomUserField_UpdateSelectedRadioOption]
	@OptionText nvarchar(MAX),
	@FieldName nvarchar(450),
	@UserID uniqueidentifier
AS

SET NOCOUNT ON

DECLARE @RadioButtonId int
SELECT @RadioButtonId = Id from ckbx_CustomUserFieldRadio
WHERE FieldName = @FieldName

DECLARE @SelectedOptionId int 

SELECT @SelectedOptionId = cf_user.Id FROM ckbx_CustomUserFieldRadioOption_User cf_user
	LEFT JOIN ckbx_CustomUserFieldRadioOption cf_option on 
		cf_user.SelectedOptionId = cf_option.Id
	WHERE cf_option.RadioButtonId = @RadioButtonId AND UserID = @UserID

IF @SelectedOptionId IS NULL
BEGIN
	UPDATE ckbx_CustomUserFieldRadioOption_User
	SET SelectedOptionId = (SELECT Id from ckbx_CustomUserFieldRadioOption WHERE OptionText = @OptionText AND RadioButtonId = @RadioButtonId)
	WHERE Id = @SelectedOptionId
END
ELSE
BEGIN
	INSERT INTO ckbx_CustomUserFieldRadioOption_User (SelectedOptionId, UserID)
	VALUES
	((SELECT Id from ckbx_CustomUserFieldRadioOption WHERE OptionText = @OptionText AND RadioButtonId = @RadioButtonId), @UserID)
END

GO

EXEC ckbx_sp_Profile_CreateProperty 'FirstName', 0, 0
EXEC ckbx_sp_Profile_CreateProperty 'LastName', 0, 0

GO

