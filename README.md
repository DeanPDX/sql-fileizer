# sql-fileizer
This is a tool mainly for internal use to avoid repetitive SQL tasks. Currently only supporting `MSSQL`.

## Getting Started
To get started, clone the repo and run it. To do that, run the following commands:

```
git clone https://github.com/DeanPDX/sql-fileizer.git
cd sql-fileizer
dotnet restore
dotnet run
```
Once you run it, you should be greeted with this:
```
  _____  ____  _        ______ _ _      _              
 / ____|/ __ \| |      |  ____(_) |    (_)             
| (___ | |  | | |      | |__   _| | ___ _ _______ _ __ 
 \___ \| |  | | |      |  __| | | |/ _ \ |_  / _ \ '__|
 ____) | |__| | |____  | |    | | |  __/ |/ /  __/ |   
|_____/ \___\_\______| |_|    |_|_|\___|_/___\___|_|   


Welcome to the SQL Fileizer!

Available commands:

1) Display help text
2) Generate sql files from stored procs
```
## Commands
### Display help text
There isn't much here yet. I am planning on adding non-interactive support at which point this command will become a lot more useful.

### Generate sql files from stored procs
It happens semi-frequently that I have to update/maintain old applications along with their databases. As part of our DB build process, we like to drop/create all procs. `sql-fileizer` allows you to quickly create one file per proc and add `drop if exists` logic to them. This can help facilitate the DB build step described.

Select `Generate sql files from stored procs`. Provide a connection string, subfolder, and indicate whether or not you want to add `drop if exists`. You will end up with one file per proc in `<current-directory>\<subfolder-name>\`.

If you opted to add `drop if exists`, this is what it will look like in each file:

```sql
IF OBJECT_ID('{0}.{1}') IS NOT NULL
BEGIN
	DROP PROCEDURE	[{0}].[{1}]
END
go
```
Where `{0}` is schema name and `{1}` is table name.
