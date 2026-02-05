using Nimbus.Misc;
using Spectre.Console;
using Spectre.Console.Extensions;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Nimbus.Painel
{

    internal enum TipoAlerta
    {
        Error,
        Information,
    }

    internal class PainelAlerta : IPainel
    {
        private StringBuilder str = new();

        private Color borderColor = Color.White;

        public bool RequestFullScreen { get { return true; } }

        public PainelAlerta(TipoAlerta tipoAlerta, string msg)
        {
            switch (tipoAlerta)
            {
                case TipoAlerta.Error:
                    str.Append("[bold red]✗ Error: [/]");
                    borderColor = Color.Red;
                    break;
                case TipoAlerta.Information:
                    str.Append("[bold blue]✗ Info: [/]");
                    borderColor = Color.Blue;
                    break;
            }

            str.Append(msg);
        }

        public Event HandleInput(ConsoleKey key)
        {
            return Event.None;
        }

        public IRenderable Render()
        {
            var consoleWidth = AnsiConsole.Console.Profile.Width;
            var consoleHeight = AnsiConsole.Console.Profile.Height;

            var text = new Markup(str.ToString() + $" ({consoleWidth}x{consoleHeight})")
                { Justification = Justify.Center }
                .Centered();
            var panel = new Panel(text)
                .Header("Alerta")
                .HeaderAlignment(Justify.Center)
                .Border(BoxBorder.Rounded)
                .BorderColor(borderColor);
            return Align.Center(panel, VerticalAlignment.Middle);
        }
    }
}
