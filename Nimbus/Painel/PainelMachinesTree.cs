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
    // TODO: Improve tree ASCII characters
    // root .
    // ├─ item
    // ├─ item
    // ├─ item
    // ├─ item
    // └─ item
    internal abstract class Icons
    {
        internal const string LevelSpacing = "| ";
        // https://www.compart.com/en/unicode/block/U+25A0
        internal const string ItemFolderOpened = "▼";
        internal const string ItemFolderClosed = "▶";
        internal const string Item = "◇";
        internal const string Selected = "»";
    }

    internal enum TreeItemType
    {
        Folder,
        Machine
    }

    internal interface ITreeItem
    {
        public string Path { get; }

        public bool IsToggleable { get; }

        public string Name { get; }

        public IRenderable[] RenderTree();

        public void SetLevel(int level);

        public void SetSelected(bool value);

        public bool MoveSelectUp();

        public bool MoveSelectDown();

        public void Toggle();
    }


    internal class TreeItemFolder : ITreeItem
    {
        private bool isOpened = false;
        private int level;
        private int isSelected = -1;
        private string name;

        public string Path { get; }
        public string Name { get { return name; } }
        public bool IsToggleable { get { return true; } }
        public bool IsOpened { get; }
        public bool IsSelected { get { return isSelected >= 0; } }
        private int Count { get { return items.Count; } }

        private List<ITreeItem> items = new();

        public TreeItemFolder(string name, string path, int level = 0)
        {
            this.name = name;
            Path = path;
            IsOpened = false;
            this.level = level;
        }

        public void Toggle()
        {
            if (isSelected == 0)
            {
                isOpened = !isOpened;
            }
            else if (isSelected > 0)
            {
                var el = items.ElementAt(isSelected - 1);
                if (el.IsToggleable)
                {
                    el.Toggle();
                }
                else
                {
                    el.SetSelected(false);
                    isSelected = 0;
                    isOpened = !isOpened;
                }
            }
        }

        // FIX: select last item on subtree:
        // Folder
        //   Folder
        //     Folder 
        //       Item <- to this
        //   Folder <- from this
        public bool MoveSelectUp()
        {
            if (!isOpened)
            {
                return true;
            }

            bool overflow = false;
            if (isSelected < 0)
            {
                isSelected = items.Count;
                var el = items.ElementAt(isSelected - 1);
                el.SetSelected(true);
                overflow = true;
            }
            else if (isSelected == 0)
            {
                isSelected--;
                overflow = true;
            }
            else if (isSelected > 0)
            {
                var el1 = items.ElementAt(isSelected - 1);
                var ovflw = el1.MoveSelectUp();
                if (ovflw)
                {
                    el1.SetSelected(false);
                    isSelected--;
                    if (isSelected > 0)
                    {
                        var el = items.ElementAt(isSelected - 1);
                        el.SetSelected(true);
                        el.MoveSelectUp();
                        if (el is TreeItemFolder)
                        {
                            el.MoveSelectUp();
                        }
                    }
                }
            }
            return overflow;
        }

        public bool MoveSelectDown()
        {
            if (!isOpened)
            {
                return true;
            }

            bool overflow = false;
            if (isSelected < 0)
            {
                overflow = true;
            }
            else if (isSelected == 0)
            {
                isSelected++;
                items.ElementAt(0).SetSelected(true);
            }
            else if (isSelected > 0)
            {
                var ovflw = items.ElementAt(isSelected - 1).MoveSelectDown();
                if (ovflw)
                {
                    items.ElementAt(isSelected - 1).SetSelected(false);
                    isSelected++;
                    if (isSelected > items.Count)
                    {
                        isSelected = -1;
                        overflow = true;
                    }
                    else
                    {
                        items.ElementAt(isSelected - 1).SetSelected(true);
                    }
                }
            }
            return overflow;
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
        private IRenderable Render()
        {
            StringBuilder str = new();
            str.Append(isSelected == 0 ? $"[blue]{Icons.Selected} " : "  ");

            for (int n = level; n > 0; n--)
                str.Append(Icons.LevelSpacing);

            str.Append(isOpened ? $"{Icons.ItemFolderOpened} " : $"{Icons.ItemFolderClosed} ");
            str.Append(isSelected == 0 ? $"{Name}[/]" : $"{Name}");

            var text = new Markup(str.ToString())
                .Overflow(Overflow.Ellipsis);

            return text;
        }

        public IRenderable[] RenderTree()
        {
            List<IRenderable> renders = [Render()];

            if (isOpened)
            {
                foreach (var item in items)
                {
                    renders.AddRange(item.RenderTree());
                }
            }

            return [.. renders];
        }

        public void SetSelected(bool value)
        {
            isSelected = value ? 0 : -1;
        }
    }

    internal class TreeItemMachine : ITreeItem
    {
        private int level;
        private bool isSelected;
        private string name;

        public string Path { get; }

        public bool IsToggleable { get { return false; } }

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

        public void Toggle() { }

        public bool MoveSelectUp()
        {
            return true;
        }

        public bool MoveSelectDown()
        {
            return true;
        }

        public IRenderable[] RenderTree()
        {
            StringBuilder str = new();
            str.Append(isSelected ? $"[blue]{Icons.Selected} " : "  ");

            for (int n = level; n > 0; n--)
                str.Append(Icons.LevelSpacing);

            if (isSelected)
            {
                str.Append("[/]");
            }

            str.Append($"[yellow]{Icons.Item}");
            /*
            Random r = new();
            if (r.Next(0, 2) == 0)
            {
                str.Append($"[green]{Icons.Item}");
            }
            else
            {
                str.Append($"[red]{Icons.Item}");
            }
            */

            if (isSelected)
            {
                str.Append("[/][blue] ");
            }
            else
            {
                str.Append("[/] ");
            }

            str.Append(isSelected ? $"{Name}[/]" : $"{Name}");

            var text = new Markup(str.ToString())
                .Overflow(Overflow.Ellipsis);

            return [text];
        }

        public void SetSelected(bool value)
        {
            isSelected = value;
        }
    }

    internal class PainelMachinesTree : IPainel
    {
        TreeItemFolder root;

        internal PainelMachinesTree()
        {
            root = new TreeItemFolder("root", "/root/");

            var room1 = new TreeItemFolder("room1", "/root/room1/");

            var place1 = new TreeItemFolder("place1", "/root/room1/place1");
            var place2 = new TreeItemFolder("place2", "/root/room1/place2");

            room1.AddTreeItem(place1);
            room1.AddTreeItem(place2);

            place1.AddTreeItem(new TreeItemMachine("machine1_1", "/root/room1/place1/machine1_1"));
            place1.AddTreeItem(new TreeItemMachine("machine1_2", "/root/room1/place1/machine1_2"));
            place1.AddTreeItem(new TreeItemMachine("machine1_3", "/root/room1/place1/machine1_3"));
            place2.AddTreeItem(new TreeItemMachine("machine1_4", "/root/room1/place2/machine1_1"));
            place2.AddTreeItem(new TreeItemMachine("machine1_5", "/root/room1/place2/machine1_1"));

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
        }

        public bool RequestFullScreen { get { return false; } }

        public Event? HandleInput(ConsoleKey key)
        {
            Event? mEvent = null;
            switch (key)
            {
                case ConsoleKey.Escape:
                    mEvent = Event.ClosePanel;
                    break;
                case ConsoleKey.UpArrow:
                    MoveSelectUp();
                    break;
                case ConsoleKey.DownArrow:
                    MoveSelectDown();
                    break;
                case ConsoleKey.Enter:
                    break;
                case ConsoleKey.Spacebar:
                    root.Toggle();
                    break;
                case ConsoleKey.E:
                    break;
                case ConsoleKey.P:
                    mEvent = Event.OpenPing;
                    break;
            }
            return mEvent;
        }

        private void MoveSelectUp()
        {
            if (root == null)
            {
                return;
            }

            var overflow = root.MoveSelectUp();
            if (overflow)
            {
                root.SetSelected(true);
            }
        }

        private void MoveSelectDown()
        {
            if (root == null)
            {
                return;
            }

            var overflow = root.MoveSelectDown();
            if (overflow)
            {
                root.SetSelected(true);
            }
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

            var layout = new Rows(root.RenderTree());
            var panel = new Panel(layout)
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
