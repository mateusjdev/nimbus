using Nimbus.Misc;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nimbus.Painel
{
    internal interface IPainel
    {
        public bool RenderOptionFullScreen { get; }

        public Event? HandleInput(ConsoleKey key);

        public IRenderable Render();

        public IRenderable? RenderControls();
    }
}
