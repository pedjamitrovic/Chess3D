using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Project.ChessEngine.Exceptions
{
    public class BoardIntegrityException : Exception
    {
        public BoardIntegrityException() {}

        public BoardIntegrityException(string message) : base(message) {}

        public BoardIntegrityException(string message, Exception inner) : base(message, inner) {}
    }
}
