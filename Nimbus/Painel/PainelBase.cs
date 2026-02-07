using Nimbus.Event;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nimbus.Painel
{
    internal abstract class PainelBase
    {
        protected readonly EventPublisher eventPublisher;

        internal bool RenderOptionFullScreen { get; }

        internal string PanelName { get; }

        internal PainelBase(EventPublisher ep, string panelName = "Panel", bool renderOptionFullScreen = false)
        {
            eventPublisher = ep;
            PanelName = panelName;
            RenderOptionFullScreen = renderOptionFullScreen;
        }

        internal abstract void HandleInput(ConsoleKey key);

        // TODO: RenderPanel();
        internal abstract IRenderable Render();

        internal abstract IRenderable? RenderControls();
    }
}
