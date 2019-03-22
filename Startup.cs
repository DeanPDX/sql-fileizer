using System;
using System.Collections.Generic;
using SqlFileizer.Commands;

namespace SqlFileizer
{
    class Startup
    {
        public static void Start(string[] args)
        {
            List<ICommand> commands = new List<ICommand>();
            commands.Add(new HelpCommand());
            commands.Add(new GenerateSampleConfigCommand());
            commands.Add(new GenerateFilesFromConfigCommand());
            commands.Add(new GenerateFilesFromProcsCommand());
            commands.Add(new GenerateFilesFromViewsCommand());
            commands.Add(new GenerateFilesFromFunctionsCommand());
            commands.Add(new GenerateFilesFromTriggersCommand());
            commands.Add(new GenerateFilesFromProcsFunctionsViewsTriggersCommand());

            // TODO: Support non-interactive mode (where user just supplies args and we do something)
            // For now, just showing interactive user interface all the time.
            var ui = new InteractiveUserInterface(commands);
            ui.Start();
        }
    }
}
