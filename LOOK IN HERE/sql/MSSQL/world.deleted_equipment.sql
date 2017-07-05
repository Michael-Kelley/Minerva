USE [world]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[deleted_equipment](
	[id] [int] IDENTITY(0,1) NOT NULL,
	[head] [varbinary](15) NULL,
	[body] [varbinary](15) NULL,
	[hands] [varbinary](15) NULL,
	[feet] [varbinary](15) NULL,
	[lefthand] [varbinary](15) NULL,
	[righthand] [varbinary](15) NULL,
	[neck] [varbinary](15) NULL,
	[finger1] [varbinary](15) NULL,
	[finger2] [varbinary](15) NULL,
	[leftear] [varbinary](15) NULL,
	[rightear] [varbinary](15) NULL,
	[leftwrist] [varbinary](15) NULL,
	[rightwrist] [varbinary](15) NULL,
	[back] [varbinary](15) NULL,
	[card] [varbinary](15) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Auto-incrementing unique ID.  Also the ID of the character this equipment belongs to.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'deleted_equipment', @level2type=N'COLUMN',@level2name=N'id'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Helmet' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'deleted_equipment', @level2type=N'COLUMN',@level2name=N'head'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Suit' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'deleted_equipment', @level2type=N'COLUMN',@level2name=N'body'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Gloves' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'deleted_equipment', @level2type=N'COLUMN',@level2name=N'hands'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Boots' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'deleted_equipment', @level2type=N'COLUMN',@level2name=N'feet'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Left-hand Weapon' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'deleted_equipment', @level2type=N'COLUMN',@level2name=N'lefthand'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Right-hand Weapon' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'deleted_equipment', @level2type=N'COLUMN',@level2name=N'righthand'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Amulet' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'deleted_equipment', @level2type=N'COLUMN',@level2name=N'neck'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ring 1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'deleted_equipment', @level2type=N'COLUMN',@level2name=N'finger1'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ring 2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'deleted_equipment', @level2type=N'COLUMN',@level2name=N'finger2'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Earring 1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'deleted_equipment', @level2type=N'COLUMN',@level2name=N'leftear'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Earring 2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'deleted_equipment', @level2type=N'COLUMN',@level2name=N'rightear'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Bracelet 1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'deleted_equipment', @level2type=N'COLUMN',@level2name=N'leftwrist'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Bracelet 2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'deleted_equipment', @level2type=N'COLUMN',@level2name=N'rightwrist'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Epaulette' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'deleted_equipment', @level2type=N'COLUMN',@level2name=N'back'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Bike/Board Card' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'deleted_equipment', @level2type=N'COLUMN',@level2name=N'card'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Each item value is 15 bytes long, consisting of the following: short ID, byte bonus, (byte)bool isBound, byte craftType, byte craftBonus, byte slots, byte slot1, byte slot2, byte slot3, byte slot4, unixdatetime expiry' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'deleted_equipment'
GO