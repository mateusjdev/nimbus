using Nimbus.Painel.MachineTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nimbus.Misc
{
    internal enum EventType
    {
        ClosePanel,
        OpenPing,
        OpenMachineTree,
        OpenCommandPanel,
        SendCommand
    }

    internal interface EventExtra;

    internal class Event
    {
        public EventType Type { get; private set; }
        public EventExtra? Extra { get; private set; }

        internal Event(EventType tipo, EventExtra? extra = null)
        {
            Type = tipo;
            Extra = extra;
        }
    }

    internal class CommandTargetList : EventExtra
    {
        public MachineTreeElement[] Targets { get; private set; }

        internal CommandTargetList(MachineTreeElement[] targets)
        {
            Targets = targets;
        }
    }
}
