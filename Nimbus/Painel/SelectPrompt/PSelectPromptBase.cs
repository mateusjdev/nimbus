using Nimbus.Config;
using Nimbus.Event;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nimbus.Painel.SelectPrompt
{

    internal abstract class PSelectPromptBase<T> : PainelBase where T : Enum
    {
        #region private

        protected struct PTreeOption
        {
            internal string Text;
            internal T Value;
            internal bool Confirm;
        }

        protected readonly List<PTreeOption> options = [];

        protected int OpSelecionada = 0;

        protected int OpCount { get { return options.Count; } }

        #endregion private

        #region public

        internal PSelectPromptBase(EventPublisher ep, string panelName) : base(ep, panelName) { }

        protected T? GetSelected()
        {
            if (options.Count == 0)
            {
                return default;
            }

            // TODO: Check bounds
            return options.ElementAt(OpSelecionada).Value;
        }

        protected void AddOption(string text, T value, bool confirm = false)
        {
            options.Add(new PTreeOption { Text = text, Value = value, Confirm = confirm });
        }

        #endregion public

        #region interface

        internal override void HandleInput(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    OpSelecionada--;
                    if (OpSelecionada < 0)
                    {
                        OpSelecionada = OpCount;
                    }
                    eventPublisher(new EventData(EventType.RequestRender));
                    break;
                case ConsoleKey.DownArrow:
                    OpSelecionada++;
                    if (OpSelecionada > OpCount)
                    {
                        OpSelecionada = 0;
                    }
                    eventPublisher(new EventData(EventType.RequestRender));
                    break;
                case ConsoleKey.Enter:
                    // TODO: Prompt Confirm
                    // if (options.ElementAt(OpSelecionada).Confirm)
                    eventPublisher(new EventData(EventType.RequestRender));
                    break;
                case ConsoleKey.Escape:
                    eventPublisher(new EventData(EventType.ClosePanel));
                    eventPublisher(new EventData(EventType.RequestRender));
                    break;
            }
        }

        internal override IRenderable Render()
        {
            List<Text> rows = [];
            var selectedStyle = new Style(foreground: Theme.SelectedItemColor);

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
                .Header(PanelName)
                .HeaderAlignment(Justify.Center)
                .Border(Theme.BorderShape)
                .BorderColor(Theme.BorderColor)
                .Expand();

            return panel;
        }

        internal override IRenderable RenderControls()
        {
            return new Text("[Esc] Voltar [Enter] Selecionar", new Style(Theme.ControlsTextColor));
        }

        #endregion interface
    }
}
