using System;

namespace SqlFileizer.Commands
{
    public interface ICommand 
    {
        string CommandName { get; }
        string Description { get; }
        string[] ArgDefinitions { get; }
        void Execute(string[] args);
    }
}