using System;

namespace Contest.Core.Exceptions
{
    public class CommandException : Exception
    {
        public string Command { get; set; }

        public CommandException(string command, string message)
            : base(message)
        {
            Command = command;
        }
    }
}