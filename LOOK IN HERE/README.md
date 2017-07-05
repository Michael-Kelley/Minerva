## Set-up
1. Create 2 MSSQL DB's called "login" and "world".
2. Run the files inside /sql/ to create the MSSQL tables you need or run the deploy.bat script.
3. Compile Minerva.
4. Copy /conf/, /scripts/, and /data/ into the same directory as Minerva.exe.
5. Edit the config files you copied as needed.  Make sure to read the comments.
6. Unpack the cabal.enc from your client and copy it to /data/cabal.xml.
7. Remove everything before the \<cabal\> tag in /data/cabal.xml.  I will eventually write a program to automate this.		  
8. Set your CABAL's internal.txt to point to 127.0.0.1:38101 for login.
9. Either login with one of the existing accounts (unverified, banned, online, normal {password = "pass" for all accounts}), or create your own.

## Notes
  * Inside /tools/, there's the DumpSQL VS project.  This "dumps" an SQL table and all its data into an SQL query file.  You won't need to use this for now.  There is also a packet logger to help when analysing packets. *(MOVED TO OWN REPO. LOOK FOR 'Ostara')*
  * The OSTTemplate.bt file is a template for reading the OST files output by Ostara when saving logged packets.  
  * Inside /web/, there's an HTML file and a script for generating an SHA-1 hash from a password.  Maybe chumpy will write a proper account-creation script for us ;P  <3 Goo