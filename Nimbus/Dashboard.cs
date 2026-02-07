using Nimbus.Event;
using Nimbus.Misc;
using Nimbus.Painel;
using Nimbus.Painel.SelectPrompt;
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
        private bool FlagRequestRender = false;

        private readonly LinkedList<PainelBase> painelStack = new();

        private readonly EventController eventController = new();

        private PainelBase PainelFocado
        {
            get { return painelStack.Last(); }
        }

        internal Dashboard()
        {
            eventController.OnEvent += OnEvent;
            eventController.OnInput += OnInput;
            var painelInicial = new PSelectPrompStart(eventController.EventPublisher);
            painelStack.AddLast(painelInicial);
        }

        private void OnInput(object? e, ConsoleKey key)
        {
            PainelFocado.HandleInput(key);
            FlagRequestRender = true;
        }

        private void OnEvent(object? e, EventData eventData)
        {
            switch (eventData.Type)
            {
                case EventType.GeometryChange:
                    {
                        FlagRequestRender = true;

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
                                        $"Tamanho da janela deve ser maior que 80x24! ({ConsoleWidth}x{ConsoleHeight})"
                                        );
                                }
                            }
                        }
                        else
                        {
                            FlagScreenSize = flagScreenSize;
                            if (flagScreenSize)
                            {
                                // Criar Alerta
                                var painelAlerta = new PainelAlerta(
                                    eventController.EventPublisher,
                                    TipoAlerta.Error,
                                    $"Tamanho da janela deve ser maior que 80x24! ({ConsoleWidth}x{ConsoleHeight})"
                                    );
                                painelStack.AddLast(painelAlerta);
                            }
                            else
                            {
                                // Remover Alerta
                                painelStack.RemoveLast();
                            }
                        }
                    }
                    break;
                case EventType.OpenMachineTree:
                    {
                        var novoPainel = new PainelMachinesTree(eventController.EventPublisher);
                        painelStack.AddLast(novoPainel);
                        FlagRequestRender = true;
                    }
                    break;
                case EventType.OpenCommandSelector:
                    {
                        if (eventData.Extra is not ExtraCommandTargetList)
                        {
                            throw new Exception("OpenCommandSelector: Extra is null");
                        }
                        /*
                        var selected = novoPainel.GetSelected();
                            var cmdExec = new CommandExecutor(selected, []);

                            foreach (var item in extra.Targets)
                            {
                                cmdExec.AddTarget(new CommandTarget { IP = item.IP, DomainName = item.Name });
                            }

                            _ = cmdExec.Execute();

                        var extra = (ExtraCommandTargetList)eventData.Extra;
                        extra.Targets
                        */
                        var novoPainel = new PSelectPromptCommand(eventController.EventPublisher, []);
                        painelStack.AddLast(novoPainel);
                        FlagRequestRender = true;
                    }
                    break;
                case EventType.ExecuteCommand:
                    break;
                case EventType.ClosePanel:
                    {
                        painelStack.RemoveLast();
                        if (painelStack.Count <= 0)
                        {
                            FlagExit = true;
                        }
                        FlagRequestRender = true;
                    }
                    break;
            }
        }

        private void Render()
        {
            AnsiConsole.Clear();
            var grid = CriarDashboard();
            AnsiConsole.Write(grid);
            AnsiConsole.Cursor.MoveLeft(10000);
        }

        private void CheckInput()
        {
            if (Console.KeyAvailable)
            {
                eventController.PublishInput(Console.ReadKey().Key);
            }
        }

        private void CheckGeometry()
        {
            var consoleWidth = AnsiConsole.Console.Profile.Width;
            var consoleHeight = AnsiConsole.Console.Profile.Height;

            // if (sizeUpdated)
            if (consoleWidth != ConsoleWidth || consoleHeight != ConsoleHeight)
            {
                ConsoleWidth = consoleWidth;
                ConsoleHeight = consoleHeight;

                eventController.PublishEvent(new EventData(EventType.GeometryChange));
            }
        }

        public void Start()
        {
            while (true)
            {
                CheckInput();
                CheckGeometry();

                if (FlagExit)
                {
                    AnsiConsole.Clear();
                    break;
                }

                if (FlagRequestRender)
                {
                    Render();
                    FlagRequestRender = false;
                }

                Thread.Sleep(Config.Config.UpdateIntervalMilliseconds);
            }
        }

        private Layout CriarDashboard()
        {
            int minSelecaoWidth = 30;

            var layout = new Layout("Root");
            var layoutContent = new Layout("Content");
            var layoutMain = new Layout("Main");

            var controls = PainelFocado.RenderControls();
            if (PainelFocado.RenderOptionFullScreen || painelStack.Count <= 1)
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
                var beforeLast = painelStack.Last?.Previous?.Value.Render();
                if (beforeLast != null)
                {
                    layout["History"].Update(beforeLast);
                }
            }

            var menuInicial = PainelFocado.Render();
            layout["Main"].Update(menuInicial);

            return layout;
        }
    }
}