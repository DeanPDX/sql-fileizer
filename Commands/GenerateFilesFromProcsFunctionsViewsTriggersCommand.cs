using System;
using System.IO;
using System.Linq;
using SqlFileizer.Data;

namespace SqlFileizer.Commands
{
    public class GenerateFilesFromProcsFunctionsViewsTriggersCommand : ICommand
    {        
        public string CommandName => "generateProcsFunctionsViewsTriggers";

        public string Description => "Generate sql files from stored procs, functions, views and triggers (you will be asked for connection string)";

        public string[] ArgDefinitions => new string[] 
        { 
            "DB Connection String (e.g. \"Server=MyServer\\MyInstance;Database=MyDB;Trusted_Connection=True;\")",
            "Include drop if exists for each proc before create (y/n)?"
        };

        public void Execute(string[] args)
        {
            bool includeDropIfExists = args[1].ToUpper().Contains("Y");
            // The arg definitions for child commands are all 0: Connection String, 1: Subfolder, 2: Drop if exists.
            // Passing args through where appropriate and changing subfolder based on command.
            var procs = new GenerateFilesFromProcsCommand();
            procs.Execute(new string[] { args[0], "Procs", args[1] });

            var functions = new GenerateFilesFromFunctionsCommand();
            functions.Execute(new string[] { args[0], "Functions", args[1] });

            var views = new GenerateFilesFromViewsCommand();
            views.Execute(new string[] { args[0], "Views", args[1] });

            var triggers = new GenerateFilesFromTriggersCommand();
            triggers.Execute(new string[] { args[0], "Triggers", args[1] });

            Console.WriteLine();
            string currentDirectory = Environment.CurrentDirectory;
            Console.WriteLine("Wrote all procs, functions, views, and triggers to:");
            Console.WriteLine($"- {currentDirectory}/Procs");
            Console.WriteLine($"- {currentDirectory}/Functions");
            Console.WriteLine($"- {currentDirectory}/Views");
            Console.WriteLine($"- {currentDirectory}/Triggers");
        }
    }
}