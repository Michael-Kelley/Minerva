@ECHO off
COLOR 0C
ECHO MSSQL deployment script
ECHO Please make sure that the [login] and [world] databases exist.
ECHO Press any key to continue or hit CTRL + C to cancel.
PAUSE
COLOR 0A
REM This is a very silly deployment script.
REM It connects via named pipes to the default instance (not named),
REM authenticates via the user account used to spawn the process.
REM It then batch runs the .sql scripts in the proper order.
REM You might need to modify it in order to get it running. Check
REM http://msdn.microsoft.com/en-us/library/ms165702.aspx
REM for the sqlcmd documentation

ECHO login.accounts.sql
sqlcmd -S (local) -i login.accounts.sql
ECHO login.sp.account_create.sql
sqlcmd -S (local) -i login.sp.account_create.sql

REM Now execute those scripts, like a baws :3
ECHO world.characters.sql
sqlcmd -S (local) -i world.characters.sql
ECHO world.characters_equipment.sql
sqlcmd -S (local) -i world.characters_equipment.sql
ECHO world.characters_items.sql
sqlcmd -S (local) -i world.characters_items.sql
ECHO world.characters_quickslots.sql
sqlcmd -S (local) -i world.characters_quickslots.sql
ECHO world.characters_skills.sql
sqlcmd -S (local) -i world.characters_skills.sql
ECHO world.characters_stats.sql
sqlcmd -S (local) -i world.characters_stats.sql
ECHO world.deleted_characters.sql
sqlcmd -S (local) -i world.deleted_characters.sql
ECHO world.deleted_equipment.sql
sqlcmd -S (local) -i world.deleted_equipment.sql
PAUSE
CLS