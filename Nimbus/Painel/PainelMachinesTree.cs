using Nimbus.Misc;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

// root
// > -room_1(3) // { Type = Folder, Path = \\ad.room_1 }
//     - machine_1 // { Type = Machine, Path = \\ad.room_1.machine_1 }
//     - machine_2
//       - machine_3
//   - room_2(4)
//       - machine_1
//       - machine_2
//       - machine_3
//       - machine_4
//   + room_3(10)
//   + room_3(6)
// 
// -- -
// 
// Space = Toggle
// Enter = Connect ?
// e = edit
// p = ping

namespace Nimbus.Painel
{
    internal enum TreeItemType
    {
        Folder,
        Machine
    }

    interface ITreeItem
    {
        public string Path { get; }

        public string Name { get; }

        public IRenderable Render();

        public string TextRender();

        public void SetLevel(int level);

        public void SetSelected(bool value);
    }


    internal class TreeItemFolder : ITreeItem
    {
        private int level;
        private bool isSelected = false;
        private string name;

        public string Path { get; }
        public string Name { get { return name; } }
        public bool IsOpened { get; }
        private int Count { get { return items.Count; } }

        private List<ITreeItem> items = new();

        public TreeItemFolder(string name, string path, int level = 0)
        {
            this.name = name;
            Path = path;
            IsOpened = false;
            this.level = level;
        }

        public void AddTreeItem(ITreeItem item)
        {
            item.SetLevel(level + 1);
            items.Add(item);
        }

        public void SetLevel(int level)
        {
            this.level = level;
            foreach (var item in items)
            {
                item.SetLevel(level + 1);
            }
        }

        // TODO: Merge/Reuse TextRender and SplitRender
        public string TextRender()
        {
            StringBuilder str = new();
            if (isSelected)
            {
                str.Append("[blue]>");
            }
            else
            {
                str.Append(" ");
            }

            for (int n = level; n > 0; n--)
                str.Append(" ");
            str.Append(Name);

            if (isSelected)
            {
                str.Append("[/]");
            }

            str.AppendLine();

            foreach (var item in items)
            {
                str.Append(item.TextRender());
            }

            return str.ToString();
        }

        public IRenderable Render()
        {
            StringBuilder str = new();
            if (isSelected)
            {
                str.Append("[blue]>");
            }
            else
            {
                str.Append(" ");
            }

            for (int n = level; n > 0; n--)
                str.Append(" ");
            str.Append(Name);

            if (isSelected)
            {
                str.Append("[/]");
            }

            str.AppendLine();

            foreach (var item in items)
            {
                // Provavelmente erro
                str.Append(item.TextRender());
            }

            var text = new Markup(str.ToString())
                .Overflow(Overflow.Ellipsis);
            return text;
        }

        public void SetSelected(bool value)
        {
            isSelected = value;
            /*
            if (isSelected)
            {
                foreach (var item in items)
                {
                    item.SetSelected(false);
                }
            }
            */
        }
    }

    internal class TreeItemMachine : ITreeItem
    {
        private int level;
        private bool isSelected;
        private string name;

        public string Path { get; }
        public string Name { get { return name; } }

        public TreeItemMachine(string name, string path, int level = 0)
        {
            this.name = name;
            Path = path;
            this.level = level;
        }

        public void SetLevel(int level)
        {
            this.level = level;
        }

        // TODO: Merge/Reuse TextRender and SplitRender
        public string TextRender()
        {
            StringBuilder str = new();
            if (isSelected)
            {
                str.Append(">");
            }
            else
            {
                str.Append(" ");
            }

            for (int n = level; n > 0; n--)
                str.Append(" ");
            str.AppendLine(Name);

            return str.ToString();
        }

        public IRenderable Render()
        {
            StringBuilder str = new();
            if (isSelected)
            {
                str.Append(">");
            }
            else
            {
                str.Append(" ");
            }

            for (int n = level; n > 0; n--)
                str.Append(" ");
            str.AppendLine(Name);

            var text = new Markup(str.ToString())
                .Overflow(Overflow.Ellipsis);

            return text;
        }

        public void SetSelected(bool value)
        {
            isSelected = value;
        }
    }

    internal class PainelMachinesTree : IPainel
    {
        public bool RequestFullScreen { get { return false; } }

        public Event? HandleInput(ConsoleKey key)
        {
            Event? mEvent = null;
            switch (key)
            {
                case ConsoleKey.Escape:
                    mEvent = Event.ClosePanel;
                    break;
                case ConsoleKey.P:
                    mEvent = Event.OpenPing;
                    break;
            }
            return mEvent;
        }

        public IRenderable Render()
        {
            /*
            var tree = new Tree("Root");
            tree.Guide(TreeGuide.Line);

            var node1 = tree.AddNode("Child 1");
            node1.AddNode("Grandchild 1.1");
            node1.AddNode("Grandchild 1.2");

            tree.AddNode("Child 2");
            */

            var root = new TreeItemFolder("root", "/root/");

            var room1 = new TreeItemFolder("room1", "/root/room1/");

            room1.AddTreeItem(new TreeItemMachine("machine1_1", "/root/room1/machine1_1"));
            room1.AddTreeItem(new TreeItemMachine("machine1_2", "/root/room1/machine1_2"));
            room1.AddTreeItem(new TreeItemMachine("machine1_3", "/root/room1/machine1_3"));
            room1.AddTreeItem(new TreeItemMachine("machine1_4", "/root/room1/machine1_4"));
            room1.AddTreeItem(new TreeItemMachine("machine1_5", "/root/room1/machine1_5"));

            var room2 = new TreeItemFolder("room2", "/root/room2/");

            room2.AddTreeItem(new TreeItemMachine("machine2_1", "/root/room1/machine2_1"));
            room2.AddTreeItem(new TreeItemMachine("machine2_2", "/root/room1/machine2_2"));
            room2.AddTreeItem(new TreeItemMachine("machine2_3", "/root/room1/machine2_3"));

            var room3 = new TreeItemFolder("room3", "/root/room3/");

            room3.AddTreeItem(new TreeItemMachine("machine3_1", "/root/room1/machine3_1"));
            room3.AddTreeItem(new TreeItemMachine("machine3_2", "/root/room1/machine3_2"));
            room3.AddTreeItem(new TreeItemMachine("machine3_3", "/root/room1/machine3_3"));

            root.AddTreeItem(room1);
            root.AddTreeItem(room2);
            root.AddTreeItem(room3);

            root.SetSelected(true);

            var panel = new Panel(root.Render())
               .Header("Machine Tree")
               .HeaderAlignment(Justify.Center)
               .Border(BoxBorder.Rounded)
               .BorderColor(Color.Purple)
               .Expand();

            return panel;
        }

        public IRenderable? RenderControls()
        {
            return new Text("[Esc] Voltar [Espaço] Alternar [e] Editar [p] Ping", new Style(Color.Purple));
        }
    }
}
