using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Logging
{
    public interface ILogger
    {
        public void Log(string message, LogLevel level);
    }
}
