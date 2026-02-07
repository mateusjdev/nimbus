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
    internal sealed class PSelectPromptCustom<T> : PSelectPromptBase<T> where T : Enum
    {
        #region private

        private Action? OnSelect;

        #endregion private

        #region public

        internal PSelectPromptCustom(EventPublisher ep, string panelName) : base(ep, panelName) { }

        internal new PSelectPromptCustom<T> AddOption(string text, T value, bool confirm = false)
        {
            options.Add(new PTreeOption { Text = text, Value = value, Confirm = confirm });
            return this;
        }

        internal new T? GetSelected()
        {
            if (options.Count == 0)
            {
                return default;
            }

            // TODO: Check bounds
            return options.ElementAt(OpSelecionada).Value;
        }

        internal void SetOnSelect(Action action)
        {
            OnSelect = action;
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
                    OnSelect?.Invoke();
                    eventPublisher(new EventData(EventType.RequestRender));
                    break;
                default:
                    base.HandleInput(key);
                    break;
            }
        }

        #endregion interface
    }
}
