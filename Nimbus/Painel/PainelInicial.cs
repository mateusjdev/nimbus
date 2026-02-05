using Nimbus.Misc;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Nimbus.Painel
{
    internal class PainelInicial : IPainel
    {
        private PainelInicialOpcoes opcaoSelecionada = PainelInicialOpcoes.Ping;
        private int MaxOpcoesMenuInicial = Enum.GetValues<PainelInicialOpcoes>().Count() - 1;

        public bool RequestFullScreen { get { return false; } }

        public Event? HandleInput(ConsoleKey key)
        {
            Event? mEvent = null;
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    opcaoSelecionada--;
                    if ((int)opcaoSelecionada < 0)
                    {
                        opcaoSelecionada = (PainelInicialOpcoes)MaxOpcoesMenuInicial;
                    }
                    break;
                case ConsoleKey.DownArrow:
                    opcaoSelecionada++;
                    if ((int)opcaoSelecionada > MaxOpcoesMenuInicial)
                    {
                        opcaoSelecionada = 0;
                    }
                    break;
                case ConsoleKey.Enter:
                    switch (opcaoSelecionada)
                    {
                        case PainelInicialOpcoes.Ping:
                            mEvent = Event.OpenPing;
                            break;
                        case PainelInicialOpcoes.MachineTree:
                            mEvent = Event.OpenMachineTree;
                            break;
                        default:
                            // mEvent = Event.None;
                            break;
                    }
                    break;
                case ConsoleKey.Escape:
                    mEvent = Event.ClosePanel;
                    break;

            }
            return mEvent;
        }

        public IRenderable Render()
        {
            var rows = new List<Text>();
            var str = new StringBuilder();

            var selectedStyle = new Style(foreground: Color.Blue);

            foreach (var op in Enum.GetValues<PainelInicialOpcoes>())
            {
                if (op == opcaoSelecionada)
                {
                    rows.Add(new Text($"> {op}", selectedStyle) { Overflow = Overflow.Ellipsis });
                }
                else
                {
                    rows.Add(new Text($"  {op}") { Overflow = Overflow.Ellipsis });
                }
            }

            var iRows = new Rows(rows);
            var panel = new Panel(iRows)
                .Header("Opções")
                .HeaderAlignment(Justify.Center)
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Purple)
                .Expand();

            return panel;
        }

        public IRenderable RenderControls()
        {
            return new Text("[Esc] Sair [Enter] Selecionar", new Style(Color.Purple));
        }
    }

    enum PainelInicialOpcoes
    {
        Ping,
        MachineTree,
        Config,
        Option1,
        Option2,
        Option3,
    }
}
