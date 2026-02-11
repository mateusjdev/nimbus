using Nimbus.Config;
using Nimbus.Event;
using Nimbus.Misc;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nimbus.Painel.SelectPrompt
{
    internal sealed class PSelectPromptCommand : PSelectPromptBase<CommandType>
    {
        private const string _PanelName = "Seletor de Comando";
        private readonly Computer[] Targets;

        internal PSelectPromptCommand(EventPublisher ep, Computer[] targets) : base(ep, _PanelName)
        {
            Targets = targets;

            AddOption("Desligar", CommandType.Shutdown);
            AddOption("Reiniciar", CommandType.Reboot);
            AddOption("Acordar via RDP", CommandType.WakeUp);
            AddOption("Ping", CommandType.Ping);
            AddOption("Mensagem", CommandType.Message);
            AddOption("Customizado", CommandType.Shell);
        }

        internal override void HandleInput(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.Enter:
                    // TODO: Validate values (Ensure Non Integers
                    // Confirm
                    // Open Comand Executor
                    // OnSelect?.Invoke();
                    eventPublisher(new EventData(EventType.ExecuteCommand, new ExtraCommandTargetList(Targets)));
                    break;
                default:
                    base.HandleInput(key);
                    break;
            }
        }
    }
}
