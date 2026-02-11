using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nimbus.Painel.Tree
{
    internal sealed class TreeFolder<T> : ITreeElementFolder<T> where T : ITreeElementDisplay
    {
        private bool isOpened = false;
        private int level;
        private int isSelected = -1;

        private readonly T Element;

        private readonly List<ITreeElement<T>> items = [];

        public bool HasChildren { get { return true; } }

        internal TreeFolder(T element, int level = 0)
        {
            Element = element;
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
                if (el is TreeFolder<T> tf)
                {
                    tf.Toggle();
                }
                else
                {
                    el.SetSelected(false);
                    isSelected = 0;
                    isOpened = !isOpened;
                }
            }
        }

        public void ExpandAll()
        {
            isOpened = true;

            foreach (var item in items)
            {
                if (item is TreeFolder<T> tf)
                {
                    tf.ExpandAll();
                }
            }
        }

        public void ColapseAll()
        {
            isOpened = false;

            foreach (var item in items)
            {
                if (item is TreeFolder<T> tf)
                {
                    tf.ColapseAll();
                }
            }

            if (isSelected > 0)
            {
                var el = items.ElementAt(isSelected - 1);
                el.SetSelected(false);
            }

            SetSelected(false);
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
                var ovflw = true;
                if (el1 is TreeFolder<T> tf1)
                {
                    ovflw = tf1.MoveSelectUp();
                }

                if (ovflw)
                {
                    el1.SetSelected(false);
                    isSelected--;
                    if (isSelected > 0)
                    {
                        var el2 = items.ElementAt(isSelected - 1);
                        el2.SetSelected(true);
                        if (el2 is TreeFolder<T> tf2)
                        {
                            tf2.MoveSelectUp();
                            tf2.MoveSelectUp();
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
                var el = items.ElementAt(isSelected - 1);
                var ovflw = true;
                if (el is TreeFolder<T> tf)
                {
                    ovflw = tf.MoveSelectDown();
                }

                if (ovflw)
                {
                    el.SetSelected(false);
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

        public void AddTreeItem(ITreeElement<T> item)
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

        public IRenderable[] RenderTreeElements()
        {
            List<IRenderable> renders = [Element.Render(isSelected == 0, level)];

            if (isOpened)
            {
                foreach (var item in items)
                {
                    renders.AddRange(item.RenderTreeElements());
                }
            }

            return [.. renders];
        }

        public void SetSelected(bool value)
        {
            isSelected = value ? 0 : -1;
        }

        public T[] GetSelectedTree(bool recurse)
        {
            List<T> mteList = [];
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
}
