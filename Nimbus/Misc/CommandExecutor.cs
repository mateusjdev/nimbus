using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
        Message, // cmd msg.exe

        // TODO: PainelPing -> PainelCommand
        Ping, // Ping
    }

    internal enum CommandStatus
    {
        Waiting,
        Running,
        Finished
    }

    internal struct Machine
    {
        // TODO: IPv6 support
        internal IPAddress? IpAddress { get; }

        internal string? DomainName { get; }
    }

    internal class CommandExecutor
    {
        private readonly CommandType commandType;

        private readonly List<Machine> commandTargets = [];

        public bool HasDisplay { get; private set; }

        public CommandStatus Status { get; private set; }

        internal CommandExecutor(CommandType tipo, Machine[] targets)
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

        internal void AddTarget(Machine target)
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
                        tasks.Add(Shutdown(commandTargets[i]));
                    }
                    break;
                case CommandType.Reboot:
                    for (int i = 0; i < count; i++)
                    {
                        tasks.Add(Reboot(commandTargets[i]));
                    }
                    break;
                case CommandType.WakeUp:
                    for (int i = 0; i < count; i++)
                    {
                        tasks.Add(WakeUp(commandTargets[i]));
                    }
                    break;
                case CommandType.Shell:
                    for (int i = 0; i < count; i++)
                    {
                        tasks.Add(Shell(commandTargets[i]));
                    }
                    break;
                case CommandType.Ping:
                    for (int i = 0; i < count; i++)
                    {
                        tasks.Add(Ping(commandTargets[i]));
                        Console.WriteLine("Teste");
                    }
                    break;
                case CommandType.Message:
                    for (int i = 0; i < count; i++)
                    {
                        tasks.Add(Message(commandTargets[i], "Hello World"));
                        Console.WriteLine("Teste");
                    }
                    break;
            }

            var tasksResults = await Task.WhenAll(tasks);

            Status = CommandStatus.Finished;

            return tasksResults;
        }

        private async static Task<int> Shutdown(Machine machine)
        {
            await Task.Delay(1000);
            return 0;
        }

        private async static Task<int> Reboot(Machine machine)
        {
            await Task.Delay(1000);
            return 0;
        }

        private async static Task<int> WakeUp(Machine machine)
        {
            await Task.Delay(1000);
            return 0;
        }

        private async static Task<int> Shell(Machine machine)
        {
            await Task.Delay(1000);
            return 0;
        }

        private async static Task<int> Ping(Machine machine)
        {
            await Task.Delay(1000);
            return 0;
        }

        private async static Task<int> Message(Machine machine, string message)
        {
            var IpAddress = await ResolveDomainName(machine);
            if (IpAddress == null)
            {
                return -1; // TODO: Return error
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                // /c tells cmd to run the command and then terminate
                // $"msg * /server:{ip} {message}"
                Arguments = $"/c msg %username% {IpAddress} : {message}",
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

        private async static Task<IPAddress?> ResolveDomainName(Machine machine)
        {
            IPAddress? IpAddress = null;
            if (machine.IpAddress != null)
            {
                IpAddress = machine.IpAddress;
            }
            else if (machine.DomainName != null)
            {
                var addresses = await Dns.GetHostAddressesAsync(machine.DomainName);
                if (addresses.Length > 0)
                {
                    IpAddress = addresses[0];
                }
            }
            return IpAddress;
        }
    }
}
