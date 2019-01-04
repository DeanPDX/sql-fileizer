using System;
using System.IO;
using SqlFileizer.Data;

namespace SqlFileizer.Commands
{
    public class GenerateSampleConfigCommand : ICommand
    {
          /// <summary>
        /// Sample config file; default to scripting out all procs in a given database
        /// </summary>
        /// <returns></returns>
        private static string _sampleConfigText = @"
<config>
    <OutputDirectory>procs</OutputDirectory>
    <ConnectionString>Server=ServerName;Database=DatabaseName;Trusted_Connection=True;</ConnectionString>

    <!-- Result set is parsed based on column names, not order -->
    <!-- FileName: name of file without extension -->
    <!-- FileExtension: do not include the dot -->
    <!-- BodyText: only handles text output for now (no binary support) -->
    <!-- HeaderText: to be appended to the beginning of each file; separated from body by ""go"" directive -->
    <!-- FooterText: to be appended to the end of each file; separated from body by ""go"" directive -->
    <SqlQuery>
        select o.name as FileName,
	        'sql' as FileExtension,
	        definition as BodyText,
	        'if object_id(''' + o.name + ''') is not null drop procedure ' + o.name as HeaderText,
	        '' as FooterText
        from sys.sql_modules m
	        join sys.objects o on m.object_id = o.object_id
        where o.type_desc = 'SQL_STORED_PROCEDURE'
    </SqlQuery>
</config>
";
        public string CommandName => "config";

        public char CommandShortcut => '3';

        public string Description => "Generate sample config file to be used for step 4";

        public string[] ArgDefinitions => new string[0];

        public void Execute(string[] args)
        {
            string fileName = "config.xml";
            string currentDirectory = Environment.CurrentDirectory;

            Console.WriteLine($"Creating {fileName} in {currentDirectory}");

            File.WriteAllText(Path.Combine(currentDirectory, fileName), _sampleConfigText);

            Console.WriteLine();
            Console.WriteLine($"{fileName} created in {Environment.CurrentDirectory}");
        }
    }
}