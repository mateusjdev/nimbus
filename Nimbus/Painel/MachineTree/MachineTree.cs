using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        internal IPAddress IP { get; private set; }
        internal string Name { get; private set; }
        internal string Path { get; private set; }
        internal MachineTreeElementStatus Status { get; private set; }

        internal MachineTreeElement(IPAddress ip, string name, string path)
        {
            IP = ip;
            Name = name;
            Path = path;
            Status = MachineTreeElementStatus.Pending;
        }

        // TODO: Disable constructor
        internal MachineTreeElement(string name, string path)
        {
            IP = new IPAddress(16843009); // tmp 1.1.1.1
            Name = name;
            Path = path;
            Status = MachineTreeElementStatus.Pending;
        }

        // TODO: Create a Comom Element and a Machine Element

        // TODO: For machines, use fqdn or ip address

        // ComputerName: mypc
        // DomainName: mypc
        // FQDN: ComputerName.DomainName - mypc.domain.net


        public string Display()
        {
            return Name;
        }
    }
}
