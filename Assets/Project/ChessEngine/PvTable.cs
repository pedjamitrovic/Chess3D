using Assets.Project.ChessEngine.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using static Assets.Project.ChessEngine.PvTable;

namespace Assets.Project.ChessEngine
{
    public class PvTable : Dictionary<ulong, PvTableValue>
    {
        public class PvTableValue
        {
            public Move Move { get; set; }
            public int Score { get; set; }
            public int Depth { get; set; }
        };
        public int NewWriteCounter { get; private set; } = 0;
        public int OverWriteCounter { get; private set; } = 0;
        public int Hits { get; private set; } = 0;

        public bool ProbeHashEntry(Board board, ref Move move, ref int score, int depth, int alpha, int beta)
        {
            if (TryGetValue(board.StateKey, out PvTableValue value))
            {
                move = value.Move;
                if (value.Depth >= depth)
                {
                    ++Hits;

                    score = value.Score;
                    if (score > Constants.IsMate) score += board.Ply;
                    else if (score < -Constants.IsMate) score -= board.Ply;
                }
            }
            return false;
        }

        public void StoreHashEntry(Board board, Move move, int score, int depth)
        {
            if (ContainsKey(board.StateKey)) ++NewWriteCounter;
            else ++OverWriteCounter;

            if (score > Constants.IsMate) score += board.Ply;
            else if (score < -Constants.IsMate) score -= board.Ply;

            this[board.StateKey] = new PvTableValue() { Move = move, Score = score, Depth = depth };
        }
    }
}
