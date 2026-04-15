// Assembly-CSharp © 2025-2026 Vindemiatrix Collective

#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.Assertions;

#endregion

namespace VindemiatrixCollective.Terminalizer
{
    public readonly struct ArgumentContext
    {
        public bool HasValue { get; }
        public bool IsOptional { get; }

        public Regex Validator { get; }

        public string Argument { get; }

        public ArgumentContext(string argument, Regex regex, bool isOptional, bool hasValue = true)
        {
            Argument   = argument;
            Validator  = regex;
            IsOptional = isOptional;
            HasValue   = hasValue;
        }
    }

    public class ArgumentParser
    {
        private readonly char separator;
        private readonly Dictionary<string, ArgumentContext> arguments;
        private readonly Dictionary<string, string> argumentValues;
        public string ErrorMessage { get; private set; }

        public ArgumentParser(char separator)
        {
            this.separator = separator;
            arguments      = new Dictionary<string, ArgumentContext>();
            argumentValues = new Dictionary<string, string>();
        }

        public bool ParseArguments(string input)
        {
            argumentValues.Clear();
            if (arguments.Count == 0)
            {
                return true;
            }


            ErrorMessage = string.Empty;
            string[] args = input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (string.IsNullOrEmpty(arg) && args.Length == 1 && NumberRequiredArguments > 0)
                {
                    ErrorMessage =
                        $"Missing arguments: {string.Join(", ", arguments.Values.Where(arg => !arg.IsOptional).Select(arg => arg.Argument))}";
                    return false;
                }

                if (arguments.TryGetValue(arg, out ArgumentContext context))
                {
                    if (context.HasValue)
                    {
                        arg = args.Length > i + 1 ? args[i + 1] : string.Empty;
                        i++;
                    }
                    else
                    {
                        arg = string.Empty;
                    }
                }
                else if (!arguments.TryGetValue($"arg{i}", out context))
                {
                    ErrorMessage = $"Argument '{arg}' not recognized";
                    return false;
                }

                Match match = context.Validator.Match(arg);
                if (match.Success)
                {
                    argumentValues.Add(context.Argument, match.Groups[0].Value);
                }
                else
                {
                    arg          = string.IsNullOrEmpty(arg) ? "empty" : arg;
                    ErrorMessage = $"Argument {context.Argument} value '{arg}' is invalid";
                    return false;
                }
            }

            string[] requiredArguments = arguments.Values.Where(arg => !arg.IsOptional).Select(arg => arg.Argument).ToArray();
            if (requiredArguments.Length == 0)
            {
                return true;
            }

            bool isSubset = requiredArguments.All(arg => argumentValues.ContainsKey(arg));

            if (!isSubset)
            {
                StringBuilder sb = new();
                foreach (string arg in requiredArguments)
                {
                    if (!argumentValues.ContainsKey(arg))
                    {
                        sb.Append($"Missing argument: {arg}");
                    }
                }

                ErrorMessage = sb.ToString();
            }

            return isSubset;
        }

        public bool TryGetValue(string argument, out string value) => argumentValues.TryGetValue(argument, out value);

        public string GetValue(string argument) => argumentValues[argument];

        public string Output()
        {
            StringBuilder sb = new();
            foreach (KeyValuePair<string, string> kvp in argumentValues)
            {
                sb.AppendLine($"{kvp.Key}: {kvp.Value}");
            }

            return sb.ToString();
        }

        public void AddArgument(string argument, Regex regex, bool optional = false, bool hasValue = true)
        {
            Assert.IsNotNull(regex, nameof(regex));
            Assert.IsFalse(string.IsNullOrEmpty(argument), nameof(argument));

            arguments.Add(argument, new ArgumentContext(argument, regex, optional, hasValue));
        }

        public class ArgumentBuilder
        {
            private readonly ArgumentParser parser;

            public ArgumentBuilder(char argumentSeparator = ' ')
            {
                parser = new ArgumentParser(argumentSeparator);
            }

            public ArgumentBuilder WithAlphanumericArgument(string argument, bool optional)
            {
                if (string.IsNullOrEmpty(argument))
                {
                    argument = GetDefaultArgName();
                }

                parser.AddArgument(argument, ValidateAlphanumeric(argument), optional);
                return this;
            }

            public ArgumentBuilder WithArgument(string argument, string regexPattern, bool optional)
            {
                PreProcessArgument(ref argument);
                parser.AddArgument(argument, ValidateRegex(argument, regexPattern), optional);
                return this;
            }

            public ArgumentBuilder WithEmptyArgument(string argument, bool optional)
            {
                PreProcessArgument(ref argument);
                parser.AddArgument(argument, new Regex(@"^$"), optional, false);
                return this;
            }

            public ArgumentBuilder WithNumericArgument(string argument, bool optional)
            {
                PreProcessArgument(ref argument);
                parser.AddArgument(argument, ValidateNumeric(argument), optional);
                return this;
            }

            public ArgumentParser Build() => parser;

            private string GetDefaultArgName() => $"arg{parser.arguments.Count}";

            private void PreProcessArgument(ref string argument)
            {
                if (string.IsNullOrEmpty(argument))
                {
                    argument = GetDefaultArgName();
                }
            }

            private Regex ValidateAlphanumeric(string argument) => ValidateRegex(argument, @"[A-Za-z0-9!?,.\-_]+");

            private Regex ValidateNumeric(string argument) => ValidateRegex(argument, @"[0-9.,\-+]+");

            private Regex ValidateRegex(string argument, string pattern)
            {
                if (argument[0] == '-')
                {
                    argument = argument[1..];
                }

                return new Regex(@$"(?<{argument}>{pattern})");
            }
        }

        private int NumberRequiredArguments => arguments.Values.Count(arg => !arg.IsOptional);
    }
}