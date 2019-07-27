using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Project.ChessEngine.Exceptions
{
    public class FENFormatException : Exception
    {
        public FENFormatException() {}

        public FENFormatException(string message) : base(message) {}

        public FENFormatException(string message, Exception inner) : base(message, inner) {}
    }
}
