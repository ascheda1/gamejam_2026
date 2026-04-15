// Terminalizer © 2025 Vindemiatrix Collective
// Website and Documentation - https://dev.vindemiatrixcollective.com

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

#endregion

namespace VindemiatrixCollective.Terminalizer
{
    public class TerminalController
    {
        private readonly int commandHistorySize;
        private readonly List<CommandContext> commandHistory;
        private readonly TerminalConfig config;
        private readonly TerminalModel model;

        private readonly TerminalView view;
        private bool isSuggestionVisible;
        private int commandHistoryIndex;

        [SerializeField] private string inputSoFar;

        public TerminalController(TerminalModel model, TerminalView view, TerminalConfig config)
        {
            this.config = config;
            this.model  = model;
            this.view   = view;
            if (config.CommandHistorySize > 0)
            {
                commandHistorySize = config.CommandHistorySize;
                commandHistory     = new List<CommandContext>(commandHistorySize);
            }
            else
            {
                commandHistory = new List<CommandContext>();
            }

            view.BindKeyUpProcessor(KeyUp);
            view.BindKeyDownEvent(KeyDown);
            view.BindTextInputEvent(InputEvent);

            model.RegisterCommand("echo", Echo, "Displays any input passed as a parameter.");
            model.RegisterCommand("help", Help, "Provides help information for Terminal commands.");
            model.RegisterCommand("test", Test, "Prints lorem ipsum.");
            model.RegisterCommand("clear", Clear, "Clears the screen.");
        }

        /// <summary>
        ///     Implements the Clear function, clears the screen.
        /// </summary>
        /// <param name="text">Raw argument string.</param>
        /// <param name="parser">Reference to an <see cref="ArgumentParser">ArgumentParser</see> instance.</param>
        public void Clear(string text, ArgumentParser parser)
        {
            model.ClearLines();
            view.Clear();
        }

        /// <summary>
        ///     Displays the given argument string in the terminal.
        /// </summary>
        /// <param name="text">Raw argument string.</param>
        /// <param name="parser">Reference to an <see cref="ArgumentParser">ArgumentParser</see> instance.</param>
        public void Echo(string text, ArgumentParser parser = null)
        {
            SendToOutput(text, string.Empty);
        }

        /// <summary>
        ///     Displays the given argument using the 'text-danger' USS class.
        /// </summary>
        /// <param name="text">Raw argument string.</param>
        /// <param name="parser">Reference to an <see cref="ArgumentParser">ArgumentParser</see> instance.</param>
        public void Error(string text, ArgumentParser parser)
        {
            SendToOutput(text, "text-danger");
        }

        /// <summary>
        ///     Displays a list of the currently registered commands.
        /// </summary>
        /// <param name="text">Raw argument string.</param>
        /// <param name="parser">Reference to an <see cref="ArgumentParser" /> instance.</param>
        public void Help(string text, ArgumentParser parser)
        {
            StringBuilder sb = new();
            sb.AppendLine("Available commands:");
            foreach (KeyValuePair<string, CommandEntry> kvp in model.Commands)
            {
                CommandEntry cmd         = kvp.Value;
                string       description = cmd.Description ?? "No description available.";
                sb.AppendLine($"   {kvp.Key.Info()}: {description}");
            }

            SendToOutput(sb.ToString());
        }

        private void ExecuteCommand(CommandContext context)
        {
            CommandEntry                   commandEntry = model.Commands[context.Command];
            Action<string, ArgumentParser> cmd          = commandEntry.Callback;
            if (commandEntry.Parser != null)
            {
                if (!commandEntry.Parser.ParseArguments(context.Arguments))
                {
                    Error(commandEntry.Parser.ErrorMessage, commandEntry.Parser);
                    return;
                }
            }

            cmd(context.Arguments, commandEntry.Parser);
        }

        private void HistoryGoBack(int steps)
        {
            if (commandHistoryIndex + steps > commandHistory.Count)
            {
                return;
            }

            commandHistoryIndex++;
            CommandContext context      = commandHistory[^commandHistoryIndex];
            inputSoFar = view.TextInput = context.ToString();
            view.SetCursorAtEndline();
            SuggestClosestCommand(context.ToString());
        }

        private void HistoryGoForward(int steps)
        {
            if (commandHistoryIndex - steps <= 0)
            {
                commandHistoryIndex = 0;
                inputSoFar          = view.TextInput = string.Empty;
                return;
            }

            commandHistoryIndex--;

            CommandContext context      = commandHistory[^commandHistoryIndex];
            inputSoFar = view.TextInput = context.ToString();
            view.SetCursorAtEndline();
            SuggestClosestCommand(context.ToString());
        }

        private void InputEvent(InputEvent evt)
        {
            SuggestClosestCommand(evt.newData);
        }

        private void KeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Tab)
            {
                UseSuggestion();
                evt.StopImmediatePropagation();
            }
            else if (evt.keyCode == KeyCode.A && evt.modifiers.HasFlag(EventModifiers.Control))
            {
                view.SelectPromptToEnd();
                evt.StopImmediatePropagation();
            }
            else if (evt.keyCode == KeyCode.Delete && isSuggestionVisible)
            {
                if (view.Selection.cursorIndex == inputSoFar.Length + config.Prompt.Length)
                {
                    evt.StopImmediatePropagation();
                }
            }
            else if (evt.keyCode == KeyCode.Home)
            {
                if (evt.modifiers.HasFlag(EventModifiers.Shift))
                {
                    view.SelectPromptToEnd();
                }
                else
                {
                    view.SetCursorAt(config.Prompt.Length);
                }

                evt.StopImmediatePropagation();
            }
            else if (evt.keyCode == KeyCode.End)
            {
                if (evt.modifiers.HasFlag(EventModifiers.Shift))
                {
                    view.SelectPromptToEnd();
                }
                else
                {
                    view.SetCursorAt(inputSoFar.Length + config.Prompt.Length);
                }

                evt.StopImmediatePropagation();
            }
            else if (evt.keyCode == KeyCode.Escape)
            {
                view.TextInput = inputSoFar = string.Empty;
                evt.StopImmediatePropagation();
            }
            else if (evt.keyCode == KeyCode.Backspace && view.TextInput.Length - 1 < config.Prompt.Length)
            {
                evt.StopImmediatePropagation();
            }
            else if (evt.keyCode == KeyCode.UpArrow)
            {
                HistoryGoBack(1);
                evt.StopImmediatePropagation();
            }
            else if (evt.keyCode == KeyCode.DownArrow)
            {
                HistoryGoForward(1);
                evt.StopImmediatePropagation();
            }
        }

        private void KeyUp(KeyUpEvent evt)
        {
            if (evt.keyCode == KeyCode.Return)
            {
                ProcessCommand(inputSoFar);
                view.TextInput = inputSoFar = string.Empty;
                view.FocusInput();
            }
        }

        private void ProcessCommand(string line)
        {
            string[] args = line.Replace(config.Prompt, string.Empty).Trim().Split(" ");
            if (string.IsNullOrEmpty(line))
            {
                if (config.EchoCommands)
                {
                    Echo(config.Prompt);
                }

                return;
            }

            string command   = args[0];
            string arguments = line.Length > command.Length ? line[(command.Length + 1)..] : string.Empty;

            bool searchCommand = model.Commands.ContainsKey(command);

            CommandContext context = new(command, arguments);

            if (config.EchoCommands)
            {
                Echo($"{config.Prompt}{line}");
            }

            if (searchCommand)
            {
                ExecuteCommand(context);
            }
            else
            {
                string errorMessage =
                    $"Command <{command.Highlight()}> has not been registered. Type <{"help".Highlight()}> to see a list of available commands.";
                Error(errorMessage, null);
            }

            commandHistory.Add(context);
            if (commandHistorySize != 0 && commandHistory.Count > commandHistorySize)
            {
                commandHistory.RemoveAt(0);
            }

            commandHistoryIndex = 0;
        }

        private void SendToOutput(string line, string lineClass = null)
        {
            model.AddLine(line, lineClass);
            view.AddLine(line, lineClass);
        }

        private void SuggestClosestCommand(string newString)
        {
            Regex regex = new(@"<color.*>");
            newString  = newString.Replace(config.Prompt, string.Empty);
            inputSoFar = regex.Replace(newString, string.Empty);
            string closest = model.Commands.Keys.FirstOrDefault(cmd => cmd.StartsWith(inputSoFar));
            if (closest != null && !string.IsNullOrEmpty(inputSoFar))
            {
                string suffix = closest[inputSoFar.Length..];
                view.TextInput      = $"{inputSoFar}{suffix.Info()}";
                isSuggestionVisible = true;
            }
            else
            {
                view.TextInput      = inputSoFar;
                isSuggestionVisible = false;
            }
        }

        private void Test(string args, ArgumentParser parser)
        {
            Echo("\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\"");
        }

        private void UseSuggestion()
        {
            string closest = model.Commands.Keys.FirstOrDefault(cmd => cmd.StartsWith(inputSoFar));
            inputSoFar     = closest;
            view.TextInput = closest;
            view.SetCursorAtEndline();
        }
    }
}