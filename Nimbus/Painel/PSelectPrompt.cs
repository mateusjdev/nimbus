using Nimbus.Config;
using Nimbus.Misc;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nimbus.Painel
{

    internal class PSelectPrompt<T> : IPainel where T : Enum
    {
        #region private

        private struct PTreeOption
        {
            internal string Text;
            internal T Value;
            internal bool Confirm;
        }

        private readonly List<PTreeOption> options = [];

        private readonly string PanelName;

        private Action? OnSelect;

        private int OpSelecionada = 0;

        private int OpCount { get { return options.Count; } }

        #endregion private

        #region public

        internal PSelectPrompt(string panelName)
        {
            PanelName = panelName;
        }

        internal PSelectPrompt<T> AddOption(string text, T value, bool confirm = false)
        {
            options.Add(new PTreeOption { Text = text, Value = value, Confirm = confirm });
            return this;
        }

        internal T? GetSelected()
        {
            if (options.Count == 0)
            {
                return default;
            }

            // TODO: Check bounds
            return options.ElementAt(OpSelecionada).Value;
        }

        internal void SetOnSelect(Action action)
        {
            OnSelect = action;
        }

        #endregion public

        #region interface

        public bool RenderOptionFullScreen { get { return false; } }

        public Event? HandleInput(ConsoleKey key)
        {
            Event? mEvent = null;
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    OpSelecionada--;
                    if (OpSelecionada < 0)
                    {
                        OpSelecionada = OpCount;
                    }
                    mEvent = new Event(EventType.None, flagRequestRender: true);
                    break;
                case ConsoleKey.DownArrow:
                    OpSelecionada++;
                    if (OpSelecionada > OpCount)
                    {
                        OpSelecionada = 0;
                    }
                    mEvent = new Event(EventType.None, flagRequestRender: true);
                    break;
                case ConsoleKey.Enter:
                    if (options.ElementAt(OpSelecionada).Confirm)
                    {
                        // TODO: Prompt Confirm
                    }
                    OnSelect?.Invoke();
                    mEvent = new Event(EventType.None, flagRequestRender: true);
                    break;
                case ConsoleKey.Escape:
                    mEvent = new Event(EventType.ClosePanel, flagRequestRender: true);
                    break;
            }
            return mEvent;
        }

        public IRenderable Render()
        {
            List<Text> rows = [];
            var selectedStyle = new Style(foreground: Theme.SelectedItemColor);

            for (int i = 0; i < options.Count; i++)
            {
                if (OpSelecionada == i)
                {
                    rows.Add(new Text($"> {options.ElementAt(i).Text}", selectedStyle) { Overflow = Overflow.Ellipsis });
                }
                else
                {
                    rows.Add(new Text($"  {options.ElementAt(i).Text}") { Overflow = Overflow.Ellipsis });
                }
            }

            var iRows = new Rows(rows);

            var panel = new Panel(iRows)
                .Header(PanelName)
                .HeaderAlignment(Justify.Center)
                .Border(Theme.BorderShape)
                .BorderColor(Theme.BorderColor)
                .Expand();

            return panel;
        }

        public IRenderable RenderControls()
        {
            return new Text("[Esc] Voltar [Enter] Selecionar", new Style(Theme.ControlsTextColor));
        }

        #endregion interface
    }
}
