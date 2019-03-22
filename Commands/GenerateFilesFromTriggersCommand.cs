using System;
using System.IO;
using System.Linq;
using SqlFileizer.Data;

namespace SqlFileizer.Commands
{
    public class GenerateFilesFromTriggersCommand : ICommand
    {
        /// <summary>
        /// A format string for drop if exists logic. First arg should be schema, second should be proc name.
        /// </summary>
        /// <returns></returns>
        private static string _dropIfExistsSqlStringFormat = @"IF OBJECT_ID('{0}.{1}') IS NOT NULL
BEGIN
	DROP TRIGGER	[{0}].[{1}]
END
go

";
        public string CommandName => "generateTriggers";

        public string Description => "Generate sql files from triggers (you will be asked for connection string)";

        public string[] ArgDefinitions => new string[] 
        { 
            "DB Connection String (e.g. \"Server=MyServer\\MyInstance;Database=MyDB;Trusted_Connection=True;\")",
            "Subfolder to put files in (I find \"triggers\" to be good)",
            "Include drop if exists for each proc before create (y/n)?"
        };

        public void Execute(string[] args)
        {
            bool includeDropIfExists = args[2].ToUpper().Contains("Y");
            var pathToWrite = Path.Combine(Environment.CurrentDirectory, args[1]);
            Console.WriteLine($"Using path: {pathToWrite}");
            // Create the directory (if it already exists, that's fine)
            System.IO.Directory.CreateDirectory(pathToWrite);
            var triggers = SqlFileizerDbContext.GetAllTriggersFromDB(args[0]);
            foreach (var trigger in triggers)
            {
                string fullFileName = Path.Combine(pathToWrite, $"{trigger["TriggerName"].ToString()}.sql");
                Console.WriteLine($"Writing file {fullFileName}...");
                if (includeDropIfExists)
                {
                    string dropText = string.Format(_dropIfExistsSqlStringFormat, trigger["Schema"], trigger["TriggerName"]);
                    System.IO.File.WriteAllText(fullFileName, dropText + trigger["TriggerValue"].ToString());
                }
                else
                {
                    System.IO.File.WriteAllText(fullFileName, trigger["TriggerValue"].ToString());
                }
            }
            Console.WriteLine();
            Console.WriteLine($"{triggers.Count()} triggers successfully written to {pathToWrite}");
        }
    }
}