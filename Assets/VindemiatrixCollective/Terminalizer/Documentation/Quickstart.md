# Welcome to Terminalizer 1.0 by the Vindemiatrix Collective
Thanks for purchasing Terminalizer!
We recommend watching the video/tutorial on YouTube at: https://youtu.be/pKkzSiwAISg

## Guide
The "Terminalizer DemoScene" sample provides a scene with everything already set up. If you want to integrate Terminalizer in your own project, you must follow these steps:
1.	Create a `Terminal Config` asset and edits its properties, via the `Assets/Create/Terminalizer`menu.
2.	The Line and Text Input classes field refers to classes in the file `Terminalizer/UI Toolkit/Unity Themes/Terminalizer theme.uss`. "Line Classes" will be applied to every output line in the terminal, while "Text Input Classes" will be applied to the prompt only.
3.	If you wish to use your own theme file, make sure to set yours as the default instead of the "UI theme.tss" in the same folder.
4.	You can edit the provided `Terminalizer/UI Toolkit/Theme/Terminalizer Theme.uss` file to style the terminal.
5.	Once you have created a `Terminal Config`, add an empty GameObject and give it a name. Be sure to specify a sequence of characters for the prompt, like "> " without the quotes (the greater than symbol and a space)
6.	Add a `Terminal` component to the newly created GameObject.
7.	Link it with the needed objects: the `Terminal Config` asset, a reference to your `UI Document` object, and a reference to the `Terminal.UXML` file.
8.	If you then want to add a command handler, create a new script, preferably in the same GameObject. 

Get a reference to the `Terminal` component and use its methods to add commands. Watch the linked video for an in-depth tutorial.

There are three main properties you can access from the `Terminal` component:

* The `Terminal.View` object controls all aspects of its inner view-facing components (e.g., show, hide, select all, move cursor, etc.)
* The `Terminal.Controller` implements the basic functions of the terminal such as `Echo`, `Error` and `Help`. 
* You should use `Echo` and `Error` whenever you want to display something to the Terminal.
* The `Terminal.Model` is holds the unprocessed text lines displayed in the terminal and is used to (un)register commands.

See the provided `MyTerminal.cs` script and the video tutorial for more detailed instructions on how to use Terminalizer.

## Notes about using a key to trigger the appearance of the Terminalizer console
The terminal will automatically register the KeyCode specified in the config via a KeyUp handler to the UI Document's Root Element. In order for this to work, the Root Element will be set to "Focusable" and its PickingMode to "Position". This means that clicks might not get through to underlying UI elements. For that to work, it must be set to "Ignore". To disable this behaviour, set "Shortcut" in the Terminal Config asset to "None".

    `terminal.View.RegisterShortcut(KeyCode.F1); // for example`

For best results, we recommend using an action in the Unity (new) input system and triggering the console that way.

## Support
Need support for a specific feature? Do you have a question? You can either email us at vindemiatrixcollective@gmail.com or join our Discord via: https://discord.gg/qCX6XKvJ4f

Thanks again for purchasing Terminalizer!