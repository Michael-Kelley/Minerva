USE [world]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[characters_equipment](
	[id] [int] IDENTITY(0,1) NOT NULL,
	[head] [binary](15) NULL,
	[body] [binary](15) NULL,
	[hands] [binary](15) NULL,
	[feet] [binary](15) NULL,
	[lefthand] [binary](15) NULL,
	[righthand] [binary](15) NULL,
	[neck] [binary](15) NULL,
	[finger1] [binary](15) NULL,
	[finger2] [binary](15) NULL,
	[leftear] [binary](15) NULL,
	[rightear] [binary](15) NULL,
	[leftwrist] [binary](15) NULL,
	[rightwrist] [binary](15) NULL,
	[back] [binary](15) NULL,
	[card] [binary](15) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Auto-incrementing unique ID.  Also the ID of the character this equipment belongs to.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'characters_equipment', @level2type=N'COLUMN',@level2name=N'id'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Helmet' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'characters_equipment', @level2type=N'COLUMN',@level2name=N'head'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Suit' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'characters_equipment', @level2type=N'COLUMN',@level2name=N'body'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Gloves' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'characters_equipment', @level2type=N'COLUMN',@level2name=N'hands'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Boots' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'characters_equipment', @level2type=N'COLUMN',@level2name=N'feet'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Left-hand Weapon' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'characters_equipment', @level2type=N'COLUMN',@level2name=N'lefthand'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Right-hand Weapon' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'characters_equipment', @level2type=N'COLUMN',@level2name=N'righthand'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Amulet' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'characters_equipment', @level2type=N'COLUMN',@level2name=N'neck'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ring 1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'characters_equipment', @level2type=N'COLUMN',@level2name=N'finger1'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ring 2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'characters_equipment', @level2type=N'COLUMN',@level2name=N'finger2'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Earring 1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'characters_equipment', @level2type=N'COLUMN',@level2name=N'leftear'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Earring 2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'characters_equipment', @level2type=N'COLUMN',@level2name=N'rightear'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Bracelet 1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'characters_equipment', @level2type=N'COLUMN',@level2name=N'leftwrist'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Bracelet 2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'characters_equipment', @level2type=N'COLUMN',@level2name=N'rightwrist'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Epaulette' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'characters_equipment', @level2type=N'COLUMN',@level2name=N'back'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Bike/Board Card' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'characters_equipment', @level2type=N'COLUMN',@level2name=N'card'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Each item value is 15 bytes long, consisting of the following: short ID, byte bonus, (byte)bool isBound, byte craftType, byte craftBonus, byte slots, byte slot1, byte slot2, byte slot3, byte slot4, unixdatetime expiry' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'characters_equipment'
GO