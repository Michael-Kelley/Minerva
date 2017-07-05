USE [world]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[characters_items](
	[id] [int] NOT NULL,
	[item] [binary](15) NOT NULL,
	[amount] [int] NULL,
	[slot] [smallint] NOT NULL
) ON [PRIMARY]

GO