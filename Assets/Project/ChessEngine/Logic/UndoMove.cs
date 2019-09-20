using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Project.ChessEngine
{
    public class UndoMove
    {
        public Move Move { get; set; }
        public int CastlePerm { get; set; }
        public Square EnPassant { get; set; }
        public int FiftyMove { get; set; }
        public ulong StateKey { get; set; }
    }
}
