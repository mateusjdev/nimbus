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
    internal class PainelPing : IPainel
    {
        public bool RequestFullScreen { get { return false; } }

        public Event? HandleInput(ConsoleKey key)
        {
            Event? mEvent = null;
            if (key == ConsoleKey.Escape)
            {
                mEvent = Event.ClosePanel;
            }

            return mEvent;
        }

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
        }

        public IRenderable RenderControls()
        {
            return new Text("[Esc] Voltar", new Style(Color.Purple));
        }
    }
}
