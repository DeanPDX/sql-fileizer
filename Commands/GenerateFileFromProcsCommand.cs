using System;
using System.IO;
using SqlFileizer.Data;

namespace SqlFileizer.Commands
{
    public class GenerateFileFromProcsCommand : ICommand
    {
        /// <summary>
        /// A format string for drop if exists logic. First arg should be schema, second should be proc name.
        /// </summary>
        /// <returns></returns>
        private static string _dropIfExistsSqlStringFormat = @"IF OBJECT_ID('{0}.{1}') IS NOT NULL
BEGIN
	DROP PROCEDURE	[{0}].[{1}]
END
go

";
        public string CommandName => "generate";

        public char CommandShortcut => '2';

        public string Description => "Interactively generate sql files from stored procs";

        public string[] ArgDefinitions => new string[] 
        { 
            "DB Connection String (e.g. \"Server=MyServer\\MyInstance;Database=MyDB;Trusted_Connection=True;\")",
            "Subfolder to put files in (I find \"procs\" to be good)",
            "Include drop if exists for each proc before create (y/n)?"
        };

        public void Execute(string[] args)
        {
            bool includeDropIfExists = args[2].ToUpper().Contains("Y");
            var pathToWrite = Path.Combine(Environment.CurrentDirectory, args[1]);
            Console.WriteLine($"Using path: {pathToWrite}");
            // Create the directory (if it already exists, that's fine)
            System.IO.Directory.CreateDirectory(pathToWrite);
            var procs = SqlFileizerDbContext.GetAllStoredProcsFromDB(args[0]);
            foreach (var proc in procs)
            {
                string fullFileName = Path.Combine(pathToWrite, $"{proc["ProcName"].ToString()}.sql");
                Console.WriteLine($"Writing file {fullFileName}...");
                if (includeDropIfExists)
                {
                    string dropText = string.Format(_dropIfExistsSqlStringFormat, proc["Schema"], proc["ProcName"]);
                    System.IO.File.WriteAllText(fullFileName, dropText + proc["ProcValue"].ToString());
                }
                else
                {
                    System.IO.File.WriteAllText(fullFileName, proc["ProcValue"].ToString());
                }
                    
            }
            Console.WriteLine();
            Console.WriteLine("Done!");
        }
    }
}