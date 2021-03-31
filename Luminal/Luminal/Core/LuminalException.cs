using System;

namespace Luminal.Core
{
    public class LuminalException : ApplicationException
    {
        new public string Message;

        public LuminalException(string msg) => Message = msg;
    }
}