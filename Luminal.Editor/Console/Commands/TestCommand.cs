using Luminal.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Editor.Console.Commands
{
    [
        ConCommand("test")
    ]
    public class TestCommand : IConCommand
    {
        public void Run(Arguments a)
        {
            Log.Debug("TestCommand.Run");
        }
    }
}
