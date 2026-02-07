using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nimbus.Misc
{
    internal enum CommandType : int
    {
        Shutdown, // Desligar
        Reboot, // Reiniciar
        WakeUp, // Acordar vie RDP (TCP - Porta 3389)
        Shell, // cmd or powershell command

        // TODO: PainelPing -> PainelCommand
        Ping, // Ping
    }

    internal enum CommandStatus
    {
        Waiting,
        Running,
        Finished
    }

    internal struct CommandTarget
    {
        // TODO: IPv6 support
        internal UInt32 IP;
        internal string DomainName;
    }

    internal class CommandExecutor
    {
        private CommandType commandType;

        private List<CommandTarget> commandTargets = new();

        public bool HasDisplay { get; private set; }

        public CommandStatus Status { get; private set; }

        internal CommandExecutor(CommandType tipo, CommandTarget[] targets)
        {
            commandType = tipo;
            commandTargets.AddRange(targets);
            Status = CommandStatus.Waiting;

            /*
            if (targets.Length > 1)
            {
                HasDisplay = false;
            }
            */

            // TODO: switch
            if (tipo == CommandType.Ping)
            {
                HasDisplay = true;
            }
        }

        internal void AddTarget(CommandTarget target)
        {
            if (Status != CommandStatus.Waiting)
            {
                throw new Exception("CommandExecutor: Command already running");
            }

            commandTargets.Add(target);
        }

        // TODO: Task/async
        public async Task<int[]> Execute()
        {
            var count = commandTargets.Count;
            if (count == 0)
            {
                throw new Exception("CommandExecutor: no targets");
            }

            Status = CommandStatus.Running;

            List<Task<int>> tasks = [];
            switch (commandType)
            {
                case CommandType.Shutdown:
                    for (int i = 0; i < count; i++)
                    {
                        tasks.Add(Shutdown(commandTargets[i].IP));
                    }
                    break;
                case CommandType.Reboot:
                    for (int i = 0; i < count; i++)
                    {
                        tasks.Add(Reboot(commandTargets[i].IP));
                    }
                    break;
                case CommandType.WakeUp:
                    for (int i = 0; i < count; i++)
                    {
                        tasks.Add(WakeUp(commandTargets[i].IP));
                    }
                    break;
                case CommandType.Shell:
                    for (int i = 0; i < count; i++)
                    {
                        tasks.Add(Shell(commandTargets[i].IP));
                    }
                    break;
                case CommandType.Ping:
                    for (int i = 0; i < count; i++)
                    {
                        tasks.Add(Ping(commandTargets[i].IP));
                    }
                    break;
            }

            var tasksResults = await Task.WhenAll(tasks);

            Status = CommandStatus.Finished;

            return tasksResults;
        }

        private async Task<int> Shutdown(UInt32 ip)
        {
            await Task.Delay(1000);
            return 0;
        }

        private async Task<int> Reboot(UInt32 ip)
        {
            await Task.Delay(1000);
            return 0;
        }

        private async Task<int> WakeUp(UInt32 ip)
        {
            await Task.Delay(1000);
            return 0;
        }

        private async Task<int> Shell(UInt32 ip)
        {
            await Task.Delay(1000);
            return 0;
        }

        private async Task<int> Ping(UInt32 ip)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                // /c tells cmd to run the command and then terminate
                Arguments = $"/c msg %username% ${ip}",
                UseShellExecute = false,
                CreateNoWindow = true // Runs it hidden in the background
            };

            var process = Process.Start(startInfo);

            if (process == null)
            {
                return -1;
            }

            await process.WaitForExitAsync();
            return process.ExitCode;
        }

        private async Task<int> Message(UInt32 ip, string message)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                // /c tells cmd to run the command and then terminate
                Arguments = $"/c msg %username% ${ip}",
                UseShellExecute = false,
                CreateNoWindow = true // Runs it hidden in the background
            };

            var process = Process.Start(startInfo);

            if (process == null)
            {
                return -1;
            }

            await process.WaitForExitAsync();
            return process.ExitCode;
        }
    }
}
