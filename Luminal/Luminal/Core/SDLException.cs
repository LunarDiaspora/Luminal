using System;

namespace Luminal.Core
{
    public class SDLException : ApplicationException
    {
        new public string Message;

        public SDLException(string msg) => Message = msg;
    }
}