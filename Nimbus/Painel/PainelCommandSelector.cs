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
    internal struct PainelSelecionOptions<T>
    {
        internal string Text;
        internal T Value;
    }

    internal class PainelCommandSelector<T> : IPainel
    {
        internal List<PainelSelecionOptions<T>> options = new();

        private string PanelName;

        private Action? OnSelect;

        internal PainelCommandSelector(string panelName)
        {
            PanelName = panelName;
        }

        private int OpSelecionada = 0;
        private int OpCount { get { return options.Count; } }

        public PainelCommandSelector<T> AddOption(string text, T value)
        {
            PainelSelecionOptions<T> op = new();
            options.Add(new PainelSelecionOptions<T> { Text = text, Value = value });
            return this;
        }

        public T GetSelected()
        {
            // TODO: Check bounds
            return options.ElementAt(OpSelecionada).Value;
        }

        public void SetOnSelect(Action action)
        {
            OnSelect = action;
        }

        public bool RequestFullScreen { get { return false; } }

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
                    break;
                case ConsoleKey.DownArrow:
                    OpSelecionada++;
                    if (OpSelecionada > OpCount)
                    {
                        OpSelecionada = 0;
                    }
                    break;
                case ConsoleKey.Enter:
                    // TODO: Validate values (Ensure Non Integers
                    // Confirm
                    // Open Comand Executor
                    OnSelect?.Invoke();
                    break;
                case ConsoleKey.Escape:
                    mEvent = new Event(EventType.ClosePanel);
                    break;

            }
            return mEvent;
        }

        public IRenderable Render()
        {
            var rows = new List<Text>();
            var str = new StringBuilder();

            var selectedStyle = new Style(foreground: Color.Blue);

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
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Purple)
                .Expand();

            return panel;
        }

        /*
        public IRenderable Render()
        {
            var text = new Text("Pinging 1.1.1.1 with 32 bytes of data:");
            var panel = new Panel(text)
                .Header("Ping")
                .HeaderAlignment(Justify.Center)
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Purple)
                .Expand();

            return panel;
        }*/

        public IRenderable RenderControls()
        {
            return new Text("[Esc] Voltar", new Style(Color.Purple));
        }
    }
}
