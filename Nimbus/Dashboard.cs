using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nimbus
{
    internal class Dashboard
    {
        private const int MinWidth = 80;
        private const int MinHeight = 24;

        private int ConsoleWidth;
        private int ConsoleHeight;

        private bool FlagScreenSize = false;
        private bool FlagExit = false;
        private bool FlagRequestDraw = false;

        // private Views View = Views.Inicial;
        private LinkedList<Painel> painelStack = new();
        private Queue<Event> eventQueue = new();

        private Painel PainelFocado
        {
            get { return painelStack.Last(); }
        }

        internal Dashboard()
        {
            OnSizeUpdate();

            painelStack.AddLast(new PainelInicial());
        }

        private void OnSizeUpdate()
        {
            var consoleWidth = AnsiConsole.Console.Profile.Width;
            var consoleHeight = AnsiConsole.Console.Profile.Height;

            if (consoleWidth != ConsoleWidth || consoleHeight != ConsoleHeight)
            {
                FlagRequestDraw = true;
                ConsoleWidth = consoleWidth;
                ConsoleHeight = consoleHeight;
            }

            FlagScreenSize = ConsoleWidth < MinWidth || ConsoleHeight < MinHeight;
        }

        private void OnInput()
        {
            if (!Console.KeyAvailable)
            {
                return;
            }

            var key = Console.ReadKey().Key;

            var mEvent = PainelFocado.HandleInput(key);
            if (mEvent != Event.None)
            {
                eventQueue.Enqueue(mEvent);
            }

            FlagRequestDraw = true;
        }

        private void OnEvent()
        {
            if (eventQueue.Count <= 0)
            {
                return;
            }

            var mEvent = eventQueue.Dequeue();
            switch (mEvent)
            {
                case Event.OpenPing:
                    {
                        var painelPing = new PainelPing();
                        painelStack.AddLast(painelPing);
                        FlagRequestDraw = true;
                    }
                    break;
                case Event.ClosePanel:
                    {
                        painelStack.RemoveLast();
                        if (painelStack.Count <= 0)
                        {
                            FlagExit = true;
                        }
                    }
                    break;
            }
        }

        private void Render()
        {
            AnsiConsole.Clear();
            if (FlagScreenSize)
            {
                AnsiConsole.MarkupLineInterpolated($"[bold red]✗ Error:[/] Tamanho da janela deve ser maior que 80x24!");
                return;
            }

            var grid = CriarDashboard();
            AnsiConsole.Write(grid);
        }

        public void Start()
        {
            while (true)
            {
                OnSizeUpdate();
                OnInput();
                OnEvent();

                if (FlagExit)
                {
                    break;
                }

                if (FlagRequestDraw)
                {
                    Render();
                    FlagRequestDraw = false;
                }

                Thread.Sleep(200);
            }
        }

        private Layout CriarDashboard()
        {
            int minSelecaoWidth = 30;

            Layout layout;

            if (painelStack.Count > 1)
            {
                layout = new Layout("Root")
                    .SplitColumns(
                        new Layout("Sidebar").Ratio(2),
                        new Layout("Content").MinimumSize(minSelecaoWidth).Ratio(1)
                    );

                var beforeLast = painelStack.Last.Previous.Value.Render();
                var panelRecente = CriarPainel(beforeLast);
                layout["Content"].Update(panelRecente);
            }
            else
            {
                layout = new Layout("Root")
                    .SplitColumns(
                        new Layout("Sidebar")
                    );
            }

            var criarMenuInicial = PainelFocado.Render();
            var panelSelecao = CriarPainel(criarMenuInicial);
            layout["Sidebar"].Update(panelSelecao);

            return layout;
        }

        private static Panel CriarPainel(IRenderable content)
        {
            var panel = new Panel(content)
                .Header("Opções")
                .HeaderAlignment(Justify.Center)
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Purple)
                .Expand();
            return panel;
        }
    }
}