USE [world]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[characters](
	[id] [int] IDENTITY(0,1) NOT NULL,
	[account] [int] NOT NULL,
	[slot] [tinyint] NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[lv] [int] NOT NULL,
	[class] [tinyint] NOT NULL,
	[gender] [bit] NOT NULL,
	[face] [tinyint] NOT NULL,
	[hair] [tinyint] NOT NULL,
	[colour] [tinyint] NOT NULL,
	[map] [tinyint] NOT NULL,
	[x] [tinyint] NOT NULL,
	[y] [tinyint] NOT NULL,
	[created] [datetime] NOT NULL
) ON [PRIMARY]

GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Auto-incrementing unique ID.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'characters', @level2type=N'COLUMN',@level2name=N'id'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID of the account this character belongs to.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'characters', @level2type=N'COLUMN',@level2name=N'account'
GO