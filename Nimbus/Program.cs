using Spectre.Console;
using Spectre.Console.Rendering;
using System;

namespace Nimbus
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var dashboard = new Dashboard();
            dashboard.Start();
        }
    }
}
