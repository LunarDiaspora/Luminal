using Luminal.Logging;

namespace Luminal.Editor.Console.Commands
{
    [ConCommand("test")]
    public class TestCommand : IConCommand
    {
        public void Run(Arguments a)
        {
            Log.Debug("TestCommand.Run");
        }
    }
}
