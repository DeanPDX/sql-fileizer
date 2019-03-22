using System;
using System.IO;
using System.Linq;
using SqlFileizer.Data;

namespace SqlFileizer.Commands
{
    public class GenerateFilesFromViewsCommand : ICommand
    {
        /// <summary>
        /// A format string for drop if exists logic. First arg should be schema, second should be proc name.
        /// </summary>
        /// <returns></returns>
        private static string _dropIfExistsSqlStringFormat = @"IF OBJECT_ID('{0}.{1}') IS NOT NULL
BEGIN
	DROP VIEW	[{0}].[{1}]
END
go

";
        public string CommandName => "generateViews";

        public string Description => "Generate sql files from views (you will be asked for connection string)";

        public string[] ArgDefinitions => new string[] 
        { 
            "DB Connection String (e.g. \"Server=MyServer\\MyInstance;Database=MyDB;Trusted_Connection=True;\")",
            "Subfolder to put files in (I find \"views\" to be good)",
            "Include drop if exists for each proc before create (y/n)?"
        };

        public void Execute(string[] args)
        {
            bool includeDropIfExists = args[2].ToUpper().Contains("Y");
            var pathToWrite = Path.Combine(Environment.CurrentDirectory, args[1]);
            Console.WriteLine($"Using path: {pathToWrite}");
            // Create the directory (if it already exists, that's fine)
            System.IO.Directory.CreateDirectory(pathToWrite);
            var views = SqlFileizerDbContext.GetAllViewsFromDB(args[0]);
            foreach (var view in views)
            {
                string fullFileName = Path.Combine(pathToWrite, $"{view["ViewName"].ToString()}.sql");
                Console.WriteLine($"Writing file {fullFileName}...");
                if (includeDropIfExists)
                {
                    string dropText = string.Format(_dropIfExistsSqlStringFormat, view["Schema"], view["ViewName"]);
                    System.IO.File.WriteAllText(fullFileName, dropText + view["ViewValue"].ToString());
                }
                else
                {
                    System.IO.File.WriteAllText(fullFileName, view["ViewValue"].ToString());
                }
            }
            Console.WriteLine();
            Console.WriteLine($"{views.Count()} views successfully written to {pathToWrite}");
        }
    }
}