using Nimbus.Config;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nimbus.Painel.Tree
{
    interface ITreeElement<T> where T : ITreeElementDisplay
    {
        // public string Path { get; }

        public IRenderable[] RenderTreeElements();

        public void SetLevel(int level);

        public void SetSelected(bool value);

        public T[] GetSelectedTree(bool recurse);
    }

    interface ITreeElementFolder<T> : ITreeElement<T> where T : ITreeElementDisplay
    {
        public bool MoveSelectUp();

        public bool MoveSelectDown();

        public void Toggle();

        public void ExpandAll();

        public void ColapseAll();
    }

    interface ITreeElementItem<T> : ITreeElement<T> where T : ITreeElementDisplay { }
}
