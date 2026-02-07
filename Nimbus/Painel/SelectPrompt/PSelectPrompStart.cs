using Nimbus.Event;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Nimbus.Painel.SelectPrompt
{
    enum PainelInicialOpcoes
    {
        Ping,
        MachineTree,
        Config,
        About,
        Option1,
        Option2,
        Option3,
    }

    internal class PSelectPrompStart : PSelectPromptBase<PainelInicialOpcoes>
    {
        private const string _PanelName = "Opções";

        internal PSelectPrompStart(EventPublisher ep) : base(ep, _PanelName)
        {
            AddOption("Ping", PainelInicialOpcoes.Ping);
            AddOption("Árvore de Maquinas", PainelInicialOpcoes.MachineTree);
            AddOption("Config", PainelInicialOpcoes.Config);
            AddOption("Sobre", PainelInicialOpcoes.About);
            AddOption("Opção 1", PainelInicialOpcoes.Option1);
            AddOption("Opção 2", PainelInicialOpcoes.Option2);
            AddOption("Opção 3", PainelInicialOpcoes.Option3);
        }

        internal override void HandleInput(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.Enter:
                    switch (GetSelected())
                    {
                        case PainelInicialOpcoes.Ping:
                            // TODO: Replace with another screen
                            eventPublisher(new EventData(EventType.OpenMachineTree));
                            break;
                        case PainelInicialOpcoes.MachineTree:
                            eventPublisher(new EventData(EventType.OpenMachineTree));
                            break;
                    }
                    break;
                default:
                    base.HandleInput(key);
                    break;
            }
        }
    }
}
