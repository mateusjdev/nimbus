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
        None,
        ClosePanel,
        OpenMachineTree,
        OpenCommandSelector,
        ExecuteCommand
    }

    internal interface IEventExtra;

    internal class Event
    {
        public EventType Type { get; }
        public IEventExtra? Extra { get; }
        public bool FlagRequestRender { get; }

        internal Event(EventType tipo, IEventExtra? extra = null, bool flagRequestRender = false)
        {
            Type = tipo;
            Extra = extra;
            FlagRequestRender = flagRequestRender;
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
