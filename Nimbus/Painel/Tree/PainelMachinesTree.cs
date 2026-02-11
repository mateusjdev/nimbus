using Nimbus.Config;
using Nimbus.Event;
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

// TODO: Improve tree ASCII characters
// root .
// ├─ item
// ├─ item
// ├─ item
// ├─ item
// └─ item

namespace Nimbus.Painel.Tree
{
    internal interface ITreeItemOld
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

        public void ToggleAll(bool value);

        public MachineTreeElement[] GetSelectedTree(bool recurse);
    }


    internal class TreeItemFolder : ITreeItemOld
    {
        private bool isOpened = false;
        private int level;
        private int isSelected = -1;

        private MachineTreeElement Mtd;

        public string Path { get { return Mtd.Path; } }
        public string Name { get { return Mtd.Name; } }
        public bool IsToggleable { get { return true; } }
        public bool IsOpened { get { return isOpened; } }
        public bool HasSelected { get { return isSelected >= 0; } }
        private int Count { get { return items.Count; } }

        private List<ITreeItemOld> items = new();

        public TreeItemFolder(MachineTreeElement mtd, int level = 0)
        {
            Mtd = mtd;
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

        public void ToggleAll(bool value)
        {
            isOpened = value;

            foreach (var item in items)
            {
                item.ToggleAll(value);
            }

            if (!value)
            {
                if (isSelected > 0)
                {
                    var el = items.ElementAt(isSelected - 1);
                    el.SetSelected(false);
                }

                SetSelected(false);
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

        public void AddTreeItem(ITreeItemOld item)
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

        public MachineTreeElement[] GetSelectedTree(bool recurse)
        {
            List<MachineTreeElement> mteList = [];
            if (recurse)
            {
                foreach (var item in items)
                {
                    mteList.AddRange(item.GetSelectedTree(true));
                }
            }
            else if (isSelected > 0)
            {
                var el = items.ElementAt(isSelected - 0);
                mteList.AddRange(el.GetSelectedTree(false));
            }

            return [.. mteList];
        }
    }

    internal class TreeItemMachine : ITreeItemOld
    {
        private int level;
        private bool isSelected;

        private MachineTreeElement Mtd;

        public string Path { get { return Mtd.Path; } }
        public string Name { get { return Mtd.Name; } }

        public bool IsToggleable { get { return false; } }

        public TreeItemMachine(MachineTreeElement mtd, int level = 0)
        {
            Mtd = mtd;
            this.level = level;
        }

        public void SetLevel(int level)
        {
            this.level = level;
        }

        public void Toggle() { }

        public void ToggleAll(bool value) { }

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

            switch (Mtd.Status)
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

            return [text];
        }

        public void SetSelected(bool value)
        {
            isSelected = value;
        }


        public MachineTreeElement[] GetSelectedTree(bool _)
        {
            return [Mtd];
        }
    }

    internal sealed class PainelMachinesTree : PainelBase
    {
        TreeItemFolder root;

        internal PainelMachinesTree(EventPublisher ep) : base(ep)
        {
            // MachineTreeDisplayFolder("root", "/root/");
            root = new TreeItemFolder(new MachineTreeElement("root", "/root/"));

            var room1 = new TreeItemFolder(new MachineTreeElement("room1", "/root/room1/"));

            var place1 = new TreeItemFolder(new MachineTreeElement("place1", "/root/room1/place1"));
            var place2 = new TreeItemFolder(new MachineTreeElement("place2", "/root/room1/place2"));

            room1.AddTreeItem(place1);
            room1.AddTreeItem(place2);

            place1.AddTreeItem(new TreeItemMachine(new MachineTreeElement("machine1_1", "/root/room1/place1/machine1_1")));
            place1.AddTreeItem(new TreeItemMachine(new MachineTreeElement("machine1_2", "/root/room1/place1/machine1_2")));
            place1.AddTreeItem(new TreeItemMachine(new MachineTreeElement("machine1_3", "/root/room1/place1/machine1_3")));

            place2.AddTreeItem(new TreeItemMachine(new MachineTreeElement("machine1_4", "/root/room1/place2/machine1_1")));
            place2.AddTreeItem(new TreeItemMachine(new MachineTreeElement("machine1_5", "/root/room1/place2/machine1_1")));

            var room2 = new TreeItemFolder(new MachineTreeElement("room2", "/root/room2/"));

            room2.AddTreeItem(new TreeItemMachine(new MachineTreeElement("machine2_1", "/root/room1/machine2_1")));
            room2.AddTreeItem(new TreeItemMachine(new MachineTreeElement("machine2_2", "/root/room1/machine2_2")));
            room2.AddTreeItem(new TreeItemMachine(new MachineTreeElement("machine2_3", "/root/room1/machine2_3")));

            var room3 = new TreeItemFolder(new MachineTreeElement("room3", "/root/room3/"));

            room3.AddTreeItem(new TreeItemMachine(new MachineTreeElement("machine3_1", "/root/room1/machine3_1")));
            room3.AddTreeItem(new TreeItemMachine(new MachineTreeElement("machine3_2", "/root/room1/machine3_2")));
            room3.AddTreeItem(new TreeItemMachine(new MachineTreeElement("machine3_3", "/root/room1/machine3_3")));

            root.AddTreeItem(room1);
            root.AddTreeItem(room2);
            root.AddTreeItem(room3);

            root.SetSelected(true);
        }

        internal override void HandleInput(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.Escape:
                    eventPublisher(new EventData(EventType.ClosePanel));
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
                case ConsoleKey.A:
                    root.ToggleAll(true);
                    break;
                case ConsoleKey.F:
                    root.ToggleAll(false);
                    root.SetSelected(true);
                    break;
                case ConsoleKey.C:
                    var machines = root.GetSelectedTree(true);
                    eventPublisher(new EventData(
                        EventType.OpenCommandSelector,
                        new ExtraCommandTargetList()
                        ));
                    break;
                case ConsoleKey.E:
                    break;
                case ConsoleKey.P:
                    var machines = root.GetSelectedTree(false);
                    eventPublisher(new EventData(
                        EventType.ExecuteCommand,
                        new ExtraCommandTargetList()
                        ));
                    break;
            }
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

        internal override IRenderable Render()
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

        internal override IRenderable? RenderControls()
        {
            StringBuilder str = new();
            str.Append("[Esc] Voltar ");
            str.Append("[Espaço] Alternar ");
            str.Append("[e] Editar ");
            str.Append("[p] Ping ");
            str.Append("[c] Comandos ");
            str.Append("[A] Abrir ");
            str.Append("[F] Fechar ");

            return new Text(str.ToString(), new Style(Color.Purple));
        }
    }
}
