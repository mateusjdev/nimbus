using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nimbus.Painel.Tree
{
    internal interface ITree<T> where T: ITreeElementDisplay
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

        public ITreeElement<T>[] GetSelectedTree(bool recurse);
    }
}
