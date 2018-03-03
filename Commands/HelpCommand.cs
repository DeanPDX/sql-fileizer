using System;

namespace SqlFileizer.Commands
{
    public class HelpCommand : ICommand
    {
        private const string _helpText = 
@"
This application was developed by Dean Davidson (https://github.com/DeanPDX) mostly for internal app development use. The idea is to automate repetetive tasks involving, for example, getting all proc info and writing it to files for a DB build project. If you are using it and require help, submit an issue on GitHub.

Press 'q' to quit.
";
        public string CommandName => "help";

        public char CommandShortcut => '1';

        public string Description => "Display help text";

        public string[] ArgDefinitions => new string[0];

        public void Execute(string[] args)
        {
            Console.Write(_helpText);
        }
    }
}