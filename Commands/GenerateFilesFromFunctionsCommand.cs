using System;
using System.IO;
using System.Linq;
using SqlFileizer.Data;

namespace SqlFileizer.Commands
{
    public class GenerateFilesFromFunctionsCommand : ICommand
    {
        /// <summary>
        /// A format string for drop if exists logic. First arg should be schema, second should be proc name.
        /// </summary>
        /// <returns></returns>
        private static string _dropIfExistsSqlStringFormat = @"IF OBJECT_ID('{0}.{1}') IS NOT NULL
BEGIN
	DROP FUNCTION	[{0}].[{1}]
END
go

";
        public string CommandName => "generateFunctions";

        public string Description => "Generate sql files from functions (you will be asked for connection string)";

        public string[] ArgDefinitions => new string[] 
        { 
            "DB Connection String (e.g. \"Server=MyServer\\MyInstance;Database=MyDB;Trusted_Connection=True;\")",
            "Subfolder to put files in (I find \"functions\" to be good)",
            "Include drop if exists for each proc before create (y/n)?"
        };

        public void Execute(string[] args)
        {
            bool includeDropIfExists = args[2].ToUpper().Contains("Y");
            var pathToWrite = Path.Combine(Environment.CurrentDirectory, args[1]);
            Console.WriteLine($"Using path: {pathToWrite}");
            // Create the directory (if it already exists, that's fine)
            System.IO.Directory.CreateDirectory(pathToWrite);
            var functions = SqlFileizerDbContext.GetAllFunctionsFromDB(args[0]);
            foreach (var function in functions)
            {
                string fullFileName = Path.Combine(pathToWrite, $"{function["FunctionName"].ToString()}.sql");
                Console.WriteLine($"Writing file {fullFileName}...");
                if (includeDropIfExists)
                {
                    string dropText = string.Format(_dropIfExistsSqlStringFormat, function["Schema"], function["FunctionName"]);
                    System.IO.File.WriteAllText(fullFileName, dropText + function["FunctionValue"].ToString());
                }
                else
                {
                    System.IO.File.WriteAllText(fullFileName, function["FunctionValue"].ToString());
                }
            }
            Console.WriteLine();
            Console.WriteLine($"{functions.Count()} functions successfully written to {pathToWrite}");
        }
    }
}