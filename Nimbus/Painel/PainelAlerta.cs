using Nimbus.Event;
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

    internal class PainelAlerta : PainelBase
    {
        private readonly StringBuilder str = new();
        private Color borderColor = Color.White;
        private readonly TipoAlerta tipoAlerta;

        public PainelAlerta(EventPublisher ec, TipoAlerta tipoAlerta, string msg) : base(ec, renderOptionFullScreen: true)
        {
            this.tipoAlerta = tipoAlerta;
            SetMarkup();
            str.Append(msg);
        }

        private void SetMarkup()
        {
            str.Clear();
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
        }

        internal override void HandleInput(ConsoleKey key) { }

        internal override IRenderable Render()
        {
            var text = new Markup(str.ToString()) { Justification = Justify.Center }
                .Centered();

            var panel = new Panel(text)
                .Header("Alerta")
                .HeaderAlignment(Justify.Center)
                .Border(BoxBorder.Rounded)
                .BorderColor(borderColor);

            return Align.Center(panel, VerticalAlignment.Middle);
        }

        internal override IRenderable? RenderControls()
        {
            return null;
        }

        public void ChangeText(string msg)
        {
            SetMarkup();
            str.Append(msg);
        }
    }
}
