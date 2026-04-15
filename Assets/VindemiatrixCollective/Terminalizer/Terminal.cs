// Assembly-CSharp © 2025-2026 Vindemiatrix Collective

#region using

using UnityEngine;
using UnityEngine.UIElements;

#endregion

namespace VindemiatrixCollective.Terminalizer
{
    public class Terminal : MonoBehaviour
    {
        public TerminalConfig TerminalConfig;
        public UIDocument UIDocument;
        public VisualTreeAsset TerminalTemplate;

        public TerminalController Controller { get; private set; }

        public TerminalModel Model { get; private set; }

        public TerminalView View { get; private set; }

        private void OnEnable()
        {
            if (TerminalConfig == null)
            {
                TerminalConfig = TerminalConfig.Create();
            }

            Model = new TerminalModel(TerminalConfig);
            Model.AddLine("Welcome SmartHouse_0.13a.1207".Info());
            View       = new TerminalView(Model, TerminalTemplate, TerminalConfig, UIDocument);
            Controller = new TerminalController(Model, View, TerminalConfig);

            if (TerminalConfig.Shortcut != KeyCode.None)
            {
                View.RegisterShortcut(TerminalConfig.Shortcut);
            }
        }

        private void OnDestroy()
        {
            View.UnregisterHandlers();
        }
    }
}