using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nimbus
{
    interface Painel
    {
        public Event HandleInput(ConsoleKey key);
        public IRenderable Render();
    }

    internal class PainelInicial : Painel
    {
        private PainelInicialOpcoes opcaoSelecionada = PainelInicialOpcoes.Ping;
        private int MaxOpcoesMenuInicial = Enum.GetValues<PainelInicialOpcoes>().Count() - 1;

        public Event HandleInput(ConsoleKey key)
        {
            Event mEvent = Event.None;
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
            return new Rows(rows);
        }
    }

    enum PainelInicialOpcoes
    {
        Ping,
        Config,
        Option1,
        Option2,
        Option3,
    }
}
