using Nimbus.Config;
using Nimbus.Event;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nimbus.Painel.Tree
{
    internal sealed class PTree<T> : PainelBase where T : ITreeElementDisplay
    {
        #region interface

        internal PTree(EventPublisher ep) : base(ep, "Machine Tree") { }

        internal override void HandleInput(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.Escape:
                    eventPublisher(new EventData(EventType.ClosePanel));
                    eventPublisher(new EventData(EventType.RequestRender));
                    break;
                case ConsoleKey.UpArrow:
                    MoveSelectUp();
                    eventPublisher(new EventData(EventType.RequestRender));
                    break;
                case ConsoleKey.DownArrow:
                    MoveSelectDown();
                    eventPublisher(new EventData(EventType.RequestRender));
                    break;
                case ConsoleKey.Enter:
                    ShowInfo();
                    eventPublisher(new EventData(EventType.RequestRender));
                    break;
                case ConsoleKey.Spacebar:
                    Toggle();
                    eventPublisher(new EventData(EventType.RequestRender));
                    break;
                case ConsoleKey.A:
                    ExpandAll();
                    eventPublisher(new EventData(EventType.RequestRender));
                    break;
                case ConsoleKey.F:
                    ColapseAll();
                    eventPublisher(new EventData(EventType.RequestRender));
                    break;
                case ConsoleKey.C:
                    SendCommand();
                    eventPublisher(new EventData(EventType.RequestRender));
                    break;
                case ConsoleKey.E:
                    // TODO: EDITAR
                    eventPublisher(new EventData(EventType.RequestRender));
                    break;
                case ConsoleKey.P:
                    SendCommand();
                    eventPublisher(new EventData(EventType.RequestRender));
                    break;
            }
        }

        internal override IRenderable Render()
        {
            IRenderable layout;

            if (root != null)
            {
                layout = new Rows(root.RenderTreeElements());
            }
            else
            {
                layout = new Text("Tree have 0 items.");
            }

            var panel = new Panel(layout)
               .Header(PanelName)
               .HeaderAlignment(Justify.Center)
               .Border(Theme.BorderShape)
               .BorderColor(Theme.BorderColor)
               .Expand();

            return panel;
        }

        internal override IRenderable? RenderControls()
        {
            StringBuilder str = new();
            str.Append("[Esc] Voltar ");
            str.Append("[Espaço] Alternar ");
            str.Append("[e] Editar ");
            str.Append("[p] Ping ");
            str.Append("[c] Comandos ");
            str.Append("[A] Abrir ");
            str.Append("[F] Fechar ");

            return new Text(str.ToString(), new Style(Theme.ControlsTextColor));
        }

        #endregion interface

        #region private

        private ITreeElement<T>? root;

        public void MoveSelectUp()
        {
            if (root != null && root is TreeFolder<T> tf)
            {
                var overflow = tf.MoveSelectUp();
                if (overflow)
                {
                    root.SetSelected(true);
                }
            }
        }

        public void MoveSelectDown()
        {
            if (root != null && root is TreeFolder<T> tf)
            {
                var overflow = tf.MoveSelectDown();
                if (overflow)
                {
                    tf.SetSelected(true);
                }
            }
        }

        private void ShowInfo()
        {
            // TODO: if(folder) Toggle()
            // else DisplayInfo(Selected)
        }

        private void Toggle()
        {
            if (root != null && root is TreeFolder<T> tf)
            {
                tf.Toggle();
            }
        }

        private void ExpandAll()
        {
            if (root != null && root is TreeFolder<T> tf)
            {
                tf.ExpandAll();
            }
        }

        private void ColapseAll()
        {
            if (root != null)
            {
                if (root is TreeFolder<T> tf)
                {
                    tf.ColapseAll();
                }
                root.SetSelected(true);
            }
        }

        private void SendCommand()
        {
            if (root != null)
            {
                var machines = root.GetSelectedTree(true);
                eventPublisher(new EventData(
                    EventType.OpenCommandSelector,
                    new ExtraCommandTargetList()
                    ));
            }
        }

        private void SendCommandPing()
        {
            if (root != null)
            {
                var machines = root.GetSelectedTree(false);
                eventPublisher(new EventData(
                    EventType.ExecuteCommand,
                    ))
            }
        }

        #endregion private

    }
}
