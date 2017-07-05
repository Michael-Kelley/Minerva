USE [world]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[characters_stats](
	[id] [int] IDENTITY(0,1) NOT NULL,
	[curhp] [smallint] NOT NULL,
	[maxhp] [smallint] NOT NULL,
	[curmp] [smallint] NOT NULL,
	[maxmp] [smallint] NOT NULL,
	[cursp] [smallint] NOT NULL,
	[maxsp] [smallint] NOT NULL,
	[xp] [bigint] NOT NULL,
	[str] [int] NOT NULL,
	[int] [int] NOT NULL,
	[dex] [int] NOT NULL,
	[honour] [int] NOT NULL,
	[rank] [int] NOT NULL,
	[swordrank] [tinyint] NOT NULL,
	[swordxp] [int] NOT NULL,
	[swordpoints] [int] NOT NULL,
	[magicrank] [tinyint] NOT NULL,
	[magicxp] [int] NOT NULL,
	[magicpoints] [int] NOT NULL,
	[alz] [bigint] NOT NULL
) ON [PRIMARY]

GO