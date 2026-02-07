using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nimbus.Config
{
    internal abstract class Theme
    {
        internal static Color Primary = Color.Purple;
        internal static Color Highlight = Color.Blue;
        internal static Color BorderColor = Primary;
        internal static Color ControlsTextColor = Primary;
        internal static Color SelectedItemColor = Highlight;
        internal static BoxBorder BorderShape = BoxBorder.Rounded;
    }
}
