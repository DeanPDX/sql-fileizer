using System;

namespace SqlFileizer.Commands
{
    public interface ICommand 
    {
        string CommandName { get; }
        char CommandShortcut { get; }
        string Description { get; }
        string[] ArgDefinitions { get; }
        void Execute(string[] args);
    }
}