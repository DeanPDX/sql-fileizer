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

Welcome to the SQL Fileizer! Press ""q"" to quit.
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
            int i = 1;
            availableCommands.ForEach(cmd => _availableCommands.Add(i++.ToString()[0], cmd));
        }

        /// <summary>
        /// Display a friendly welcome message to the user and start up the interactive UI.
        /// </summary>
        public void Start()
        {
            Console.Write(_greetingMessage);
            DisplayRepl();
        }

        /// <summary>
        /// Start our read–eval–print loop (REPL).
        /// </summary>
        private void DisplayRepl()
        {
            DisplayAvailableCommands();
            while (true)
            {
                var key = Console.ReadKey(true);
                var pressedCharacter = key.KeyChar;
                if (pressedCharacter == 'q') { return; }
                else if (_availableCommands.ContainsKey(pressedCharacter))
                {
                    var commandToExecute = _availableCommands[pressedCharacter];
                    var args = PromptUserForArgs(commandToExecute.ArgDefinitions);
                    commandToExecute.Execute(args);
                    DisplayAvailableCommands();
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
            foreach (char key in _availableCommands.Keys)
            {
                var command = _availableCommands[key];
                Console.WriteLine($"{key}) {command.Description}");
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