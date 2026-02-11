using Nimbus.Config;
using Nimbus.Misc;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Nimbus.Painel.Tree
{
    internal interface ITreeElementDisplay
    {
        public IRenderable Render(bool isSelected, int level, bool isExpanded);

        public Machine GetElement();
    }

    internal sealed class TreeDisplayDirectory : ITreeElementDisplay
    {
        private string Name;

        internal TreeDisplayDirectory(string name)
        {
            Name = name;
        }

        public IRenderable Render(bool isSelected, int level, bool isExpanded)
        {
            StringBuilder str = new();
            str.Append(isSelected ? $"[blue]{Icons.Selected} " : "  ");

            for (int n = level; n > 0; n--)
                str.Append(Icons.LevelSpacing);

            str.Append(isExpanded ? $"{Icons.ItemFolderOpened} " : $"{Icons.ItemFolderClosed} ");
            str.Append(isSelected ? $"{Name}[/]" : $"{Name}");

            var text = new Markup(str.ToString())
                .Overflow(Overflow.Ellipsis);

            return text;
        }
    }

    internal sealed class TreeDisplayComputer : ITreeElementDisplay
    {
        private MachineTreeElementStatus Status = MachineTreeElementStatus.Pending;
        private readonly Computer Computer;
        private string? Name { get { return Computer.Name; } }

        internal TreeDisplayComputer(Computer computer)
        {
            Computer = computer;
            GetStatus().ContinueWith((t) =>
            {
                var result = t.Result;
                if (result == 0)
                {
                    Status = MachineTreeElementStatus.Ok;
                }
                else
                {
                    Status = MachineTreeElementStatus.Error;
                }
            });
        }

        public IRenderable Render(bool isSelected, int level, bool _)
        {
            StringBuilder str = new();
            str.Append(isSelected ? $"[blue]{Icons.Selected} " : "  ");

            for (int n = level; n > 0; n--)
                str.Append(Icons.LevelSpacing);

            if (isSelected)
            {
                str.Append("[/]");
            }

            switch (Status)
            {
                case MachineTreeElementStatus.None:
                    str.Append($"[blue]{Icons.Item}");
                    break;
                case MachineTreeElementStatus.Pending:
                    str.Append($"[yellow]{Icons.Item}");
                    break;
                case MachineTreeElementStatus.Ok:
                    str.Append($"[green]{Icons.Item}");
                    break;
                case MachineTreeElementStatus.Error:
                    str.Append($"[red]{Icons.Item}");
                    break;
            }

            str.Append(isSelected ? $"[/][blue] {Name}[/]" : $"[/] {Name}");

            var text = new Markup(str.ToString())
                .Overflow(Overflow.Ellipsis);

            return text;
        }

        private async Task<int> GetStatus()
        {
            return await Task.Run(() =>
            {
                int result = -1;
                // TODO: Ping Computer
                if (Computer.IpAddress != null)
                {
                    var ping = new Ping();
                    var resp = ping.Send(Computer.IpAddress);
                    result = (int)resp.Status;
                }
                return result;
            });
        }
    }
}