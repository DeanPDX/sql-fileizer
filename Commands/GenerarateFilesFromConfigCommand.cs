using SqlFileizer.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace SqlFileizer.Commands
{
    public class GenerateFilesFromConfigCommand: ICommand
    {
        public string CommandName => "filesFromConfig";

        public char CommandShortcut => '4';

        public string Description => "Generate sql files from results of SQL query specified in config file";

        public string[] ArgDefinitions => new string[] 
        { 
            "Config file name (path not required if config file is in current directory)"
        };

        public void Execute(string[] args)
        {
            string step = "";
            int rowNumber = 0;

            try
            {
                string configFile = args[0];
                
                step = "Load config file";
                string config = File.ReadAllText(configFile);

                step = "Convert config file to XML";
                var configXml = new XmlDocument();
                configXml.LoadXml(config);

                step = "Get output directory";
                string outputDirectory = GetTextByTagName(configXml, "OutputDirectory");

                step = "Get connection string";
                string connectionString = GetTextByTagName(configXml, "ConnectionString");

                step = "Get SQL query";
                string sqlQuery = GetTextByTagName(configXml, "SqlQuery");

                step = "Create output directory if not exists";
                Directory.CreateDirectory(outputDirectory);

                step = "Execute SQl query";
                //output message to make it clear we're doing something since it takes awhile for the connection to time out if it's a bad connection string
                Console.WriteLine("Connecting to database with connection string: " + connectionString);
                Console.WriteLine();
                var data = SqlFileizerDbContext.GetData(connectionString, sqlQuery).ToList();

                string[] expectedColumns = { "FileName", "FileExtension", "BodyText", "HeaderText", "FooterText" };
                string[] dataColumns = data[0].Keys.ToArray();
                if (!dataColumns.OrderBy(x => x).SequenceEqual(dataColumns.OrderBy(y => y))) // verify that array elements match, order doesn't matter
                {
                    string message = String.Format(@"Data columns don't match expected columns.{0}Data columns: {1}{0}Expected columns: {2}{0}",
                        Environment.NewLine, String.Join(",", expectedColumns), String.Join(", ", dataColumns));

                    throw new Exception(message);
                }

                step = "Write files to output folder";
                for (int i = 0; i < data.Count; i++)
                {
                    rowNumber = i + 1; //rowNumber starts at 1 instead of 0
                    var row = data[i];

                    string fileNameWithPath = Path.Combine(outputDirectory, row["FileName"] + "." + row["FileExtension"]);

                    var fileText = new StringBuilder();
                    string header = row["HeaderText"].ToString().Trim();
                    string content = row["FileContent"].ToString().Trim();
                    string footer = row["FooterText"].ToString().Trim();

                    if (header.Length > 0)
                    {
                        fileText.AppendLine(header);
                        fileText.AppendLine("go");
                    }

                    if (content.Length == 0) { throw new Exception("No content found for data row " + rowNumber.ToString());  }
                    fileText.AppendLine(content);

                    if (footer.Length > 0)
                    {
                        fileText.AppendLine("go");
                        fileText.AppendLine(footer);
                    }

                    File.WriteAllText(fileNameWithPath, fileText.ToString());
                }

                Console.WriteLine("Files successfully written to " + outputDirectory);
            }
            catch (Exception ex)
            {
                string message = "Error on following step: " + step;
                if (rowNumber > 0) { message += "  row number: " + rowNumber.ToString(); }
                message += "  Error message: " + ex.Message;

                Console.WriteLine(message);
            }
        }

        private string GetTextByTagName (XmlDocument xmlDoc, string tagName)
        {
            XmlElement element = (XmlElement)xmlDoc.GetElementsByTagName(tagName)[0];

            if (element == null)
            {
                throw (new Exception(tagName + " not found in config file"));
            }

            return element.InnerText;
        }
    }
}