using System;
using System.Collections.Generic;
using SqlFileizer.Commands;

namespace SqlFileizer
{
    class InteractiveUserInterface
    {
        private static string _greetingMessage = @"
  _____  ____  _        ______ _ _      _              
 / ____|/ __ \| |      |  ____(_) |    (_)             
| (___ | |  | | |      | |__   _| | ___ _ _______ _ __ 
 \___ \| |  | | |      |  __| | | |/ _ \ |_  / _ \ '__|
 ____) | |__| | |____  | |    | | |  __/ |/ /  __/ |   
|_____/ \___\_\______| |_|    |_|_|\___|_/___\___|_|   

Welcome to the SQL Fileizer!
";
        /// <summary>
        /// We are storing our commands in a dictionary for fast indexing later on.
        /// </summary>
        private readonly Dictionary<char, ICommand> _availableCommands;

        /// <summary>
        /// A class for showing messages to the user
        /// </summary>
        /// <param name="availableCommands">Commands available to the user.</param>
        public InteractiveUserInterface(List<ICommand> availableCommands)
        {
            _availableCommands = new Dictionary<char, ICommand>();
            availableCommands.ForEach(cmd => _availableCommands.Add(cmd.CommandShortcut, cmd));
        }

        /// <summary>
        /// Display a friendly welcome message to the user and start up the interactive UI.
        /// </summary>
        public void Start()
        {
            Console.Write(_greetingMessage);
            DisplayRepl();
        }

        private void DisplayRepl()
        {
            while (true)
            {
                DisplayAvailableCommands();
                var key = Console.ReadKey(true);
                var pressedCharacter = key.KeyChar;
                if (pressedCharacter == 'q') { return; }
                else if (_availableCommands.ContainsKey(pressedCharacter))
                {
                    var commandToExecute = _availableCommands[pressedCharacter];

                    var args = PromptUserForArgs(commandToExecute.ArgDefinitions);
                    commandToExecute.Execute(args);

                }
                else
                {
                    Console.WriteLine($"Well, this is awkward. That was not found.");
                }
            }
        }

        /// <summary>
        /// Display a list of available commands
        /// </summary>
        private void DisplayAvailableCommands()
        {
            // We always want a line above the commands
            Console.WriteLine();
            Console.WriteLine("Available commands:");
            Console.WriteLine();
            foreach (ICommand command in _availableCommands.Values)
            {
                Console.WriteLine($"{command.CommandShortcut}) {command.Description}");
            }
        }

        /// <summary>
        /// Prompt the user for args and return the values they submitted.
        /// </summary>
        /// <param name="argDefinitions">Definiotions for the args to display to the users.</param>
        private string[] PromptUserForArgs(string[] argDefinitions)
        {
            string[] args = new string[argDefinitions.Length];
            for (int i = 0; i < argDefinitions.Length; i++)
            {
                while (true)
                {
                    Console.Write($"{argDefinitions[i]}: ");
                    var userInput = Console.ReadLine();
                    if (userInput.Length > 0) 
                    {
                        args[i] = userInput;
                        break;
                    }
                }
            }
            Console.WriteLine();
            return args;
        }
    }
}