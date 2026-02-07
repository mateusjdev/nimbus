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
    internal struct PainelSelecionOptions
    {
        internal string Text;
        internal int Value;
    }

    internal class PainelCommandSelector : IPainel
    {
        internal List<PainelSelecionOptions> options = new();
        private int OpSelecionada = 0;
        private int OpCount { get { return options.Count; } }

        internal PainelCommandSelector()
        {
            AddOption(new PainelSelecionOptions { Text = "Desligar", Value = (int)CommandType.Shutdown });
            AddOption(new PainelSelecionOptions { Text = "Reiniciar", Value = (int)CommandType.Reboot });
            AddOption(new PainelSelecionOptions { Text = "Acordar via RDP", Value = (int)CommandType.WakeUp });
            AddOption(new PainelSelecionOptions { Text = "Ping", Value = (int)CommandType.Ping });
            AddOption(new PainelSelecionOptions { Text = "Customizado", Value = (int)CommandType.Shell });
        }

        public void AddOption(PainelSelecionOptions option)
        {
            options.Add(option);
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
                .Header("Seletor de Comando")
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
