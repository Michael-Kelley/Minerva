USE [login]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[accounts](
	[id] [int] IDENTITY(0,1) NOT NULL,
	[username] [nvarchar](64) NOT NULL,
	[hash] [nchar](40) NOT NULL,
	[auth] [tinyint] NOT NULL,
	[access] [tinyint] NOT NULL,
	[email] [nvarchar](64) NOT NULL,
	[lastlogin] [datetime] NULL,
	[lastip] [nvarchar](15) NULL,
	[online] [bit] NOT NULL
) ON [PRIMARY]

GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Auto-incrementing unique ID.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'accounts', @level2type=N'COLUMN',@level2name=N'id'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The login ID associated with the account.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'accounts', @level2type=N'COLUMN',@level2name=N'username'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SHA1-encrypted password associated with the account.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'accounts', @level2type=N'COLUMN',@level2name=N'hash'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The account''s authority (0 = not verified, 1 = normal, 2 = banned).' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'accounts', @level2type=N'COLUMN',@level2name=N'auth'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The access level associated with this account.  Uses bitflags.  Refer to Minerva''s documentation for details.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'accounts', @level2type=N'COLUMN',@level2name=N'access'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The email used when creating this account.  Will be used incase of a forgotten password, or to confirm password changes.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'accounts', @level2type=N'COLUMN',@level2name=N'email'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The date this account was last logged in on.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'accounts', @level2type=N'COLUMN',@level2name=N'lastlogin'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The IP this account was last logged in from.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'accounts', @level2type=N'COLUMN',@level2name=N'lastip'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'A boolean used to show whether or not the account is currently logged in.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'accounts', @level2type=N'COLUMN',@level2name=N'online'
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Stores accounts for Minerva.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'accounts'
GO

INSERT INTO accounts (username, hash, auth, access, email, lastlogin, lastip, online) VALUES ('unverified', '9d4e1e23bd5b727046a9e3b4b7db57bd8d6ee684', 0, 0, 'yamapwns@coding.net', '06/03/2010 20:25:00', '127.0.0.1', 'False')
INSERT INTO accounts (username, hash, auth, access, email, lastlogin, lastip, online) VALUES ('normal', '9d4e1e23bd5b727046a9e3b4b7db57bd8d6ee684', 1, 0, 'yamapwns@coding.net', '06/03/2010 20:25:00', '127.0.0.1', 'False')
INSERT INTO accounts (username, hash, auth, access, email, lastlogin, lastip, online) VALUES ('banned', '9d4e1e23bd5b727046a9e3b4b7db57bd8d6ee684', 2, 0, 'yamapwns@coding.net', '06/03/2010 20:25:00', '127.0.0.1', 'False')
INSERT INTO accounts (username, hash, auth, access, email, lastlogin, lastip, online) VALUES ('online', '9d4e1e23bd5b727046a9e3b4b7db57bd8d6ee684', 1, 0, 'yamapwns@coding.net', '06/03/2010 20:25:00', '127.0.0.1', 'True')
GO