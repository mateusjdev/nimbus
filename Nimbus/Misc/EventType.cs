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
        OpenMachineTree,
        OpenCommandSelector,
        ExecuteCommand
    }

    internal interface IEventExtra;

    internal class Event
    {
        public EventType Type { get; private set; }
        public IEventExtra? Extra { get; private set; }

        internal Event(EventType tipo, IEventExtra? extra = null)
        {
            Type = tipo;
            Extra = extra;
        }
    }

    internal class ExtraCommandTargetList : IEventExtra
    {
        public MachineTreeElement[] Targets { get; private set; }

        internal ExtraCommandTargetList(MachineTreeElement[] targets)
        {
            Targets = targets;
        }
    }
}
