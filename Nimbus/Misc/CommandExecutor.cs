using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nimbus.Misc
{
    internal class CommandExecutor
    {
    }

    internal enum CommandType: int
    {
        Shutdown, // Desligar
        Reboot, // Reiniciar
        WakeUp, // Acordar vie RDP (TCP - Porta 3389)
        Shell, // cmd or powershell command

        // TODO: PainelPing -> PainelCommand
        Ping, // Ping
    }

    internal struct CommandTarget
    {
        // TODO: IPv6 support
        internal UInt32 IP;
        internal string DomainName;
    }

    internal class CommandEvent : Event
    {
        internal CommandType CommandType { get; private set; }

        internal CommandTarget[] CommandTargets { get; private set; }

        internal CommandEvent(
            CommandType tipo,
            CommandTarget[] targets
            ) : base(EventType.SendCommand)
        {
            CommandType = tipo;
            CommandTargets = targets;
        }
    }
}
