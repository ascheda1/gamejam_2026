// Terminalizer © 2025 Vindemiatrix Collective
// Website and Documentation - https://dev.vindemiatrixcollective.com

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

#endregion

namespace VindemiatrixCollective.Terminalizer
{
    public readonly struct LineContext
    {
        public string Line { get; }

        public string LineClass { get; }

        public LineContext(string line, string lineClass = null)
        {
            Line = line;
            LineClass = lineClass;
        }
    }

    public class TerminalView
    {
        private readonly Button btnClose;
        private readonly List<LineContext> screenLines;
        private readonly ListView listScreen;
        private readonly Scroller scrollbar;
        private readonly StyleFontDefinition fontDefinition;
        private readonly TerminalConfig config;
        private readonly TerminalModel model;
        private readonly TextElement textElement;
        private readonly TextField textInput;
        private readonly UIDocument uiDocument;
        private readonly VisualElement rowScreen;
        private readonly VisualElement terminalElement;
        private readonly VisualElement terminalViewport;

        private DragManipulator dragManipulator;
        private EventCallback<InputEvent> inputEventProcessor;
        private EventCallback<KeyDownEvent> keyDownProcessor;
        private EventCallback<KeyUpEvent> keyUpProcessor;

        public int InputCursorIndex => textElement.selection.cursorIndex;

        public ITextSelection Selection => textElement.selection;

        /// <summary>
        ///     Retrieves or sets the value of the TextField input without notify.
        /// </summary>
        public string TextInput
        {
            get => textInput.value;
            set
            {
                if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(config.Prompt))
                {
                    textInput.SetValueWithoutNotify(config.Prompt);
                }
                else
                {
                    textInput.SetValueWithoutNotify($"{config.Prompt}{value}");
                }
            }
        }

        public TerminalView(TerminalModel model, VisualTreeAsset terminalTemplate, TerminalConfig config, UIDocument uiDocument)
        {
            this.model = model;
            this.config = config;
            this.uiDocument = uiDocument;
            terminalElement = terminalTemplate.Instantiate().Q<VisualElement>("terminal");
            terminalElement.dataSource = config;
            terminalElement.transform.position = new Vector3(config.StartLocation.x, config.StartLocation.y, 0);

            textInput = terminalElement.Q<TextField>("text-input");
            textElement = textInput.Q("unity-text-input").Q<TextElement>();
            listScreen = terminalElement.Q<ListView>("list-screen");
            terminalViewport = listScreen.Q<VisualElement>("unity-content-viewport");
            scrollbar = listScreen.Q<Scroller>();
            btnClose = terminalElement.Q<Button>("btn-close");
            rowScreen = terminalElement.Q<VisualElement>("row-screen");
            VisualElement headerBar = terminalElement.Q<VisualElement>("row-header");

            if (config.TextInputClasses != null)
            {
                foreach (string textInputClass in config.TextInputClasses)
                {
                    textInput.AddToClassList(textInputClass);
                }
            }

            TextInput = string.Empty;

            if (config.TerminalFont != null)
            {
                fontDefinition = new StyleFontDefinition(config.TerminalFont);
            }
            else
            {
                fontDefinition = new StyleFontDefinition(new Font("NotInter-Regular"));
            }

            textInput.style.unityFontDefinition = fontDefinition;

            textElement.enableRichText = true;
            btnClose.clicked += Hide;

            screenLines = new List<LineContext>(model.Lines);

            listScreen.bindItem += BindItem;
            listScreen.makeItem += MakeItem;
            listScreen.itemsSource = screenLines;

            dragManipulator = new DragManipulator(headerBar, terminalElement);

            Selection.selectAllOnFocus = false;
            Selection.selectAllOnMouseUp = false;

            if (!config.InitialVisibility)
            {
                Hide();
            }

            uiDocument.rootVisualElement.Add(terminalElement);

            textInput.RegisterCallbackOnce<GeometryChangedEvent>(evt =>
            {
                textInput.style.minHeight = textInput.resolvedStyle.fontSize + textInput.resolvedStyle.paddingTop
                                                                             + textInput.resolvedStyle.paddingBottom;
                listScreen.fixedItemHeight = textInput.resolvedStyle.fontSize + 4;
                if (model.Lines > 0)
                {
                    terminalElement.schedule.Execute(Rebuild);
                }
            });

            RegisterHandlers();
        }

        /// <summary>
        ///     Measures the width of the text. It only works if the controls have been laid out in the UI and at least a frame has
        ///     passed.
        /// </summary>
        /// <param name="line">The text you want to measure.</param>
        /// <returns>Its size in pixels.</returns>
        public Vector2 MeasureText(string line) =>
            textInput.MeasureTextSize(line, 0, VisualElement.MeasureMode.Undefined, 0, VisualElement.MeasureMode.Undefined);

        /// <summary>
        ///     Adds a line to the terminal screen.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="lineClass">The USS class you want to give it.</param>
        public void AddLine(string line, string lineClass)
        {
            int linesAdded = SplitLines(line, MaxWidth, lineClass);
            listScreen.RefreshItems();
            listScreen.ScrollToItem(-1);

            listScreen.schedule.Execute(() =>
            {
                if (scrollbar.visible) Rebuild();
            });
        }


        public void BindKeyDownEvent(EventCallback<KeyDownEvent> keyDownProcessor)
        {
            this.keyDownProcessor = keyDownProcessor;
            textInput.RegisterCallback(keyDownProcessor, TrickleDown.TrickleDown);
        }

        public void BindKeyUpProcessor(EventCallback<KeyUpEvent> keyUpProcessor)
        {
            this.keyUpProcessor = keyUpProcessor;
            textInput.RegisterCallback(keyUpProcessor);
        }

        public void BindTextInputEvent(EventCallback<InputEvent> inputEvent)
        {
            inputEventProcessor = inputEvent;
            textInput.RegisterCallback(inputEventProcessor);
        }

        /// <summary>
        ///     Clears the contents of the screen.
        /// </summary>
        public void Clear()
        {
            screenLines.Clear();
            listScreen.Rebuild();
        }

        /// <summary>
        ///     Removes the Terminal window from its parent.
        /// </summary>
        public void Close()
        {
            terminalElement.RemoveFromHierarchy();
        }

        /// <summary>
        ///     Manually focuses the TextField.
        /// </summary>
        public void FocusInput()
        {
            textInput.Focus();
            if (!string.IsNullOrEmpty(config.Prompt))
            {
                SelectPromptToEnd();
            }
        }

        /// <summary>
        ///     Hides the Terminal window.
        /// </summary>
        public void Hide()
        {
            terminalElement.visible = false;
        }

        /// <summary>
        ///     Rebinds and displays the Terminal lines currently visible.
        /// </summary>
        public void Rebuild()
        {
            screenLines.Clear();

            foreach (LineContext lineContext in model.LinesUnwrapped)
            {
                int linesAdded = SplitLines(lineContext.Line, MaxWidth, lineContext.LineClass);
            }

            listScreen.RefreshItems();
            listScreen.ScrollToItem(-1);
        }

        /// <summary>
        ///     Register a KeyUp callback responding to the release of the given Keycode,
        ///     that toggles the visibility of the Terminal Window.
        ///     Event is added to the UIDocument's RootVisualElement./>
        /// </summary>
        /// <param name="keyCode">The key you want to use as a shortcut.</param>
        /// <exception cref="InvalidOperationException">
        ///     Some keys are reserved and cannot be used as a shortcut.
        ///     E.g.: Escape, Return, KeypadEnter, Home, End, Up/Down/Left/Rightarrow, Delete, Backspace,
        ///     Shift, Alt, Control, Tab, Windows/Apple key, Space.
        /// </exception>
        public void RegisterShortcut(KeyCode keyCode)
        {
            KeyCode[] invalidKeyCodes =
            {
                KeyCode.Escape,
                KeyCode.Return,
                KeyCode.KeypadEnter,
                KeyCode.Home,
                KeyCode.End,
                KeyCode.UpArrow,
                KeyCode.DownArrow,
                KeyCode.LeftArrow,
                KeyCode.RightArrow,
                KeyCode.Delete,
                KeyCode.Backspace,
                KeyCode.Tab,
                KeyCode.LeftShift,
                KeyCode.RightShift,
                KeyCode.LeftAlt,
                KeyCode.RightAlt,
                KeyCode.LeftControl,
                KeyCode.RightControl,
                KeyCode.LeftWindows,
                KeyCode.RightWindows,
                KeyCode.LeftApple,
                KeyCode.RightApple,
                KeyCode.Space,
                KeyCode.AltGr
            };
            if (invalidKeyCodes.Contains(keyCode))
            {
                throw new InvalidOperationException($"Terminalizer: KeyCode {keyCode.ToString()} cannot be used as a shortcut");
            }

            VisualElement root = uiDocument.rootVisualElement;
            root.RegisterCallback<KeyDownEvent>(ToggleVisibility, TrickleDown.TrickleDown);
            root.pickingMode = PickingMode.Position;
            root.focusable = true;
            root.Focus();
        }

        public void SelectPromptToEnd()
        {
            Selection.SelectRange(config.Prompt.Length, TextInput.Length);
        }

        public void SetCursorAt(int cursorIndex)
        {
            Selection.SelectRange(cursorIndex, cursorIndex);
        }

        public void SetCursorAtEndline()
        {
            int cursorIndex = TextInput.Length;
            Selection.SelectRange(cursorIndex, cursorIndex);
        }

        /// <summary>
        ///     Shows the Terminal window if hidden.
        /// </summary>
        public void Show()
        {
            terminalElement.visible = true;
        }

        /// <summary>
        ///     Unregisters event handlers.
        /// </summary>
        public void UnregisterHandlers()
        {
            textInput.UnregisterCallback<FocusEvent>(OnFocus);
            textInput.UnregisterCallback(keyDownProcessor);
            textInput.UnregisterCallback(keyUpProcessor);
            textInput.UnregisterCallback(inputEventProcessor);
            textInput.UnregisterCallback<BlurEvent>(OnBlur);
        }

        private void BindItem(VisualElement element, int index)
        {
            Label label = (Label)element;
            LineContext context = screenLines[index];
            label.text = context.Line;

            label.style.unityFontDefinition = fontDefinition;
            foreach (string lineClass in config.LineClasses)
            {
                if (string.IsNullOrEmpty(lineClass))
                {
                    continue;
                }

                label.AddToClassList(lineClass);
            }

            if (!string.IsNullOrEmpty(context.LineClass))
            {
                label.SwapClass("text-color", context.LineClass);
            }
        }


        private VisualElement MakeItem()
        {
            Label label = new() { style = { overflow = Overflow.Hidden, fontSize = textInput.resolvedStyle.fontSize } };
            if (config.LineClasses != null)
            {
                foreach (string lineClass in config.LineClasses)
                {
                    label.AddToClassList(lineClass);
                }
            }

            return label;
        }

        private void OnBlur(BlurEvent evt)
        {
            TextInputBlur?.Invoke(this, evt);
        }

        private void OnFocus(FocusEvent evt)
        {
            SelectPromptToEnd();
            TextInputFocus?.Invoke(this, evt);
        }

        private void RegisterHandlers()
        {
            textInput.RegisterCallback<FocusEvent>(OnFocus);
            textInput.RegisterCallback<BlurEvent>(OnBlur);
        }

        private int SplitLines(string line, float maxWidth, string lineClass = null)
        {
            int linesAdded = 0;
            Vector2 size = MeasureText(line);

            if (size.x < maxWidth && size.y < listScreen.fixedItemHeight)
            {
                screenLines.Add(new LineContext(line, lineClass));
                return 1;
            }

            List<string> lines = new();
            StringBuilder currentLine = new();
            int lineLength = 0;
            int wordStart = 0;

            for (int i = 0; i <= line.Length; i++)
            {
                bool isEnd = i == line.Length;
                char c = isEnd ? ' ' : line[i];

                if (c == '\n')
                {
                    int wordLength = i - wordStart;
                    string word = line.Substring(wordStart, wordLength);
                    currentLine.AppendLine(' ' + word);
                    lines.Add(currentLine.ToString());
                    currentLine.Clear();
                    lineLength = 0;
                    linesAdded++;
                    wordStart = i + 1;
                }
                else if (c == '\t')
                {
                    currentLine.Append("   ");
                }
                else if (char.IsWhiteSpace(c) || isEnd)
                {
                    int wordLength = i - wordStart;
                    if (wordLength > 0)
                    {
                        string word = line.Substring(wordStart, wordLength);
                        if (MeasureText(currentLine + " " + word).x > maxWidth)
                        {
                            lines.Add(currentLine.ToString());
                            currentLine.Clear();
                            lineLength = 0;
                            linesAdded++;
                        }

                        if (lineLength > 0)
                        {
                            currentLine.Append(' ');
                            lineLength += 1;
                        }

                        currentLine.Append(word);
                        lineLength += wordLength;
                    }

                    wordStart = i + 1;
                }
            }

            if (currentLine.Length > 0)
            {
                lines.Add(currentLine.ToString());
            }

            foreach (string s in lines)
            {
                screenLines.Add(new LineContext(s, lineClass));
            }

            return linesAdded;
        }

        private void ToggleVisibility(KeyDownEvent evt)
        {
            bool inFocus = textInput.focusController.focusedElement == textInput;
            if (evt.keyCode == config.Shortcut && !inFocus)
            {
                if (terminalElement.visible)
                {
                    Hide();
                }
                else
                {
                    Show();
                }

                evt.StopImmediatePropagation();
            }
        }

        private float MaxWidth => scrollbar.visible
            ? terminalViewport.resolvedStyle.width - rowScreen.resolvedStyle.paddingRight
            : terminalElement.resolvedStyle.width;


        public event EventHandler<FocusEvent> TextInputFocus;
        public event EventHandler<BlurEvent> TextInputBlur;
    }
}