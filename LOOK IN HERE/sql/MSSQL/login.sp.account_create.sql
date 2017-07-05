USE [login]
GO

/*
Name:  account_create
Description:  creates all data for a new account.
Authors:  Yamachi, Alphakilo
*/

CREATE PROCEDURE [dbo].[account_create]

@username nvarchar(50),
@hash nchar(40),
@email nvarchar(64),

@result int OUTPUT,
@account_id int OUTPUT

AS

/* Check if the requested username has already been used */
IF EXISTS (SELECT id FROM accounts WHERE username=@username)
BEGIN
SELECT @result = 1		/* Account name already taken */
RETURN
END

/* Check if the email has already been used */
IF EXISTS (SELECT id FROM accounts WHERE email=@email)
BEGIN
SELECT @result = 2		/* Email already used */
RETURN
END

BEGIN TRANSACTION CreateAccount
	WITH MARK N'Delete character invokes.';
	
	INSERT INTO accounts (username, hash, auth, access, email, lastlogin, lastip, online)
		VALUES (@username, @hash, 1, 0, @email, null, null, 'False')
		
		SELECT @result = 3							/* Account creation succeeded */
		SELECT @account_id = SCOPE_IDENTITY()		/* ID of the last inserted row */
		
COMMIT TRANSACTION CreateAccount
RETURN
	
GO