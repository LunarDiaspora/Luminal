using Luminal.Logging;

namespace Luminal.Editor.Console.Commands
{
    [ConCommand("arg_test")]
    [RequiredArgument("cool", ArgumentType.STRING)]
    [OptionalArgument("cool2", ArgumentType.STRING)]
    public class ArgumentTest : IConCommand
    {
        public void Run(Arguments a)
        {
            Log.Debug($"a['cool'] = {a.Get("cool")}");
            Log.Debug($"a['cool2'] = {a.Get("cool2", "none")}");
        }
    }
}
