using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nimbus.Painel.MachineTree
{
    internal class MachineTree
    {
    }

    internal interface TreeDisplayWrapper
    {
        public string Display();
    }

    internal class MachineTreeDisplayFolder : TreeDisplayWrapper
    {
        internal string Name { get; private set; }
        internal string Path { get; private set; }

        public string Display()
        {
            return Name;
        }
    }

    internal class MachineTreeDisplayItem : TreeDisplayWrapper
    {
        internal string Name { get; private set; }
        internal string Path { get; private set; }

        public string Display()
        {
            return Name;
        }
    }
}
