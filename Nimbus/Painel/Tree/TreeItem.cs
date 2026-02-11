using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nimbus.Painel.Tree
{
    internal sealed class TreeItem<T> : ITreeElementItem<T> where T : ITreeElementDisplay
    {
        private int level;
        private bool isSelected;

        private T Element;

        public TreeItem(T element, int level = 0)
        {
            Element = element;
            this.level = level;
        }

        public void SetLevel(int level)
        {
            this.level = level;
        }

        public IRenderable[] RenderTreeElements()
        {
            return [Element.Render(isSelected, level)];
        }

        public void SetSelected(bool value)
        {
            isSelected = value;
        }

        public T[] GetSelectedTree(bool _)
        {
            return [Element];
        }
    }
}
