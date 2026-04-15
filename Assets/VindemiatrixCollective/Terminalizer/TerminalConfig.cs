// Assembly-CSharp © 2025-2026 Vindemiatrix Collective

#region using

using Unity.Properties;
using UnityEngine;

#endregion

namespace VindemiatrixCollective.Terminalizer
{
    [CreateAssetMenu(fileName = "Dialog", menuName = "Terminalizer/Create Terminal config")]
    public class TerminalConfig : ScriptableObject
    {
        [SerializeField] private bool echoCommands;
        [SerializeField] private bool initialVisibility;
        [SerializeField] private Font terminalFont;
        [SerializeField] private int commandHistorySize;
        [SerializeField] private KeyCode shortcut;
        [SerializeField] private string prompt;
        [SerializeField] private string title;

        [SerializeField] private string[] lineClasses;
        [SerializeField] private string[] textInputClasses;
        [SerializeField] private Vector2 initialSize;
        [SerializeField] private Vector2 startLocation;

        public bool EchoCommands => echoCommands;
        public bool InitialVisibility => initialVisibility;
        public Font TerminalFont => terminalFont;
        public int CommandHistorySize => commandHistorySize;
        public KeyCode Shortcut => shortcut;
        public string Prompt => prompt;
        [CreateProperty] public string Title => title;
        public string[] LineClasses => lineClasses;
        public string[] TextInputClasses => textInputClasses;
        public Vector2 InitialSize => initialSize;
        public Vector2 StartLocation => startLocation;

        public static TerminalConfig Create()
        {
            TerminalConfig config = CreateInstance<TerminalConfig>();

            config.echoCommands       = true;
            config.initialVisibility  = true;
            config.terminalFont       = null;
            config.commandHistorySize = 10;
            config.prompt             = "> ";
            config.title              = "Terminal";
            config.lineClasses        = new[] { "text-color", "text-lg" };
            config.textInputClasses   = new[] { "text-lg" };
            config.initialSize        = new Vector2(800, 600);
            config.startLocation      = new Vector2(32, 32);
            config.shortcut           = KeyCode.BackQuote;
            return config;
        }
    }
}