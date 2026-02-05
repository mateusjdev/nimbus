using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nimbus
{
    internal class PainelPing : Painel
    {
        public Event HandleInput(ConsoleKey key)
        {
            Event mEvent = Event.None;
            if (key == ConsoleKey.Escape)
            {
                mEvent = Event.ClosePanel;
            }

            return mEvent;
        }

        public IRenderable Render()
        {
            return new Text("Pinging 1.1.1.1 with 32 bytes of data:");
        }
    }
}
