USE [world]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[characters_skills](
	[id] [int] NOT NULL,
	[skill] [smallint] NOT NULL,
	[level] [tinyint] NOT NULL,
	[slot] [tinyint] NOT NULL
) ON [PRIMARY]

GO