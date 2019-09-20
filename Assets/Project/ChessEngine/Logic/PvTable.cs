using System.Collections.Generic;

namespace Assets.Project.ChessEngine
{
    public class PvTableValue
    {
        public Move Move { get; set; }
        public int Score { get; set; }
        public int Depth { get; set; }
    };
    public class PvTable : Dictionary<ulong, PvTableValue>
    {
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

            this[board.StateKey] = new PvTableValue() { Move = move, Score = score, Depth = depth };
        }
    }
}
