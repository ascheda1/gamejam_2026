// Terminalizer © 2025 Vindemiatrix Collective
// Website and Documentation - https://dev.vindemiatrixcollective.com

#region

using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

#endregion

namespace VindemiatrixCollective.Terminalizer
{
    public readonly struct CommandEntry
    {
        public Action<string, ArgumentParser> Callback { get; }

        public ArgumentParser Parser { get; }

        public string Command { get; }

        public string Description { get; }

        public CommandEntry(
            string command, Action<string, ArgumentParser> callback, string description = null, ArgumentParser parser = null)
        {
            Command     = command;
            Description = description;
            Callback    = callback;
            Parser      = parser;
        }
    }

    public readonly struct CommandContext
    {
        public string Arguments { get; }
        public string Command { get; }

        public CommandContext(string command, string arguments = null)
        {
            Command   = command;
            Arguments = arguments;
        }

        public override string ToString() => $"{Command}{(string.IsNullOrEmpty(Arguments) ? "" : $" {Arguments}")}";
    }

    public class TerminalModel
    {
        private readonly Dictionary<string, CommandEntry> commands = new();
        private readonly List<LineContext> outputHistory = new();
        public IEnumerable<LineContext> LinesUnwrapped => outputHistory;
        public int Lines => outputHistory.Count;

        public IReadOnlyDictionary<string, CommandEntry> Commands => commands;

        public TerminalModel(TerminalConfig config) { }

        /// <summary>
        ///     Adds an unprocessed line to the output history.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="lineClass">The USS class you want to give it.</param>
        public void AddLine(string line, string lineClass = null)
        {
            outputHistory.Add(new LineContext(line, lineClass));
        }

        /// <summary>
        ///     Clears the output history.
        /// </summary>
        public void ClearLines()
        {
            outputHistory.Clear();
        }


        /// <summary>
        ///     Registers a command.
        /// </summary>
        /// <param name="command">Name of the command.</param>
        /// <param name="callback">Reference to a function delegate.</param>
        /// <param name="description">Description of the command.</param>
        /// <param name="parser">Optional: an instance of <see cref="ArgumentParser" />, if you need advanced argument processing.</param>
        public void RegisterCommand(
            string command, Action<string, ArgumentParser> callback, string description = null, ArgumentParser parser = null)
        {
            Assert.IsFalse(string.IsNullOrEmpty(command), nameof(command));
            Assert.IsNotNull(callback, nameof(callback));
            CommandEntry cmd = new(command, callback, description, parser);
            commands.Add(cmd.Command, cmd);
        }

        /// <summary>
        ///     Unregisters a command.
        /// </summary>
        /// <param name="command">The name of the command you wish to unregister.</param>
        public void UnregisterCommand(string command)
        {
            commands.Remove(command);
        }
    }
}