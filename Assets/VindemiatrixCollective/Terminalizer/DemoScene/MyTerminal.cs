// Assembly-CSharp © 2025-2026 Vindemiatrix Collective

#region using

using System.Text;
using UnityEngine;

#endregion

namespace VindemiatrixCollective.Terminalizer.DemoScene
{
    public class MyTerminal : MonoBehaviour
    {
        private Terminal terminal;

        private void Start()
        {
            // first, get a reference to the Terminal component
            terminal = GetComponent<Terminal>();

            // the terminal will automatically register a KeyUp handler to the UI Document's Root Element
            // in order for this to work, the Root Element will be set to "Focusable" and its PickingMode to "Position"
            // you can change it via
            terminal.View.RegisterShortcut(KeyCode.F1); // for example
            // otherwise just use a different keycode in the Config asset
            // for best results, we recommend using an action in the Unity (new) input system
            // and triggering the console that way

            // back to the method now!
            // we will add a command "add" that ... adds two numbers together
            // we can do so via:

            terminal.Model.RegisterCommand("add", Add, "adds two numbers together",
                                           new ArgumentParser.ArgumentBuilder()
                                              .WithNumericArgument("-a", false)
                                              .WithNumericArgument("-b", false)
                                              .Build());
            // these methods With...Argument, perform input validation for you
            // we now need to implement the Add method itself

            // ok now we can add a more complex command, such as this one to fetch the values of various gameobject properties
            terminal.Model.RegisterCommand("get", Get, "get [name] -arguments", new ArgumentParser.ArgumentBuilder()
                                               // unnamed arguments are automatically named arg0, arg1, etc.
                                               // depending on their position
                                              .WithAlphanumericArgument(string.Empty, false)
                                               // "EmptyArgument" means that you don't expect there to be a value next to it
                                               // the true/false specifies if the argument is optional (true) or not (false)
                                              .WithEmptyArgument("-p", true)
                                              .WithEmptyArgument("-r", true)
                                              .WithEmptyArgument("-s", true)
                                              .WithEmptyArgument("-m", true)
                                              .Build());

            // Command with only optional arguments must still work if no arguments are provided
            terminal.Model.RegisterCommand("optCmd", OptCmd, "optCmd -arguments",
                                           new ArgumentParser.ArgumentBuilder().WithEmptyArgument("-a", true).Build());
        }

        private void OptCmd(string rawArguments, ArgumentParser parser)
        {
            // this is an optional command
            // you can implement it as you wish
            terminal.Controller.Echo("This is an optional command!");
        }

        private void Add(string rawArguments, ArgumentParser parser)
        {
            // rawArguments: contains the unprocessed string
            // parser: a helper object that automatically provides you all the argument values you specified
            string a = parser.GetValue("-a");
            string b = parser.GetValue("-b");
            // if you specify 'string.empty' instead of -a or -b, those arguments will be called 'arg0', 'arg1', etc.

            // now we can perform the addition
            int aValue = int.Parse(a);
            int bValue = int.Parse(b);
            // this is for demonstration purposes only
            // in a real setting, you would implement more robust parsing logic
            // do not forget specifying the culture too :) (InvariantCulture to avoid comma/dot problems)

            int result = aValue + bValue;
            terminal.Controller.Echo($"the result is: {result}");

            // terminal.Controller provides some helper functions that are built-in:
            // let's try it now!
        }

        private void Get(string rawArguments, ArgumentParser parser)
        {
            // we used an unnamed argument in the first position
            // it will be called "arg0"
            string arg0 = parser.GetValue("arg0");
            if (string.IsNullOrEmpty(arg0))
            {
                terminal.Controller.Error("Must specify the name of a GameObject as the first argument.", parser);
            }

            GameObject gameObject = GameObject.Find(arg0);
            if (gameObject == null)
            {
                terminal.Controller.Error($"GameObject <{arg0}> not found", parser);
                return;
            }

            StringBuilder sb = new();
            if (parser.TryGetValue("-p", out string value))
            {
                sb.AppendLine($"position: {gameObject.transform.position.ToString()}");
            }

            // we can now add the other properties:

            if (parser.TryGetValue("-s", out value))
            {
                sb.AppendLine($"localScale: {gameObject.transform.localScale.ToString()}");
            }

            if (parser.TryGetValue("-m", out value))
            {
                sb.AppendLine($"material name: {gameObject.GetComponent<MeshRenderer>().sharedMaterial.name}");
            }

            if (sb.Length == 0)
            {
                terminal.Controller.Error("No arguments provided.", parser);
            }
            else
            {
                terminal.Controller.Echo(sb.ToString());
            }

            // this should be it! Of course, in Terminalizer you can add any command handler you need!
        }
    }
}