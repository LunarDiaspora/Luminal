using Luminal.Editor.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Editor.Console.Commands
{
    [ConCommand("clear", "Clears the console.")]
    class ClearCommand : IConCommand
    {
        public void Run(Arguments a)
        {
            Editor.ConsoleOutput.Clear();
            DebugConsole.ScrollDown();
        }
    }
}
