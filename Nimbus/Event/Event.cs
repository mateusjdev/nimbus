using Nimbus.Misc;
using Nimbus.Painel.MachineTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nimbus.Event
{
    internal enum EventType
    {
        ClosePanel,
        OpenMachineTree,
        OpenCommandSelector,
        ExecuteCommand,
        RequestRender,
        GeometryChange
    }

    internal delegate void EventPublisher(EventData ed);
    internal delegate void InputPublisher(ConsoleKey ed);

    internal class EventController
    {
        public event EventHandler<EventData>? OnEvent;
        public event EventHandler<ConsoleKey>? OnInput;

        internal EventPublisher EventPublisher { get { return PublishEvent; } }

        internal InputPublisher InputPublisher { get { return PublishInput; } }

        internal void PublishEvent(EventData e)
        {
            OnEvent?.Invoke(this, e);
        }

        internal void PublishInput(ConsoleKey e)
        {
            OnInput?.Invoke(this, e);
        }
    }

    internal interface IEventExtra;

    internal class EventData : EventArgs
    {
        public EventType Type { get; }
        public IEventExtra? Extra { get; }
        public bool FlagRequestRender { get; }

        internal EventData(EventType tipo, IEventExtra? extra = null, bool flagRequestRender = false)
        {
            Type = tipo;
            Extra = extra;
            FlagRequestRender = flagRequestRender;
        }
    }

    internal class ExtraCommandTargetList : IEventExtra
    {
        public Machine[] Targets { get; private set; }

        internal ExtraCommandTargetList(Machine[] targets)
        {
            Targets = targets;
        }
    }
}
