using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Project.ChessEngine.Exceptions
{
    public class IllegalArgumentException : Exception
    {
        public IllegalArgumentException() {}

        public IllegalArgumentException(string message) : base(message) {}

        public IllegalArgumentException(string message, Exception inner) : base(message, inner) {}
    }
}
