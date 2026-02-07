using Spectre.Console;
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

    internal enum MachineTreeElementStatus
    {

        None, // No coloring
        Pending, // Yellow
        Ok, // Green
        Error, // Red
    }


    internal class MachineTreeElement : TreeDisplayWrapper
    {
        internal string Name { get; private set; }
        internal string Path { get; private set; }
        internal MachineTreeElementStatus Status { get; private set; }

        internal MachineTreeElement(string name, string path)
        {
            Name = name;
            Path = path;
            Status = MachineTreeElementStatus.Pending;
        }

        public string Display()
        {
            return Name;
        }
    }
}
