using System;
using System.IO;
using SqlFileizer.Data;

namespace SqlFileizer.Commands
{
    public class GenerateSampleConfigCommand : ICommand
    {
        /// <summary>
        /// The folder for our sample config files.
        /// </summary>
        private static string _sampleConfigFolder = "SampleConfigs";

        /// <summary>
        /// Sample config file; default to scripting out all procs in a given database
        /// </summary>
        /// <returns></returns>
        private static string _sampleConfigFile = "ScriptsFromProcs.xml";

        public string CommandName => "config";

        public string Description => "Generate sample config file to be used for option 3";

        public string[] ArgDefinitions => new string[0];

        public void Execute(string[] args)
        {
            string fileName = "config.xml";
            string currentDirectory = Environment.CurrentDirectory;

            string configText = File.ReadAllText(Path.Combine(currentDirectory, _sampleConfigFolder, _sampleConfigFile));

            Console.WriteLine($"Creating {fileName} in {currentDirectory}");

            File.WriteAllText(Path.Combine(currentDirectory, fileName), configText);

            Console.WriteLine();
            Console.WriteLine($"{fileName} created in {currentDirectory}");
        }
    }
}