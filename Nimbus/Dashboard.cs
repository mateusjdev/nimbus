using Nimbus.Misc;
using Nimbus.Painel;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
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

        private LinkedList<IPainel> painelStack = new();
        private Queue<Event> eventQueue = new();

        private const int UpdateIntervalMilisseconds = 100;

        private IPainel PainelFocado
        {
            get { return painelStack.Last(); }
        }

        internal Dashboard()
        {
            painelStack.AddLast(new PainelInicial());
        }

        private void OnSizeUpdate()
        {
            var consoleWidth = AnsiConsole.Console.Profile.Width;
            var consoleHeight = AnsiConsole.Console.Profile.Height;

            var sizeUpdated = consoleWidth != ConsoleWidth || consoleHeight != ConsoleHeight;
            if (!sizeUpdated)
            {
                return;
            }

            FlagRequestDraw = true;
            ConsoleWidth = consoleWidth;
            ConsoleHeight = consoleHeight;

            var flagScreenSize = ConsoleWidth < MinWidth || ConsoleHeight < MinHeight;
            var flagScreenSizeToggled = flagScreenSize != FlagScreenSize;
            if (!flagScreenSizeToggled)
            {
                if (FlagScreenSize)
                {
                    var alerta = PainelFocado;
                    if (alerta is PainelAlerta)
                    {
                        ((PainelAlerta)alerta).ChangeText(
                            $"Tamanho da janela deve ser maior que 80x24! ({consoleWidth}x{consoleHeight})"
                            );
                    }
                }

                return;
            }

            FlagScreenSize = flagScreenSize;
            if (flagScreenSize)
            {
                // Criar Alerta

                var painelAlerta = new PainelAlerta(
                    TipoAlerta.Error,
                    $"Tamanho da janela deve ser maior que 80x24! ({consoleWidth}x{consoleHeight})"
                    );
                painelStack.AddLast(painelAlerta);
            }
            else
            {
                // Remover Alerta
                painelStack.RemoveLast();
            }

        }

        private void OnInput()
        {
            if (!Console.KeyAvailable)
            {
                return;
            }

            var key = Console.ReadKey().Key;

            var mEvent = PainelFocado.HandleInput(key);
            if (mEvent != null)
            {
                eventQueue.Enqueue(mEvent);
            }

            FlagRequestDraw = true;
        }

        private void OnEvent()
        {
            while (eventQueue.Count > 0)
            {
                var mEvent = eventQueue.Dequeue();
                switch (mEvent.Type)
                {
                    case EventType.OpenMachineTree:
                        {
                            var novoPainel = new PainelMachinesTree();
                            painelStack.AddLast(novoPainel);
                            FlagRequestDraw = true;
                        }
                        break;
                    case EventType.OpenCommandSelector:
                        {
                            var novoPainel = new PainelCommandSelector();
                            painelStack.AddLast(novoPainel);
                            FlagRequestDraw = true;
                        }
                        break;
                    case EventType.ClosePanel:
                        {
                            painelStack.RemoveLast();
                            if (painelStack.Count <= 0)
                            {
                                FlagExit = true;
                            }
                            FlagRequestDraw = true;
                        }
                        break;
                }
            }
        }

        private void Render()
        {
            AnsiConsole.Clear();
            var grid = CriarDashboard();
            AnsiConsole.Write(grid);
            AnsiConsole.Cursor.MoveLeft(10000);
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
                    AnsiConsole.Clear();
                    break;
                }

                if (FlagRequestDraw)
                {
                    Render();
                    FlagRequestDraw = false;
                }

                Thread.Sleep(UpdateIntervalMilisseconds);
            }
        }

        private Layout CriarDashboard()
        {
            int minSelecaoWidth = 30;

            Layout layout = new Layout("Root");
            Layout layoutContent = new Layout("Content");
            Layout layoutMain = new Layout("Main");

            var controls = PainelFocado.RenderControls();
            if (PainelFocado.RequestFullScreen || painelStack.Count <= 1)
            {
                if (controls != null)
                {
                    layout = layout.SplitRows(
                        layoutContent.SplitColumns(layoutMain),
                        new Layout("Controls").Size(1)
                    );

                    layout["Controls"].Update(controls);
                }
                else
                {
                    layout = layout.SplitRows(
                        layoutContent.SplitColumns(layoutMain)
                    );
                }
            }
            else
            {
                if (controls != null)
                {

                    layout = layout.SplitRows(
                        layoutContent.SplitColumns(
                            layoutMain.Ratio(2),
                            new Layout("History").MinimumSize(minSelecaoWidth).Ratio(1)
                        ),
                        new Layout("Controls").Size(1)
                    );

                    layout["Controls"].Update(controls);
                }
                else
                {
                    layout = layout.SplitRows(
                        layoutContent.SplitColumns(
                            layoutMain.Ratio(2),
                            new Layout("History").MinimumSize(minSelecaoWidth).Ratio(1)
                        )
                    );
                }

                // TODO: Fix possible nullable
                var beforeLast = painelStack.Last.Previous.Value.Render();
                layout["History"].Update(beforeLast);
            }

            var menuInicial = PainelFocado.Render();
            layout["Main"].Update(menuInicial);

            return layout;
        }
    }
}