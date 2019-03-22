# sql-fileizer
This is a tool mainly for internal use to avoid repetitive SQL tasks. Currently only supporting `MSSQL`.

## Getting Started
First, make sure you have .NET Core and Git installed. Type `dotnet --version` and `git --version` if you are unsure about your current environment. Assuming you have both tools installed, next clone the repo and run it. To do that, execute the following commands in your terminal:

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

Welcome to the SQL Fileizer! Press "q" to quit.

Available commands:

1) Display help text
2) Generate sample config file to be used for option 3
3) Generate sql files from results of SQL query specified in config file
4) Generate sql files from stored procs (you will be asked for connection string)
5) Generate sql files from views (you will be asked for connection string)
6) Generate sql files from functions (you will be asked for connection string)
7) Generate sql files from triggers (you will be asked for connection string)
8) Generate sql files from stored procs, functions, views and triggers (you will be asked for connection string)
```
Read on for more information on the various commands.
## Commands
### Display help text
There isn't much here yet. I am planning on adding non-interactive support at which point this command will become a lot more useful.

### Interactively generate sql files from stored procs
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

### Generate sample config file to be used for step 4
The app will create a config file named `config.xml` for you to customize. The default config file will [generate scripts from stored procedures](SampleConfigs/ScriptsFromProcs.xml). See [Sample Configs](SampleConfigs) for more information and additional config files to get you started.

### Generate sql files from results of SQL query specified in config file
You will be asked for the name of your config file, then the app will generate files based on the SQL specified in the config file.