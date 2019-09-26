using System.Collections.Generic;

namespace Assets.Project.ChessEngine
{
    public class PvTableValue
    {
        public Move Move { get; set; }
        public int Score { get; set; }
        public int Depth { get; set; }
        public ulong StateKey { get; set; }
    };
    public class PvTable
    {
        private static readonly ulong MaxEntries = 10000000;
        private PvTableValue[] values = new PvTableValue[MaxEntries];
        public void Clear()
        {
            for (ulong i = 0; i < MaxEntries; ++i) values[i] = null;
        }
        public bool ProbeHashEntry(Board board, ref Move move, ref int score, int depth, int alpha, int beta)
        {
            PvTableValue value;
            if (TryGetValue(board.StateKey, out value))
            {
                move = value.Move;
                if (value.Depth >= depth)
                {
                    score = value.Score;
                    if (score > Constants.IsMate) score += board.Ply;
                    else if (score < -Constants.IsMate) score -= board.Ply;
                }
            }
            return false;
        }

        public void StoreHashEntry(Board board, Move move, int score, int depth)
        {
            if (score > Constants.IsMate) score += board.Ply;
            else if (score < -Constants.IsMate) score -= board.Ply;

            ulong key = board.StateKey % MaxEntries;

            values[key] = new PvTableValue() { Move = move, Score = score, Depth = depth, StateKey = board.StateKey };
        }

        public bool TryGetValue(ulong StateKey, out PvTableValue value)
        {
            ulong key = StateKey % MaxEntries;
            value = values[key];
            if (value == null || value.StateKey != StateKey)
            {
                value = null;
                return false;
            }
            return true;
        }
    }
}
